using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace MvcCore.Extension.Swagger
{
	public static class SwaggerGenServiceCollect
	{
		public static void AddSwaggerGens(this IServiceCollection services, string apiName, string[] pathArr = null)
		{
			var basePath = AppContext.BaseDirectory;
			//注冊Swagger生成器，定義一個和多個Swagger 文檔
			services.AddSwaggerGen(options =>
			{
				typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
				{
					options.SwaggerDoc(version, new OpenApiInfo
					{
						Version = version,
						Title = $"{apiName} 接口文檔",
						Description = $"{apiName} 接口文檔說明 " + version,
						License = new OpenApiLicense
						{
							Name = "API调用须知",
							Url = new Uri("http://www.changemall.cn/")
						}
					});
					//自定義配置文件路徑
					if (pathArr != null)
					{
						foreach (var item in pathArr)
						{
							var path = Path.Combine(basePath, item);
							options.IncludeXmlComments(path, true);
						}
					}
					// 按相對路徑排序
					options.OrderActionsBy(o => o.RelativePath);

                    #region 启用swagger验证功能
                    //添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称一致即可。
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"}
                             },
                            new string[] { }
                        }
                    });

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "CN:JWT授权(数据将在请求头中进行传输) 在下方输入Bearer {token} 即可，注意两者之间有空格;" +
						              "EN:JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        //Name = "Authorization",//jwt默认的参数名称
                        Name = "Authorization",//jwt默认的参数名称
                        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey,
                        BearerFormat = "JWT",
                        Scheme = "Bearer",
                    });
                    #endregion
                });
				var xmlPath = Path.Combine(basePath, $"{apiName}.xml");//xml文件名
				options.IncludeXmlComments(xmlPath, true);//默認的第二個參數是false，這個是controller的注釋，
			});
		}
	}
	/// <summary>
	/// Api接口版本 自定義
	/// </summary>
	public enum ApiVersions
	{
		/// <summary>
		/// V1 版本
		/// </summary>
		V1 = 1,
		///// <summary>
		///// V2 版本
		///// </summary>
		//V2 = 2,
	}
}
