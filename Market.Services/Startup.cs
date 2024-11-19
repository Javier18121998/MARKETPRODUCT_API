using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Market.Market.Models;
using MARKETPRODUCT_API.MARKETUtilities;
using Microsoft.AspNetCore.Mvc;
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
            services.AddControllers();
            services.AddHttpContextAccessor();

            #region JWTConfiguration Auth
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));
            services.ConfigureJwtSettings(Configuration);
            #endregion

            // Configura autenticación con JWT
            var jwtKey = Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException(nameof(jwtKey), "JWT Key is not configured properly in appsettings.");
            }

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
            CommonSwaggerConfigurations(services);

            services.AddDbContext<MarketDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MarketUtilities.DefaultConnection))
            );

            //Adding JWTBEarer services
            #region JWTConfiguration Auth
            JwtBearerServices(services);
            #endregion

            // Registro de servicios de aplicación
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

            // CORS: habilitar solo si es necesario
            // app.UseCors("MiPoliticaCors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
