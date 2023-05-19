using Core;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using MvcCore.Extension.Filter;
using Serilog;
using Azure.Core;
using Serilog.Events;
using Model.Table;
using Serilog.Sinks.MSSqlServer;
using static Serilog.Sinks.MSSqlServer.ColumnOptions;
using System.Collections.ObjectModel;
using System.Data;

var ApiName = "Project.AppApi";

var builder = WebApplication.CreateBuilder(args);

//获取连接字符串
GlobalConfig.ConnectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SqlServer");

// Add services to the container.

builder.Services.AddControllers();

//是否开启Swagger
var getconfig = builder.Configuration.GetValue<bool>("ConfigSettings:SwaggerEnable");
//Swagger
if (getconfig)
{
    builder.Services.AddSwaggerGens(ApiName, new string[] { "ViewModel.xml" });
}


//注入DB链接
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<ISystemLogService, SystemLogService>();


// 将接口请求拦截器和错误拦截器 注册为全局过滤器
builder.Services.AddMvc(options =>
{
    //接口请求拦截器
    options.Filters.Add(typeof(ApiFilterAttribute));
    //错误拦截器
    options.Filters.Add(typeof(ErrorFilterAttribute));
});


var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn
            {ColumnName = "EnvironmentUserName", PropertyName = "UserName", DataType = SqlDbType.NVarChar, DataLength = 64},

        new SqlColumn
            {ColumnName = "UserId", DataType = SqlDbType.BigInt, NonClusteredIndex = true},

        new SqlColumn
            {ColumnName = "RequestUri", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = false},
    }
};


//SerilLog  再Service中引用次NuGet包
//ThreadId需要引用专用的NuGet包
//const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


//输出日志等级,可以禁止输出 ASP.NET Core 应用程序启动时记录的，并且是通过默认的日志记录器输出的（Information）
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() //设置日志记录器的最小级别为 Debug，即只记录 Debug、Information、Warning、Error 和 Fatal 级别的日志事件。
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//对 Microsoft 命名空间下的所有日志事件进行重写，将最小级别设置为 Information，即只记录 Information、Warning、Error 和 Fatal 级别的日志事件。
                                                              //.ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "PRODUCTION"}.json", optional: true).Build())
    .Enrich.FromLogContext() //启用日志上下文功能，自动获取当前线程和方法的一些信息，并添加到每个日志事件中。
    .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
    //.WriteTo.File("logs/app.txt"
    //    , rollingInterval: RollingInterval.Day,
    //     rollOnFileSizeLimit: true, // 当日志文件大小超过指定大小时自动滚动日志文件
    //     fileSizeLimitBytes: 1048576, // 日志文件最大大小为 1MB
    //      retainedFileCountLimit: 7, // 最多保留 7 天的日志文件
    //      outputTemplate: OUTPUT_TEMPLATE)
    .WriteTo.MSSqlServer("server=disk.risinguptech.com,36832;database=Project;user=risingup_admin;password=risingup2023;max pool size=300", "dbo.[test]", columnOptions: columnOptions, autoCreateSqlTable: true) //restrictedToMinimumLevel: LogEventLevel.Information,
    .CreateLogger();

//测试日志输出
Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });
Log.Error("{EnvironmentUserName}{UserId}{RequestUri}", 1, 2, 3);
Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);


//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
//    .ReadFrom.Configuration(new ConfigurationBuilder()
//    .AddJsonFile("appsettings.json")
//    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
//    optional: true).Build())
//    .Enrich.FromLogContext()
//    .WriteTo.Async(c => c.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/log/", "log")
//    , rollingInterval: RollingInterval.Day)).WriteTo.Async(c => c.Console())
//    .CreateLogger();




///写入数据库需要引用专用的NuGet包
//string connectionString = "Data Source=localhost;Initial Catalog=Logs;Integrated Security=True";

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.MSSqlServer(
//        connectionString,
//        sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" },
//        columnOptions: new ColumnOptions
//        {
//            AdditionalDataColumns = new Collection<DataColumn>
//            {
//    new DataColumn { DataType = typeof(string), ColumnName = "Application" }
//            }
//        })
//    .CreateLogger();

//写入Log案例
//Log.Information("Hello, Serilog!");
//Log.Error("err");
///Log.CloseAndFlush();

//注入 替换默认日志
builder.Host.UseSerilog(Log.Logger, dispose: true);

builder.Services.AddControllersWithViews();


//GlobalConfig方法注入
//注入配置日志
//GlobalConfig.SystemLogService()

// 批量注册服务
//builder.Services.AddAutoFacs(new string[] { "Service.dll" });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

//是否开启Swagger
if (getconfig)
{
    app.UseSwaggers(ApiName);
}

app.UseAuthorization();

app.MapControllers();

app.Run();