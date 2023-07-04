using ReviewService.Models;

namespace ReviewService.MessageBroker
{
    public interface IMessageBrokerClient
    {
        public void SendMessage(Message message);

        public void ReceiveMessage();
    }
}
