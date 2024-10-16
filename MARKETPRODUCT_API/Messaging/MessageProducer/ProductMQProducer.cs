using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MARKETPRODUCT_API.Messaging.MessageProducer
{
    public class ProductMQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public ProductMQProducer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost", 
                UserName = "guest",   
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "logs_queue",
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
                                  routingKey: "logs_queue",
                                  basicProperties: null,
                                  body: body);
            Console.WriteLine($"[x] Mensaje enviado: {jsonMessage}");
        }
    }
}
