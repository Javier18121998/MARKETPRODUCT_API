using System;

namespace Market.Utilities.MQServices.IProduceServices
{
    /// <summary>
    /// The IMQProducer interface defines the contract for a message producer.
    /// It specifies the method for sending messages to a specified queue.
    /// </summary>
    public interface IMQProducer
    {
        /// <summary>
        /// Sends a message to the specified queue.
        /// </summary>
        /// <typeparam name="T">The type of the message to be sent.</typeparam>
        /// <param name="idTransaction">The ID of the transaction.</param>
        /// <param name="nameTransaction">The name of the transaction.</param>
        /// <param name="descriptionTransaction">The description of the transaction.</param>
        /// <param name="dateTimeTransaction">The date and time of the transaction.</param>
        /// <param name="inPathEndpoint">The incoming endpoint of the transaction.</param>
        /// <param name="typeElement">The type of element associated with the transaction.</param>
        Task SendMessageAsync<T>(
            short idTransaction,
            string nameTransaction,
            string descriptionTransaction,
            DateTime dateTimeTransaction,
            string inPathEndpoint,
            string typeElement
        );
    }
}
