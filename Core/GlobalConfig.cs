using Microsoft.AspNetCore.Http;
using Model.EnumModel;
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
		///  写入日志
		/// </summary>
		/// <param name="systemLogType">日志级别</param>
		/// <param name="httpContext">请求内容</param>
		/// <param name="instructions">操作说明</param>
		/// <param name="reqParameter">请求参数内容</param>
		/// <param name="resParameter">返回参数内容</param>
		/// <param name="time">耗费时间</param>
		/// <param name="ex">错误级别需要</param>
		/// <returns></returns>
		Task LogAdd(SystemLogTypeEnum systemLogType, HttpContext httpContext, string instructions, string reqParameter, string resParameter, string? time, Exception? ex);
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
		ResultModel<AuthorizationResDto> CheckAuthorizationToken(string Token);


		bool DeleteAuthorizationToken(int UserId);
	}
}
