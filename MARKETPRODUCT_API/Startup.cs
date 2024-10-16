using MARKETPRODUCT_API.Data;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MARKETPRODUCT_API.Services.IServices;
using MARKETPRODUCT_API.Services;
using MARKETPRODUCT_API.Messaging.MessageProducer;
using MARKETPRODUCT_API.MARKETUtilities;

namespace MARKETPRODUCT_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(MarketUtilities.CurrentVersion, new OpenApiInfo 
                { 
                    Title = MarketUtilities.PedidoAPI, 
                    Version = MarketUtilities.CurrentVersion,
                    Description = MarketUtilities.SwaggerDocDescription                                        
                });
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(MarketUtilities.DefaultConnection))
            );
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddSingleton<ProductMQProducer>();
            services.AddSingleton<OrderMQProducer>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(MarketUtilities.SwaggerUrlEndpoint, MarketUtilities.SwaggerNameEndpoint));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
