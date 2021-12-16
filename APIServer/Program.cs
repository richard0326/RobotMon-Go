
using System;
using ApiServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace ApiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    /*
                    // Development, Staging, Production 중 하나로 설정합시다.
                    아직 테스트 중.
                    // launchSettings : "ASPNETCORE_ENVIRONMENT": "Development" 설정 값이
                    // context.HostingEnvironment.EnvironmentName의 값이 된다.
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddJsonFile("appsettings.json");
                    configurationBuilder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                    */
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    //logging.SetMinimumLevel(LogLevel.Debug); // appsettings.json 파일에 의해서 Information으로 오버라이드됨.
                    //logging.AddZLoggerFile("filename.txt");

                    logging.AddZLoggerConsole();
                    /*
                    logging.AddZLoggerConsole(options =>
                    {
                        // 구조화된 형식으로 로그 출력
                        options.EnableStructuredLogging = true;
                    }); // Zlogger 사용
                    */
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            return builder;
        }
    }
}
