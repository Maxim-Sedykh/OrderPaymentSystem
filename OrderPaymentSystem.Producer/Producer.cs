using Newtonsoft.Json;
using OrderPaymentSystem.Producer.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace OrderPaymentSystem.Producer
{
    public class Producer : IMessageProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Producer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessage<T>(T message, string routingKey, string? exchange = null)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                return;
            }

            var json = JsonConvert.SerializeObject(message, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                });

            var body = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(exchange, routingKey, basicProperties: null, body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
