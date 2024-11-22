using Market.DataModels.DTos;
using Market.AuthorizationController.AuthConfigurations;

namespace MARKETPRODUCT_API
{
    /// <summary>
    /// Configures services and middleware components for the MARKETPRODUCT_API application.
    /// This class handles dependency injection and sets up the necessary configurations for services.
    /// </summary>
    public class Startup : CommonStartup
    {
        /// <summary>
        /// Initializes a new instance of the Startup class with the specified configuration.
        /// </summary>
        /// <param name="configuration">Configuration settings for the application.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration settings.
        /// This property provides access to application-wide configuration values.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services required for the application.
        /// This includes adding MVC controllers, setting up Swagger for API documentation, 
        /// and registering application-specific services such as database contexts and service interfaces.
        /// </summary>
        /// <param name="services">The service collection to register services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.CommonCorsConfigurations();
            services.AddControllers();
            services.AddHttpContextAccessor();
            #region JWTConfiguration Auth
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.ConfigureJwtSettings(Configuration);
            #endregion
            // Configura autentication with JWT
            services.AddJwtAuthentication(Configuration);
            // Configure Authorization 
            services.AddAuthorization();
            // Configure API Versioning
            CommonVersioningApplication(services);
            // Swagger Configuration for many versions
            CommonSwaggerConfigurations(services);
            // Database Configuration
            ConfigureDatabase(services, Configuration);
            //Adding JWTBEarer services
            #region JWTConfiguration Auth
            services.JwtBearerServices();
            #endregion
            // Application Services Registration
            #region Registry of services application
            RegisterCommonServices(services);
            #endregion
        }


        /// <summary>
        /// Configures middleware components for the application pipeline.
        /// This includes error handling, Swagger UI configuration, routing capabilities, 
        /// and endpoint mapping for controllers to handle incoming requests.
        /// </summary>
        /// <param name="app">The application builder to configure middleware.</param>
        /// <param name="env">The hosting environment to determine the app's behavior.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CommonConfigure(app, env);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
