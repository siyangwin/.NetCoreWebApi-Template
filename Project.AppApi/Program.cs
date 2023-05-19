using Core;
using MvcCore.Extension.Swagger;
using MvcCore.Extension;
using IService;
using Service;
using MvcCore.Extension.Filter;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using Model.EnumModel;
using System.Configuration;

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


//SerilLog再Service中引用次NuGet包
//ThreadId需要引用专用的NuGet包
//const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";


var columnOpts = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        //唯一编号
        new SqlColumn{ColumnName = "Guid", PropertyName = "Guid", DataType = SqlDbType.NVarChar, DataLength = 32, AllowNull = false},
        //請求客戶類型 APP CMS
        new SqlColumn{ColumnName = "ClientType", DataType = SqlDbType.NVarChar, DataLength = 10, AllowNull = false},
        //API名称
        new SqlColumn{ColumnName = "APIName", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = false},
        //请求方式 POST GET等
        new SqlColumn{ColumnName = "Request", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = false},
        //用户编号
        new SqlColumn{ColumnName = "UserId", DataType = SqlDbType.Int, AllowNull = false},
        //设备唯一编号,如果有，默认0
        new SqlColumn{ColumnName = "DeviceId", DataType = SqlDbType.Int, AllowNull = true},
        //操作说明
        new SqlColumn{ColumnName = "Instructions", DataType = SqlDbType.NVarChar, DataLength = 200, AllowNull = false},
        //请求参数内容
        new SqlColumn{ColumnName = "ReqParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = false},
        //返回参数内容
        new SqlColumn{ColumnName = "ResParameter", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true},
        //耗费时间
        new SqlColumn{ColumnName = "Time", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
        //访问用户IP
        new SqlColumn{ColumnName = "IP", DataType = SqlDbType.NVarChar, DataLength = 20, AllowNull = true},
         //服务器名称(负载均衡记录)
        new SqlColumn{ColumnName = "Server", DataType = SqlDbType.NVarChar, DataLength = 50, AllowNull = false}
    }
};

columnOpts.Store.Remove(StandardColumn.Message);
columnOpts.Store.Remove(StandardColumn.Properties);
columnOpts.Store.Remove(StandardColumn.MessageTemplate);
//columnOpts.Store.Add(StandardColumn.LogEvent);
//columnOpts.LogEvent.DataLength = 2048;
//columnOpts.PrimaryKey = columnOpts.TimeStamp;
columnOpts.TimeStamp.NonClusteredIndex = true; //设置为非聚类索引

//BatchPeriod
string interval = "00:00:05"; //表示5秒
TimeSpan ts;
TimeSpan.TryParse(interval, out ts);

//输出日志等级, 可以禁止输出 ASP.NET Core 应用程序启动时记录的，并且是通过默认的日志记录器输出的（Information）
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() //设置日志记录器的最小级别为 Debug，即只记录 Debug、Information、Warning、Error 和 Fatal 级别的日志事件。
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//对 Microsoft 命名空间下的所有日志事件进行重写，将最小级别设置为 Information，即只记录 Information、Warning、Error 和 Fatal 级别的日志事件。
    //.ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "PRODUCTION"}.json", optional: true).Build())
    .Enrich.FromLogContext() //启用日志上下文功能，自动获取当前线程和方法的一些信息，并添加到每个日志事件中。
    .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
    .WriteTo.File("logs/app.txt"
        , rollingInterval: RollingInterval.Day,
         rollOnFileSizeLimit: true, // 当日志文件大小超过指定大小时自动滚动日志文件
         fileSizeLimitBytes: 1048576, // 日志文件最大大小为 1MB
          retainedFileCountLimit: 7, // 最多保留 7 天的日志文件
          outputTemplate: OUTPUT_TEMPLATE)
    .AuditTo.MSSqlServer(
        connectionString: GlobalConfig.ConnectionString,
        sinkOptions: new MSSqlServerSinkOptions { TableName = "SystemLog", SchemaName = "dbo", AutoCreateSqlTable = true, BatchPeriod=ts,BatchPostingLimit = 50 },
        columnOptions: columnOpts)
    .CreateLogger();

#region SerilLog写入数据库
//WriteTo生效 AuditTo不生效
//BatchPostingLimit: 用于设置批处理日志事件的数量限制。默认值为 50，即当累积了 50 条日志事件时就会将它们作为一个批次进行写入数据库。这个选项可以帮助优化性能，因为一次提交大量的日志事件比一次提交少量的日志事件效率更高。
//BatchPeriod: 用于设置批处理的时间间隔。默认值为 2 秒，即每隔 2 秒就会将所有已缓存的日志事件作为一个批次进行写入数据库。这个选项可以保证在一定的时间间隔内一定会向数据库提交日志事件，以保证数据的实时性和完整性。

//Log.Logger = new LoggerConfiguration()
//    .WriteTo
//    .MSSqlServer(
//        connectionString: GlobalConfig.ConnectionString,
//        sinkOptions: new MSSqlServerSinkOptions { TableName = "testnew" ,SchemaName="dbo",AutoCreateSqlTable=true,BatchPostingLimit=1},
//        columnOptions: columnOpts)
//    .CreateLogger();


//测试日志输出
//Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
//Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });
//Log.Error("{UserName}{UserId}{RequestUri}", 1, 2, 3);
//Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
#endregion

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

app.Use(async (context, next) =>
{
    //表示此API是什么端
    context.Request.Headers.Add("ClientType", "APP");

    //注入Guid每次请求唯一编码
    context.Request.Headers.Add("Guid", Guid.NewGuid().ToString("N"));

    //获取默认语言
    string language = context.QueryOrHeaders("language");
    if (string.IsNullOrEmpty(language))
    { 
        language = ((int)LanguageEnum.CN).ToString(); 
    }
    context.SetHeaders("Language", language);

    //token
    context.SetHeaders("Token", context.QueryOrHeaders("Token"));

    //街市id
    string marketId = context.QueryOrHeaders("marketId");
    if (string.IsNullOrEmpty(marketId))
    {
        //默认MarkertId
        marketId = "1";
    }
    context.SetHeaders("MarketId", marketId);

    await next();
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();