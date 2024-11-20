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

namespace MARKETPRODUCT_API
{
    /// <summary>
    /// Provides centralized methods for configuring and registering 
    /// application services, middleware, and Swagger configurations.
    /// </summary>
    public class CommonStartup
    {
        /// <summary>
        /// Registers common application services into the service collection.
        /// This includes services for business logic, data access, validation,
        /// message queuing, and user management.
        /// </summary>
        /// <param name="services">The service collection to which services are added.</param>
        public void RegisterCommonServices(IServiceCollection services)
        {
            var assembly = typeof(Startup).Assembly;
            var typesWithInterfaces = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Implementation = t,
                    Interface = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}")
                })
            .Where(x => x.Interface != null);
            foreach (var type in typesWithInterfaces)
            {
                if (type.Interface != null && type.Implementation != null)
                {
                    services.AddScoped(type.Interface, type.Implementation);
                }
            }
        }

        /// <summary>
        /// Registers JWT Bearer authentication services.
        /// Configures dependency injection for authentication-related services.
        /// </summary>
        /// <param name="services">The service collection to which JWT services are added.</param>
        public void JwtBearerServices(IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }

        /// <summary>
        /// Configures common middleware components for the application pipeline.
        /// This includes handling errors, enabling Swagger in development, and 
        /// configuring HTTPS redirection, authentication, and routing.
        /// </summary>
        /// <param name="app">The application builder to configure middleware components.</param>
        /// <param name="env">The hosting environment that determines the app's behavior.</param>
        public void CommonConfigure(IApplicationBuilder app, IWebHostEnvironment env)
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
        public void CommonSwaggerConfigurations(IServiceCollection services)
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
    }
}