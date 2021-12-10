using ApiServer.Controllers;
using ApiServer.Options;
using ApiServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiServer
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
            services.Configure<DbConfig>(Configuration.GetSection(nameof(DbConfig)));

            RedisDB.Init(Configuration["SessionConfig:SessionCacheRedisIp"]);
            
            services.AddTransient<IAccountDb, AccountDb>();
            services.AddTransient<IGameDb, GameDb>();

            services.AddControllers();
            
            DataStorage.Load(Configuration["DbConfig:GameConnStr"]);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}