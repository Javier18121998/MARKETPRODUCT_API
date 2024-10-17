using MARKETPRODUCT_API.Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MARKETPRODUCT_API.Services.IServices;
using MARKETPRODUCT_API.Services;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.MARKETUtilities;
using MARKETPRODUCT_API.Data.ModelValidations.IDataModelValidations;
using MARKETPRODUCT_API.Data.ModelValidations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"], // El emisor del token (Issuer)
                    ValidAudience = Configuration["Jwt:Audience"], // El público objetivo (Audience)
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) // Llave secreta para validar el token
                };
            });

            // Agrega autorización
            services.AddAuthorization(options =>
            {
                // Puedes configurar políticas de autorización personalizadas si lo necesitas
            });

            // Configura Swagger para que soporte autenticación JWT
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(MarketUtilities.CurrentVersion, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersion,
                    Description = MarketUtilities.SwaggerDocDescription
                });
                c.EnableAnnotations(); // Enables annotations for better Swagger UI customization.

                // Definición del esquema de seguridad para JWT (Bearer Token)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese 'Bearer' seguido de su token JWT en el campo de texto. Ejemplo: 'Bearer abc123def456'"
                });

                // Requerimiento de seguridad para aplicar Bearer Token en los endpoints protegidos
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // Configures the Entity Framework DbContext with SQL Server.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MarketUtilities.DefaultConnection))
            );

            // Registers application services with their interfaces.
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductModelValidations, ProductModelValidations>();
            services.AddScoped<IOrderModelValidations, OrderModelValidations>();
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

            app.UseAuthentication(); // Asegura que las solicitudes sean autenticadas
            app.UseAuthorization();

            // Configures the endpoints for the controllers.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Maps controller routes to the endpoints.
            });
        }
    }
}
