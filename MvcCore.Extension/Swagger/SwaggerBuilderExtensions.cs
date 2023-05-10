using Microsoft.AspNetCore.Builder;

namespace MvcCore.Extension.Swagger
{
	public static class SwaggerBuilderExtensions
	{

		public static void UseSwaggers(this IApplicationBuilder app, string apiName)
		{
			//啟用中間件服務生成Swagger作為JSON終結點
			app.UseSwagger();
			//啟用中間件服務對swagger-ui，指定Swagger JSON終結點
			app.UseSwaggerUI(c =>
			{
				//根據版本名稱倒序 遍歷展示
				typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
				{
					c.SwaggerEndpoint($"./swagger/{version}/swagger.json", $"{apiName} {version}");
				});
				// 將swagger首頁，設置成我們自定義的頁面，記得這個字符串的寫法：解決方案名.index.html
				// c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Fresh.AppAPI.index.html");//這里是配合MiniProfiler進行性能監控的。
				c.RoutePrefix = ""; //路徑配置，設置為空，表示直接在根域名（localhost:8001）訪問該文件,注意localhost:8001/swagger是訪問不到的，去launchSettings.json把launchUrl去掉
			});
		}
	}
}
