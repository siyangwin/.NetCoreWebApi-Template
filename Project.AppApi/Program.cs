using Core;
using MvcCore.Extension.AutoFac;
using MvcCore.Extension.Swagger;
using IService;
using Service;
using Autofac.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcCore.Extension.Filter;

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
//builder.Services.AddScoped<ILogService, SystemLogService>();

// 将 MyActionFilter 注册为全局过滤器
builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(ApiFilterAttribute));
});

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
