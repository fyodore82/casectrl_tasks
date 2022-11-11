using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using Tasks.Models;
using System.Text.Json;
using Tasks.Services;

namespace Tasks.Services
{
    public class RabbitMqClient : IRabbitMqClient
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly AppSettings _appSettings;

        public RabbitMqClient(ConnectionFactory connectionFactory,
            IOptions<AppSettings> appSettings)
        {
            _connectionFactory = connectionFactory;
            _appSettings = appSettings.Value;
        }

        public void SendMessage(string routingKey, ToDoRabbitMessage message)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _appSettings.RabbitMq.Query,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                channel.BasicPublish(exchange: _appSettings.RabbitMq.Exchange,
                                     routingKey: routingKey,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
