using Market.Utilities.MQServices.MQModels;

namespace Market.Utilities.MQServices.IManageServices
{
    /// <summary>
    /// The IMQManagerService interface defines the contract for managing message sending operations.
    /// </summary>
    public interface IMQManagerService
    {
        /// <summary>
        /// Configures and sends a message asynchronously.
        /// </summary>
        /// <param name="logMessage">The log message containing transaction details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ConfigureMessageSendingAsync(LogMessage logMessage);
    }
}
