//using Autofac;
//using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MvcCore.Extension.AutoFac
{
	public static class AutoFacServiceCollect
	{
		///// <summary>
		///// 添加依賴注入（autofac）
		///// </summary>
		///// <param name="services"></param>
		///// <param name="pathArr">隱射的dll列表</param>
		///// <returns></returns>
		//public static AutofacServiceProvider AddAutoFacs(this IServiceCollection services,string []pathArr)
		//{
		//	var basePath = AppContext.BaseDirectory;
		//	//Service.dll 注入，有對應接口
		//	var builder = new ContainerBuilder();
		//	foreach (var path in pathArr)
		//	{
		//		var servicesDllFile = Path.Combine(basePath, path);
		//		var assemblysServices = Assembly.LoadFile(servicesDllFile);
		//		builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces().InstancePerDependency();
		//	}
		//	//將services填充到Autofac容器生成器中
		//	builder.Populate(services);
		//	//使用已進行的組件登記創建新容器
		//	var ApplicationContainer = builder.Build();

		//	return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core內置DI容器
		//}
	}
}
