using Market.AuthorizationController.AuthModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.AuthorizationController.AuthConfigurations
{
    public static class JwtSettingsConfiguration
    {
        public static void ConfigureJwtSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        }
    }
}
