using Market.BL.IBL;
using Market.BL;
using Market.DAL.IDAL;
using Market.DAL;
using Market.DataValidation.IDataBaseValidations;
using Market.DataValidation.DataBaseValidations;
using Market.Utilities.MQServices.IManageServices;
using Market.Utilities.MQServices.ManageServices;
using Market.Utilities.MQServices.IProduceServices;
using Market.Utilities.MQServices.ProduceServices;
using Market.AuthorizationController.AuthServices.UserRoleServices.IUserServices;
using Market.AuthorizationController.AuthServices.UserRoleServices.UserServices;
using Market.AuthorizationController.AuthServices;
using MARKETPRODUCT_API.MARKETUtilities;
using Market.Exceptions.Middlewares;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Market.Market.Models;
using Microsoft.EntityFrameworkCore;

namespace MARKETPRODUCT_API
{
    /// <summary>
    /// Provides centralized methods for configuring and registering 
    /// application services, middleware, and Swagger configurations.
    /// </summary>
    public static class CommonStartup
    {
        /// <summary>
        /// Registers common application services into the service collection.
        /// This includes services for business logic, data access, validation,
        /// message queuing, and user management.
        /// </summary>
        /// <param name="services">The service collection to which services are added.</param>
        public static void RegisterCommonServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderServiceBL, OrderServiceBL>();
            services.AddScoped<IProductServiceBL, ProductServiceBL>();
            services.AddScoped<ICustomerServiceBL, CustomerServiceBL>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductValidationService, ProductValidationService>();
            services.AddScoped<IOrderValidationService, OrderValidationService>();
            services.AddScoped<IMQManagerService, MQManagerService>();
            services.AddScoped<IMQProducer, MQProducer>();
            services.AddScoped<IUserService, UserService>();
        }

        /// <summary>
        /// Configures common middleware components for the application pipeline.
        /// This includes handling errors, enabling Swagger in development, and 
        /// configuring HTTPS redirection, authentication, and routing.
        /// </summary>
        /// <param name="app">The application builder to configure middleware components.</param>
        /// <param name="env">The hosting environment that determines the app's behavior.</param>
        public static void CommonConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
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
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Handles errors in production
                app.UseHsts(); // Adds additional security for production
            }

            app.UseCors("MyPoliticalCors");

            app.UseMiddleware<MarketHandlingMiddleware>(); // Custom middleware for exception handling

            app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS
            app.UseRouting(); // Configures endpoint routing
            app.UseAuthentication(); // Adds authentication to the pipeline
            app.UseAuthorization(); // Adds authorization to the pipeline
        }

        /// <summary>
        /// Configures Swagger services for API documentation.
        /// This includes support for multiple API versions, annotations,
        /// and JWT security settings in Swagger.
        /// </summary>
        /// <param name="services">The service collection to which Swagger services are added.</param>
        public static void CommonSwaggerConfigurations(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                // Define documentation for version 1
                c.SwaggerDoc(MarketUtilities.CurrentVersionV1, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersionV1,
                    Description = MarketUtilities.SwaggerDocDescription
                });

                // Define documentation for version 2
                c.SwaggerDoc(MarketUtilities.CurrentVersionV2, new OpenApiInfo
                {
                    Title = MarketUtilities.PedidoAPI,
                    Version = MarketUtilities.CurrentVersionV2,
                    Description = MarketUtilities.SwaggerDocDescriptionV2
                });

                c.EnableAnnotations(); // Enable annotations for better documentation

                c.DocInclusionPredicate((version, apiDesc) =>
                {
                    return string.Equals(apiDesc.GroupName, version, StringComparison.OrdinalIgnoreCase);
                });


                // Configure JWT security for Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your JWT token. Example: 'Bearer abc123def456'."
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
                        new string[] { }
                    }
                });
            });
        }

        /// <summary>
        /// Configures API versioning, including the default version, the ability to assume a default version 
        /// if one is not specified, and the inclusion of supported versions in responses.
        /// </summary>
        /// <param name="services">The service container where API versioning services are registered.</param>
        public static void CommonVersioningApplication(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                }
            );

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        /// <summary>
        /// Configures the database using Entity Framework and SQL Server. 
        /// This method retrieves the connection string from the configuration and sets up command timeout and logging.
        /// </summary>
        /// <param name="services">The service container where the database context is registered.</param>
        /// <param name="configuration">The application configuration containing the connection string and other relevant parameters.</param>
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string marketProductDBConn = configuration.GetConnectionString(MarketUtilities.DefaultConnection) ?? string.Empty;
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            if (!string.IsNullOrEmpty(marketProductDBConn))
            {
                services.AddDbContext<MarketDbContext>(options =>
                    options.UseSqlServer(marketProductDBConn,
                        sqlServerOptions =>
                        {
                            sqlServerOptions.CommandTimeout(Convert.ToInt32(configuration.GetValue<string>("SqlCommandTimeout") ?? "30"));
                        })
                    .LogTo(msg =>
                    {
                        string loglevel = configuration.GetValue<string>("LogLevel") ?? "Error";
                        if (loglevel.Equals("Trace", StringComparison.OrdinalIgnoreCase))
                        {
                            NLog.LogManager.GetCurrentClassLogger().Trace(msg);
                        }
                    })
                );
            }
            else
                logger.Error("Error, missing configuration 'MarketProductDB'");
        }
    }
}