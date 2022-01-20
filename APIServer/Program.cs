using ApiServer;
using ApiServer.Options;
using ApiServer.Services;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

// 내가 만든 Config 파일 가져오기
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory());
    config.AddJsonFile("MyConfig.json",
                       optional: true,
                       reloadOnChange: true);

    if (args != null)
    {
        config.AddCommandLine(args);
    }
});

// Config 파일 가져오기
IConfiguration Configuration = builder.Configuration;

// 환경 세팅하기
if (Configuration["Environment"] == "Production")
{
    builder.Environment.EnvironmentName = Environments.Production;
}
else
{
    builder.Environment.EnvironmentName = Environments.Development;
}

// Config 파일 추가하기
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Config 파일 추가 등록하기
builder.Services.Configure<DbConfig>(Configuration.GetSection(nameof(DbConfig)));

// 서비스 등록
builder.Services.AddTransient<IAccountDb, AccountDb>();
builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorPages();

// Zlogger 추가
builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();

// app build
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("is Develope Mode");
}

// 미들웨어 추가
app.UseMiddleware<AuthTokenCheckMiddleware>();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// 사용자 초기화
RedisDB.Init(Configuration["SessionConfig:SessionCacheRedisIp"]);
DataStorage.Load(Configuration["DbConfig:GameConnStr"]);
RankManager.Init(Configuration["DbConfig:GameConnStr"]);

// 현재 모드 확인하기
Console.WriteLine($"Program env Mode : {app.Environment.EnvironmentName}");
Console.WriteLine($"My appsetting Mode : {Configuration["Mode"]}");

// 앱 실행 - IP Port 설정
app.Run(Configuration["IPPort"]);