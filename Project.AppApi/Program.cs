using Core;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using MvcCore.Extension.Filter;
using Serilog;

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


//SerilLog  再Service中引用次NuGet包
//ThreadId需要引用专用的NuGet包
//const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


Log.Logger = new LoggerConfiguration()
    //.MinimumLevel.Debug() //设置日志记录器的最小级别为 Debug，即只记录 Debug、Information、Warning、Error 和 Fatal 级别的日志事件。
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)//对 Microsoft 命名空间下的所有日志事件进行重写，将最小级别设置为 Information，即只记录 Information、Warning、Error 和 Fatal 级别的日志事件。
    //.Enrich.FromLogContext() //启用日志上下文功能，自动获取当前线程和方法的一些信息，并添加到每个日志事件中。
    //.WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
    .WriteTo.File("logs/{Date}/app.txt"
        , rollingInterval: RollingInterval.Day,
         rollOnFileSizeLimit: true, // 当日志文件大小超过指定大小时自动滚动日志文件
         fileSizeLimitBytes: 1048576, // 日志文件最大大小为 1MB
          retainedFileCountLimit: 7, // 最多保留 7 天的日志文件
          outputTemplate: OUTPUT_TEMPLATE)
    .CreateLogger();

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

//注入
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
