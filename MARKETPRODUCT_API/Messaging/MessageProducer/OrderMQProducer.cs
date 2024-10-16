using MARKETPRODUCT_API.MARKETUtilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MARKETPRODUCT_API.Messaging.MessageProducer
{
    public class OrderMQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OrderMQProducer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = MarketUtilities.SpaceName,
                UserName = MarketUtilities.UserName,
                Password = MarketUtilities.UserPassword,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: MarketUtilities.LogsQueue,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void SendMessage<T>(T message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "",
                                  routingKey: MarketUtilities.LogsQueue,
                                  basicProperties: null,
                                  body: body);
            Console.WriteLine($"[x] Mensaje enviado: {jsonMessage}");
        }
    }
}
