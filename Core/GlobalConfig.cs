using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using Model.Table;
using ViewModel;
using ViewModel.App;

namespace Core
{
	public class GlobalConfig
	{
		/// <summary>
		/// 连接字符串
		/// </summary>
		public static string ConnectionString { get; set; }

		/// <summary>
		/// 第三方系统连接字符串
		/// </summary>
		public static string OtherSystemConnectionString { get; set; }

		/// <summary>
		/// 资源地址
		/// </summary>
		public static string ResourcesUrl { get; set; }

		/// <summary>
		/// 系统日志
		/// </summary>
		public static ILogService SystemLogService { get; set; }

		/// <summary>
		/// 缓存时间（分钟）
		/// </summary>
		public static int MemoryTime { get; set; } = 1 * 60;

		/// <summary>
		/// 判断小程序权限
		/// </summary>
		public static IMiniProgramCoreService MiniProgramCoreService { get; set; }

		/// <summary>
		/// 授权令牌验证
		/// </summary>
		public static IAuthorizationTokenCoreService AuthorizationTokenCoreService { get; set; }

		/// <summary>
		/// 获取缓存存储服务器地址
		/// </summary>
		public static string MemoryLockUrl { get; set; }
	}

	/// <summary>
	/// 日志操作类
	/// </summary>
	public interface ILogService
	{
		/// <summary>
		/// 写入数据库日志
		/// </summary>
		/// <param name="log">日志内容</param>
		/// <returns></returns>
		Task LogAdd(SystemLog log);

		/// <summary>
		/// 写入本地日志
		/// </summary>
		/// <param name="log">日志内容</param>
		/// <returns></returns>
		Task LocalLogAdd(SystemLog log);


		/// <summary>
		/// 写入本地和数据库日志
		/// </summary>
		/// <param name="log">日志内容</param>
		/// <returns></returns>
		Task LocalAndSqlLogAdd(SystemLog log);

		/// <summary>
		/// httpContext方式的时候一次写入日志
		/// </summary>
		/// <param name="httpContext">HTTP数据</param>
		/// <param name="instructions">操作说明</param>
		/// <param name="reqParameter">请求参数内容</param>
		/// <param name="resParameter">返回参数内容</param>
		/// <param name="type">类型 1：写数据库   2：写本地   3：同时写入数据库和本地</param>
		/// <returns></returns>
		Task AddLogByHttpContext(HttpContext httpContext, string instructions, string reqParameter, string resParameter, SystemLogType type);


		/// <summary>
		/// httpContext方式的时候一次写入日志
		/// </summary>
		/// <param name="httpContext">HTTP数据</param>
		/// <param name="instructions">操作说明</param>
		/// <param name="reqParameter">请求参数内容</param>
		/// <param name="resParameter">返回参数内容</param>
		/// <param name="type">类型 1：写数据库   2：写本地   3：同时写入数据库和本地</param>
		/// <param name="time"></param>
		/// <returns></returns>
		Task AddLogByHttpContext(HttpContext httpContext, string instructions, string reqParameter, string resParameter, SystemLogType type, string time);


		/// <summary>
		/// httpContext方式的时候一次写入日志  apiName不同
		/// </summary>
		/// <param name="httpContext">HTTP数据</param>
		/// <param name="aPIName">调用路径</param>
		/// <param name="instructions">操作说明</param>
		/// <param name="reqParameter">请求参数内容</param>
		/// <param name="resParameter">返回参数内容</param>
		/// <param name="type">类型 1：写数据库   2：写本地   3：同时写入数据库和本地</param>
		/// <param name="time"></param>
		/// <returns></returns>

		Task AddLogByHttpContext(HttpContext httpContext, string aPIName, string instructions, string reqParameter, string resParameter, SystemLogType type, string time);
	}


	/// <summary>
	/// 判断小程序权限
	/// </summary>
	public interface IMiniProgramCoreService
	{

	}


	/// <summary>
	/// 授权令牌验证
	/// </summary>
	public interface IAuthorizationTokenCoreService
	{
		/// <summary>
		/// 校验Token是否有效
		/// </summary>
		/// <param name="Token">传入Token</param>
		/// <returns></returns>
		ResultModel<AuthorizationTokenResDto> CheckAuthorizationToken(string Token);


		bool DeleteAuthorizationToken(int UserId);
	}
}
