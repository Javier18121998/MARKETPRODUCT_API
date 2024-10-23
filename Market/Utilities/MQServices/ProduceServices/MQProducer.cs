using Market.Utilities.MQServices.IProduceServices;
using Market.Utilities.MQServices.MQModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Market.Utilities.MQServices.ProduceServices
{
    /// <summary>
    /// The MQProducer class provides functionality to send messages to a RabbitMQ queue.
    /// It handles connection initialization, queue declaration, and sending messages serialized in JSON format.
    /// It also includes methods for securely and randomly manipulating transaction names.
    /// </summary>
    public class MQProducer : IMQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        /// <summary>
        /// Constructor for the MQProducer class.
        /// Initializes the connection to RabbitMQ and declares the queue where messages will be sent.
        /// </summary>
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
                      arguments: null
            );
        }

        /// <summary>
        /// Sends a message to the RabbitMQ queue.
        /// The message is serialized into JSON format and sent to the specified queue.
        /// </summary>
        /// <typeparam name="T">The generic type of the message to be sent.</typeparam>
        /// <param name="idTransaction">The transaction ID.</param>
        /// <param name="nameTransaction">The name of the transaction, which will be processed before sending.</param>
        /// <param name="descriptionTransaction">The description of the transaction.</param>
        /// <param name="dateTimeTransaction">The date and time of the transaction.</param>
        /// <param name="inPathEndpoint">The incoming endpoint related to the transaction.</param>
        /// <param name="typeElement">The type of element associated with the transaction.</param>
        public async Task SendMessageAsync<T>(
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

            // Publicar el mensaje de manera asincrónica
            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: "logs_queue",
                    basicProperties: null,
                    body: body
                );
                Console.WriteLine($"[x] Message sent: {jsonMessage}");
            });
        }


        /// <summary>
        /// Creates a secure and randomly manipulated transaction name based on a hash value.
        /// Uses SHA3-256 to create a unique value and add complexity to the transaction name.
        /// </summary>
        /// <param name="length">Length of the hash value generated that will be appended to the transaction name.</param>
        /// <param name="nameTransaction">The original transaction name.</param>
        /// <returns>A manipulated and secure transaction name.</returns>
        private static string TransactionNameCraper(int length, string nameTransaction)
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
                var shuffleStatement = ComplexShuffle(nameTransaction, crapetValue);
                return shuffleStatement;
            }
        }

        /// <summary>
        /// Applies a complex shuffling algorithm to a transaction name and a hash value.
        /// Utilizes XOR and circular bit shifts to transform the name.
        /// </summary>
        /// <param name="nameTransaction">The original transaction name.</param>
        /// <param name="crapetValue">The hash value generated to mix with the transaction name.</param>
        /// <returns>The shuffled and encrypted transaction name.</returns>
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

        /// <summary>
        /// Generates a hash seed based on the input string using SHA256.
        /// </summary>
        /// <param name="input">The input string to hash.</param>
        /// <returns>An integer seed derived from the hash of the input string.</returns>
        private static int GetHashSeed(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                int seed = BitConverter.ToInt32(hashBytes, 0);
                return Math.Abs(seed);
            }
        }

        /// <summary>
        /// Performs a circular bit shift on a character by a specified number of positions.
        /// </summary>
        /// <param name="c">The character to shift.</param>
        /// <param name="positions">The number of positions to shift.</param>
        /// <returns>The character after shifting its bits circularly.</returns>
        private static char ShiftBitsCircular(char c, int positions)
        {
            return (char)(((c >> positions) | (c << (8 - positions))) & 0xFF);
        }
    }
}
