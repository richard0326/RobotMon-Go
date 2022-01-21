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
//builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Config 파일 추가 등록하기
builder.Services.Configure<DbConfig>(Configuration.GetSection(nameof(DbConfig)));

// 서비스 등록
builder.Services.AddTransient<IAccountDb, AccountDb>();
builder.Services.AddTransient<IGameDb, GameDb>();
builder.Services.AddSingleton<IRedisDb, RedisDb>();
builder.Services.AddSingleton<IDataStorage, DataStorage>();
builder.Services.AddSingleton<IRankingManager, RankManager>();
builder.Services.AddControllers();

bool exists = System.IO.Directory.Exists("./log");
if (!exists)
    System.IO.Directory.CreateDirectory("./log");

File.Create("./log/logTest.pos").Dispose();
File.Create("./log/logTest.log").Dispose();
builder.Logging.AddZLoggerFile("C:/gitfolder/RobotMon-Go/APIServer/log/logTest.log");

// Zlogger 추가
builder.Logging.ClearProviders();
if (builder.Environment.EnvironmentName == Environments.Production)
{
    //bool exists = System.IO.Directory.Exists("./log");
    //if (!exists)
    //    System.IO.Directory.CreateDirectory("./log");

    //File.Create("./log/logTest.pos").Dispose();
    //File.Create("./log/logTest.log").Dispose();
    //builder.Logging.AddZLoggerFile("./log/logTest.log");
}

builder.Logging.AddZLoggerConsole(); // fluentd container

// app build
var app = builder.Build();

// 미들웨어 추가
app.UseMiddleware<AuthTokenCheckMiddleware>();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

// Redis Singleton 가져오기
var redisDb = app.Services.GetService<IRedisDb>();
var datastorage = app.Services.GetService<IDataStorage>();
var rankingManager = app.Services.GetService<IRankingManager>();
if (redisDb is null || datastorage is null || rankingManager is null)
{
    Console.WriteLine("singleton is null");
}
else
{
    // 사용자 초기화
    redisDb.Init(Configuration["SessionConfig:SessionCacheRedisIp"]);
    datastorage.Load(Configuration["DbConfig:GameConnStr"]);
    rankingManager.Init(Configuration["DbConfig:GameConnStr"], redisDb);

    // 현재 모드 확인하기
    Console.WriteLine($"Program env Mode : {app.Environment.EnvironmentName}");
    Console.WriteLine($"My appsetting Mode : {Configuration["Mode"]}");
    Console.WriteLine($"urls : {Configuration["urls"]}");

    // 앱 실행 - IP Port 설정
    app.Run(Configuration["urls"]);
}