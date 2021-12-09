
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
        private static string? s_serverAddress;
        
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                // rider에서 WebAPI 개발할때, args 전달이 자꾸 무시되는 현상 발생함.
                Console.WriteLine("No args!");
                s_serverAddress = "http://*:5000";
            }
            else
            {
                // Console에서 직접 인자를 전달하는 방식은 사용 가능한 것을 확인.
                var serverOption = ParseCommandLine(args);
                if(serverOption == null)
                {
                    return;
                }

                s_serverAddress = $"http://*:{serverOption.Port}";
                Console.WriteLine($"Set Address {serverOption.Port}");
            }
            
            CreateHostBuilder(args).Build().Run();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
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
                    webBuilder.UseUrls(s_serverAddress);
                });

            return builder;
        }
        
        static CommandLineOption ParseCommandLine(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<CommandLineOption>(args) as CommandLine.Parsed<CommandLineOption>;

            if (result == null)
            {
                System.Console.WriteLine("Failed Command Line");
                return null;
            }

            return result.Value;
        }
    }
}
