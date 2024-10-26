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

        // Versiones de API
        public const string CurrentVersionV1 = "v1";
        public const string CurrentVersionV2 = "v2";

        // Endpoints de Swagger para cada versión
        public const string SwaggerUrlEndpointV1 = "/swagger/v1/swagger.json";
        public const string SwaggerUrlEndpointV2 = "/swagger/v2/swagger.json";

        // Nombres de endpoints de Swagger
        public const string SwaggerNameEndpointV1 = "PedidoAPI v1";
        public const string SwaggerNameEndpointV2 = "PedidoAPI v2";

        // Descripciones para Swagger
        public const string SwaggerDocDescription = "API to understand the microservices communication with Message Brokers-RabbitMQ";
        public const string SwaggerDocDescriptionV2 = "API v2 with additional endpoints for Carts and Customers";
    }
}
