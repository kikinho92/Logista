using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Invoice.Service.Data;
using Log.Interface;
using Log.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Service.User
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Database connection inizialization.
            string connectionString = Configuration.GetConnectionString("LogistaDatabase");
            services.AddDbContext<InvoiceDbContext>(options => options.UseSqlServer(connectionString));
            
            // Database auto-migration
            DbContextOptionsBuilder<InvoiceDbContext> optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            InvoiceDbContext dbContext = new InvoiceDbContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Invoice.Service", Version = "v1" });
            });

            services.AddRateLimiter(_ => _  
                .AddConcurrencyLimiter(policyName: "Concurrency", options =>
                {
                    options.PermitLimit = 25;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 25;
                }));

                services.AddHttpClient("LogApi");
                services.AddSingleton<ILogApi>(s =>
                {
                    var clientFactory = s.GetRequiredService<IHttpClientFactory>();
                    var logCli = new LogCli(clientFactory, "http://127.0.0.1:6001");
                    return logCli;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice.Service v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
