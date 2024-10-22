using MARKETPRODUCT_API.MARKETUtilities;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MARKETPRODUCT_API.Messaging.MessageProducer
{
    /// <summary>
    /// Clase que representa un productor de mensajes para RabbitMQ.
    /// Se encarga de establecer la conexión con el servidor RabbitMQ,
    /// declarar una cola y enviar mensajes serializados en formato JSON.
    /// </summary>
    public class MQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MQProducer()
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

        /// <summary>
        /// Envía un mensaje a la cola especificada.
        /// El mensaje se serializa a formato JSON antes de ser enviado.
        /// </summary>
        /// <typeparam name="T">El tipo del mensaje que se va a enviar.</typeparam>
        /// <param name="message">El mensaje que se va a enviar.</param>
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
