using ReviewService.Constants;
using ReviewService.Context;
using ReviewService.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

            Message<T>? eventMessage = JsonConvert.DeserializeObject<Message<T>>(message);

            // Perform the message handling logic here based on the event message
         

            if (eventMessage !=null && eventMessage.EventType == EventTypes.PRODUCT_DELETED)
            {
                // Handle the PRODUCT_DELETED event
                // ...
                Product productDeleted = eventMessage.Payload;

                try
                {
                    IEnumerable<Review>? reviews = _serviceContext.Reviews.Where(review => review.ProductId == productDeleted.Id);

                    if (reviews.Any())
                    {
                        foreach (Review review in reviews)
                        {
                            _serviceContext.Reviews.Remove(review);
                            await _serviceContext.SaveChangesAsync();
                        }
                    }

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
