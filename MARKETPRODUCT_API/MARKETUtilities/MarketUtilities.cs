namespace MARKETPRODUCT_API.MARKETUtilities
{
    /// <summary>
    /// Contains application-wide constants and utility values for configuration.
    /// This class centralizes key settings related to messaging queues, database connections, 
    /// and API versioning to promote maintainability and consistency across the application.
    /// </summary>
    public static class MarketUtilities
    {
        public const string LogsQueue = "logs_queue";
        public const string SpaceName = "localhost";
        public const string UserName = "guest";
        public const string UserPassword = "guest";
        public const string DefaultConnection = "DefaultConnection";
        public const string PedidoAPI = "PedidoAPI";
        public const string CurrentVersion = "v1";
        public const string SwaggerUrlEndpoint = "/swagger/v1/swagger.json";
        public const string SwaggerNameEndpoint = "PedidoAPI v1";
        public const string SwaggerDocDescription = "API to understand the microservices communication with Message Brokers-RabbitMQ";
    }
}
