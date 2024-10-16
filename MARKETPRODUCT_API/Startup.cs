using MARKETPRODUCT_API.Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MARKETPRODUCT_API.Services.IServices;
using MARKETPRODUCT_API.Services;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.MARKETUtilities;
using MARKETPRODUCT_API.Data.ModelValidations.IDataModelValidations;
using MARKETPRODUCT_API.Data.ModelValidations;

namespace MARKETPRODUCT_API
{
    /// <summary>
    /// Configures services and middleware components for the MARKETPRODUCT_API application.
    /// This class handles dependency injection and sets up the necessary configurations for services.
    /// </summary>
    public class Startup
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
            services.AddControllers(); // Adds MVC services to the application.

            // Configures Swagger for API documentation generation.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(MarketUtilities.CurrentVersion, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersion,
                    Description = MarketUtilities.SwaggerDocDescription
                });
                c.EnableAnnotations(); // Enables annotations for better Swagger UI customization.
            });

            // Configures the Entity Framework DbContext with SQL Server.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MarketUtilities.DefaultConnection))
            );

            // Registers application services with their interfaces.
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductModelValidations, ProductModelValidations>();
            services.AddSingleton<MQProducer>(); // Registers the message queue producer as a singleton.
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
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Provides detailed error pages in development.
                app.UseSwagger(); // Enables the Swagger middleware.
                app.UseSwaggerUI(c => c.SwaggerEndpoint(MarketUtilities.SwaggerUrlEndpoint, MarketUtilities.SwaggerNameEndpoint)); // Configures the Swagger UI endpoint.
            }

            app.UseRouting(); // Enables routing capabilities for the app.

            // Configures the endpoints for the controllers.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Maps controller routes to the endpoints.
            });
        }
    }
}
