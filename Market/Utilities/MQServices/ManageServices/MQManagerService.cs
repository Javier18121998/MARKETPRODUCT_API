﻿using Market.Utilities.MQServices.IManageServices;
using Market.Utilities.MQServices.IProduceServices;
using Market.Utilities.MQServices.MQModels;
using System;
using System.Threading.Tasks;

namespace Market.Utilities.MQServices.ManageServices
{
    /// <summary>
    /// The MQManagerService class is responsible for managing message sending operations.
    /// </summary>
    public class MQManagerService : IMQManagerService
    {
        private readonly IMQProducer _mQProducer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MQManagerService"/> class.
        /// </summary>
        /// <param name="mQProducer">The message producer instance used for sending messages.</param>
        public MQManagerService(IMQProducer mQProducer)
        {
            _mQProducer = mQProducer;
        }

        /// <summary>
        /// Configures and sends a message asynchronously.
        /// </summary>
        /// <param name="logMessage">The log message containing transaction details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ConfigureMessageSendingAsync(LogMessage logMessage, bool isTransactionSuccess = true)
        {
            // Validación de parámetros
            if (string.IsNullOrEmpty(logMessage.TransactionName))
                throw new ArgumentException("El nombre de la transacción no puede estar vacío.", nameof(logMessage.TransactionName));

            if (string.IsNullOrEmpty(logMessage.TransactionPathEndpoint))
                throw new ArgumentException("El endpoint de la transacción no puede estar vacío.", nameof(logMessage.TransactionPathEndpoint));

            if (string.IsNullOrEmpty(logMessage.TransactionDescription))
                throw new ArgumentException("La descripción de la transacción no puede estar vacía.", nameof(logMessage.TransactionDescription));

            string typeElement;
            if (isTransactionSuccess)
            {
                // Configuración del elemento tipo si es success
                 typeElement = ConfigureTypeElementInTransaction(logMessage.TransactionPathEndpoint);
            }
            else
            {
                // Configuración del elemento tipo si no es success
                typeElement = "ZMPT0000000";
            }

            // Crear el objeto MQBodyTransaction
            var mqBodyTransaction = new MQBodyTransaction
            {
                Message = logMessage,
                TypeElement = typeElement
            };

            // Enviar el mensaje
            await _mQProducer.SendMessageAsync<MQBodyTransaction>(
                logMessage.TransactionId,
                logMessage.TransactionName,
                logMessage.TransactionDescription,
                logMessage.TransactionTime,
                logMessage.TransactionPathEndpoint,
                typeElement
            );
        }

        /// <summary>
        /// Configures the type element based on the transaction path endpoint.
        /// </summary>
        /// <param name="transactionPathEndpoint">The endpoint of the transaction.</param>
        /// <returns>A string representing the type element.</returns>
        private string ConfigureTypeElementInTransaction(string transactionPathEndpoint)
        {
            string typePrefix = GetTypePrefix(transactionPathEndpoint);
            return string.IsNullOrEmpty(typePrefix) ? "ZMPT0000000" : $"{typePrefix}00000001";
        }

        /// <summary>
        /// Gets the type prefix based on the transaction path endpoint.
        /// </summary>
        /// <param name="transactionPathEndpoint">The endpoint of the transaction.</param>
        /// <returns>A string representing the type prefix.</returns>
        private string GetTypePrefix(string transactionPathEndpoint)
        {
            string typePrefix = string.Empty;
            if (transactionPathEndpoint.IndexOf("Post", StringComparison.OrdinalIgnoreCase) >= 0 ||
                transactionPathEndpoint.IndexOf("Create", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                typePrefix += "PC";
            }
            if (transactionPathEndpoint.IndexOf("Put", StringComparison.OrdinalIgnoreCase) >= 0 ||
                transactionPathEndpoint.IndexOf("Update", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                typePrefix += "PU";
            }
            if (transactionPathEndpoint.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                typePrefix += "D";
            }
            if (transactionPathEndpoint.IndexOf("Get", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                typePrefix += "G";
            }
            return typePrefix;
        }
    }
}
