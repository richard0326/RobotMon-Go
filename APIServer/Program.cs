
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace APIServer
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    // lamda
                    webBuilder => { webBuilder.UseStartup<Startup>(); }
                );
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}