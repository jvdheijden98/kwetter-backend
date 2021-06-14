using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimelineService.DAL.Contexts;
using TimelineService.Messaging;

namespace TimelineService
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
            setRabbitHost();
            //Task.Run(() => RabbitSubscriber.OpenChannel());
        }

        public void setRabbitHost()
        {
            Environment.SetEnvironmentVariable("RabbitHost", _env.IsProduction() ? "RabbitMQ" : "localhost");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region EntityFramework Database
            if (_env.IsProduction())
            {
                services.AddDbContext<TimelineDBContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DockerTimelineDb")));
            }
            else
            {
                services.AddDbContext<TimelineDBContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DebugTimelineDb")));
            }

            services.AddDatabaseDeveloperPageExceptionFilter();
            #endregion

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader();
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
