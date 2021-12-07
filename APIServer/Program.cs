
using Microsoft.AspNetCore.Hosting;
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
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddZLoggerConsole();    // Zlogger 사용
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // launchSettings.json의 ip를 활용하여 ip 주소를 변경할 수 있다.
                    // launchSettings.json 파일을 빌드 시 항상 복사(Always Copy)로 되어 있다.
                    //webBuilder.UseUrls(ServerAddress);
                });
        }
    }
}