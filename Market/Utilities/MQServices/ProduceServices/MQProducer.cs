using Market.Utilities.MQServices.MQModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Market.Utilities.MQServices.ProduceServices
{
    public class MQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public MQProducer()
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

        /// <summary>
        /// Envía un mensaje a la cola especificada.
        /// El mensaje se serializa a formato JSON antes de ser enviado.
        /// </summary>
        /// <typeparam name="T">El tipo del mensaje que se va a enviar.</typeparam>
        /// <param name="message">El mensaje que se va a enviar.</param>
        public void SendMessage<T>(
            short idTransaction,
            string nameTransaction,
            string descriptionTransaction,
            DateTime dateTimeTransaction,
            string inPathEndpoint,
            string typeElement
            )
        {
            nameTransaction = TransactionNameCraper(8, nameTransaction);
            var bodyMQService = new MQBodyTransaction
            {
                Message = new LogMessage
                { 
                    TransactionId = idTransaction,
                    TransactionName = nameTransaction,
                    TransactionDescription = descriptionTransaction,
                    TransactionTime = dateTimeTransaction,
                    TransactionPathEndpoint = inPathEndpoint
                },
                TypeElement = typeElement
            };

            var jsonMessage = JsonConvert.SerializeObject(bodyMQService);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "logs_queue",
                basicProperties: null,
                body: body
            );
            Console.WriteLine($"[x] Mensaje enviado: {jsonMessage}");
        }

        private static string TransactionNameCraper(int length, string nameTransacction)
        {
            using (SHA3_256 sha256 = SHA3_256.Create())
            {
                string randomValue = Guid.NewGuid().ToString();
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(randomValue));

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2")); 
                }
                var crapetValue = sb.ToString().Substring(0, length);
                var shuffleStatement = ComplexShuffle(nameTransacction, crapetValue);
                return shuffleStatement;
            }
        }

        private static string ComplexShuffle(string nameTransaction, string crapetValue)
        {
            string combined = nameTransaction + crapetValue;
            char[] array = combined.ToCharArray();

            for (int i = 0; i < array.Length - 1; i += 2)
            {
                array[i] = (char)(array[i] ^ array[i + 1]); 
            }

            int hashSeed = GetHashSeed(combined);
            Random rng = new Random(hashSeed);

            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = (rng.Next(n + 1) * 31) % array.Length; 
                var temp = array[k];
                array[k] = array[n];
                array[n] = temp;
            }

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ShiftBitsCircular(array[i], 3); 
            }

            return new string(array);
        }

        private static int GetHashSeed(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int seed = BitConverter.ToInt32(hashBytes, 0);
                return Math.Abs(seed);
            }
        }

        private static char ShiftBitsCircular(char c, int positions)
        {
            return (char)(((c >> positions) | (c << (8 - positions))) & 0xFF);
        }
    }
}
