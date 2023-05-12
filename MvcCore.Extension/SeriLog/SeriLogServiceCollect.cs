using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System;

namespace MvcCore.Extension.SeriLog
{
	public static class SeriLogServiceCollect
	{
		public static void AddSeriLog(this WebApplicationBuilder builder)
        {
            //SerilLog
            //const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} <{ThreadId}> [{Level:u3}] {Message:lj}{NewLine}{Exception}";
            const string OUTPUT_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                //.MinimumLevel.Debug() //设置日志记录器的最小级别为 Debug，即只记录 Debug、Information、Warning、Error 和 Fatal 级别的日志事件。
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Information)//对 Microsoft 命名空间下的所有日志事件进行重写，将最小级别设置为 Information，即只记录 Information、Warning、Error 和 Fatal 级别的日志事件。
                                                                              //.Enrich.FromLogContext() //启用日志上下文功能，自动获取当前线程和方法的一些信息，并添加到每个日志事件中。
                                                       //.WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
                .WriteTo.File("logs/app.txt"
                    , rollingInterval: RollingInterval.Day,
                      rollOnFileSizeLimit: true,
                      outputTemplate: OUTPUT_TEMPLATE)
                .CreateLogger();



            //Log.Information("Hello, Serilog!");
            //////Log.Error("err");
            //////Log.CloseAndFlush();



            builder.Host.UseSerilog(Log.Logger, dispose: true);
            //builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            // {
            //     loggerConfiguration
            //         .MinimumLevel.Verbose()
            //         .Enrich.FromLogContext()
            //         .WriteTo.Console();
            // }, writeToProviders: false);
        }
	}
}
