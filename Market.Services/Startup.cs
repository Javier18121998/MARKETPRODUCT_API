using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Market.Market.Models;
using Market.OrdersController;
using Market.ProductsController;
using MARKETPRODUCT_API.Controllers;
using Market.Exceptions.Middlewares;
using MARKETPRODUCT_API.MARKETUtilities;
using Microsoft.AspNetCore.Mvc;
//using Market.Market.Models;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
            services.AddControllers();

            // Configura autenticación con JWT
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
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddAuthorization();

            // Configuración de versionamiento de API
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0); // v1.0 como versión predeterminada
                options.AssumeDefaultVersionWhenUnspecified = true; // Usa la versión predeterminada si no se especifica
                options.ReportApiVersions = true; // Devuelve las versiones soportadas en la respuesta
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // Formato de la versión
                options.SubstituteApiVersionInUrl = true; // Habilita la versión en la URL
            });

            // Configuración de Swagger para varias versiones
            services.AddSwaggerGen(c =>
            {
                // Define la documentación para v1
                c.SwaggerDoc(MarketUtilities.CurrentVersionV1, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersionV1,
                    Description = MarketUtilities.SwaggerDocDescription
                });

                // Define la documentación para v2
                c.SwaggerDoc(MarketUtilities.CurrentVersionV2, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersionV2,
                    Description = MarketUtilities.SwaggerDocDescriptionV2
                });

                c.DocInclusionPredicate((version, apiDesc) =>
                {
                    if (!apiDesc.GroupName.Equals(version, StringComparison.OrdinalIgnoreCase)) return false;
                    return true;
                });

                // Configuración de seguridad para JWT en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingrese 'Bearer' seguido de su token JWT. Ejemplo: 'Bearer abc123def456'"
                });

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

            services.AddDbContext<MarketDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MarketUtilities.DefaultConnection))
            );

            // Registro de servicios de aplicación
            services.AddScoped<Market.BL.IBL.IOrderServiceBL, Market.BL.OrderServiceBL>();
            services.AddScoped<Market.BL.IBL.IProductServiceBL, Market.BL.ProductServiceBL>();
            services.AddScoped<Market.DAL.IDAL.IProductService, Market.DAL.ProductService>();
            services.AddScoped<Market.DAL.IDAL.IOrderService, Market.DAL.OrderService>();
            services.AddScoped<Market.DataValidation.IDataBaseValidations.IProductValidationService, Market.DataValidation.DataBaseValidations.ProductValidationService>();
            services.AddScoped<Market.DataValidation.IDataBaseValidations.IOrderValidationService, Market.DataValidation.DataBaseValidations.OrderValidationService>();
            services.AddScoped<Market.Utilities.MQServices.IManageServices.IMQManagerService, Market.Utilities.MQServices.ManageServices.MQManagerService>();
            services.AddScoped<Market.Utilities.MQServices.IProduceServices.IMQProducer, Market.Utilities.MQServices.ProduceServices.MQProducer>();
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
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(MarketUtilities.SwaggerUrlEndpointV1, MarketUtilities.SwaggerNameEndpointV1);
                    c.SwaggerEndpoint(MarketUtilities.SwaggerUrlEndpointV2, MarketUtilities.SwaggerNameEndpointV2);
                });
            }

            app.UseMiddleware<MarketHandlingMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
