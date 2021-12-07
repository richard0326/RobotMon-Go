using ApiServer.Controllers;
using ApiServer.Options;
using ApiServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiServer
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
            // TODO Redis 연결 부분 추가 예정
            //services.Configure<SessionConfig>(Configuration.GetSection(nameof(SessionConfig)));
            services.Configure<AccountDbConfig>(Configuration.GetSection(nameof(AccountDbConfig)));
            
            // Dapper 를 통한 Mysql DB 서비스를 등록한다
            services.AddTransient<IAccountDb, AccountDb>();
            
            services.AddLogging();  // logger 등록
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}