using CommandLine;

namespace ApiServer.Services
{
    public class CommandLineOption
    {
        [Option("port", Required = true, HelpText = "Port")]
        public int Port { get; set; }                       // 포트 번호
    }
}