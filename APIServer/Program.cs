
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
            //string portStr = Environment.GetEnvironmentVariable("Port");
            //string serverAddress = $"http://*:{portStr}";
            var builder = Host.CreateDefaultBuilder(args)
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
                    // launchSettings.json의 ip를 활용하여 ip 주소를 변경할 수 있다.
                    // launchSettings.json 파일을 빌드 시 항상 복사(Always Copy)로 되어 있다.
                    //webBuilder.UseUrls(serverAddress);
                });

            return builder;
        }
    }
}