using System.Text;
using Market.AuthorizationController.AuthServices;
using MARKETPRODUCT_API.MARKETUtilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MARKETPRODUCT_API
{
    public static class JwtAuthenticationExtensions
    {
        /// <summary>
        /// Configures JWT authentication for the application.
        /// </summary>
        /// <param name="services">The service collection to which authentication services are added.</param>
        /// <param name="configuration">The application configuration containing JWT settings.</param>
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"];
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
        }

        /// <summary>
        /// Registers JWT Bearer authentication services.
        /// Configures dependency injection for authentication-related services.
        /// </summary>
        /// <param name="services">The service collection to which JWT services are added.</param>
        public static void JwtBearerServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }

        /// <summary>
        /// Configures Cross-Origin Resource Sharing (CORS) policies for the application,
        /// allowing requests from specific domains with support for any HTTP headers
        /// and methods.
        /// </summary>
        /// <param name="services">
        /// The service collection where the CORS configuration is registered.
        /// </param>
        public static void CommonCorsConfigurations(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyPoliticalCors", policy =>
                    {
                        policy.WithOrigins(MarketUtilities.DomainPhaser, MarketUtilities.DomainStable)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                );
            });
        }
    }
}