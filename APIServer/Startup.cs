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
            //services.Configure<SessionConfig>(Configuration.GetSection(nameof(SessionConfig)));
            services.Configure<CommonDbConfig>(Configuration.GetSection("CommonDbConfig"));
            
            // Dapper 를 통한 Mysql DB 서비스를 등록한다
            services.AddTransient<ICommonDb, CommonDb>();
            
            services.AddLogging();  // logger 등록
            var builder = services.AddControllers();
            builder.AddApplicationPart(typeof(LoginController).Assembly);
            builder.AddApplicationPart(typeof(CreateAccountController).Assembly);
            builder.AddApplicationPart(typeof(UserInfoController).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}