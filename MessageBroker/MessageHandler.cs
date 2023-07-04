using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReviewService.Constants;
using ReviewService.Context;
using ReviewService.Models;
using System.Text;

namespace ReviewService.MessageBroker
{
    public class MessageHandler<T> where T : Product
    {
        private readonly IModel _channel;

        private readonly IServiceProvider _serviceProvider;

        public MessageHandler(IModel channel, IServiceProvider serviceProvider)
        {
            //get servicecontext from injected service container
            _serviceProvider = serviceProvider;

            _channel = channel;

            Console.WriteLine("message handler created");
        }

        public async void HandleMessage(object model, BasicDeliverEventArgs eventArgs)
        {
            using var scope = _serviceProvider.CreateScope();
            var _serviceContext = scope.ServiceProvider.GetRequiredService<ServiceContext>();

            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"message received from queue {message}");

            Message? eventMessage = JsonConvert.DeserializeObject<Message>(message);

            //check if this message is already processed
            if (eventMessage != null)
            {
                string consumerId = "review-service";
                bool alreadyProcessed = await _serviceContext.ConsumedMessages.AnyAsync(message => message.Id == eventMessage.Id
                && message.ConsumerId == consumerId);

                if (alreadyProcessed) return;

                try
                {
                    ConsumedMessage consumedMessage = new ConsumedMessage(eventMessage.Id, consumerId);
                    await _serviceContext.AddAsync(consumedMessage);
                    await _serviceContext.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Perform the message handling logic here based on the event message


            if (eventMessage != null && eventMessage.EventType == EventTypes.PRODUCT_DELETED)
            {
                // Handle the PRODUCT_DELETED event
                // ...
                Product productDeleted = JsonConvert.DeserializeObject<Product>(eventMessage.Payload);

                try
                {
                    IEnumerable<Review>? reviews = _serviceContext.Reviews.Where(review => review.ProductId == productDeleted.Id);

                    _serviceContext.Reviews.RemoveRange(reviews);

                    await _serviceContext.SaveChangesAsync();

                    // Acknowledge the message
                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error occured when deleting review for product id {productDeleted.Id}");
                }
            }

        }
    }
}
