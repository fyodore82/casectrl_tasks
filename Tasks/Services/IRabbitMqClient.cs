using Tasks.Models;

namespace Tasks.Services
{
    public interface IRabbitMqClient
    {
        public void SendMessage(string routingKey, ToDoRabbitMessage message);
    }
}
