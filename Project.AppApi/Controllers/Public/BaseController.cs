using Microsoft.AspNetCore.Mvc;
using Dapper;
using Model.EnumModel;
using Microsoft.AspNetCore.Authorization;
using MvcCore.Extension.Filter;
using MvcCore.Extension.Auth;
using Core;

namespace Project.AppApi.Controllers
{
	/// <summary>
	/// 控制器基類
	/// </summary>
	//[ApiController]
	[Authorize]  //加了这个，所有的API都会需要鉴权
    public class BaseController : ControllerBase
	{
		private string _token { get => base.HttpContext.Request.Headers["Token"].ToString(); }
		private string _language { get => base.HttpContext.Request.Headers["Language"].ToString(); }

		/// <summary>
		/// 用戶id（从 JWT claims 中读取，客户端无法伪造；未认证时为 0）
		/// </summary>
		public int UserId { get => int.TryParse(User?.FindFirst("UserId")?.Value, out var id) ? id : 0; }

		/// <summary>
		/// token
		/// </summary>
		public string Token { get => _token; }

		/// <summary>
		/// 語言枚举（内部使用）
		/// </summary>
		public LanguageEnum Language { get => !string.IsNullOrEmpty(_language) ? LanguageHelper.Parse(_language) : LanguageHelper.DefaultLanguage; }

		/// <summary>
		/// 当前语言代码（zh/en/ja），用于 Lang.Get() 和数据库翻译表查询
		/// </summary>
		public string LanguageCode { get => LanguageHelper.ToCode(Language); }

		/// <summary>
		/// 静态构造函数：仅订阅一次 SqlMapper.Aop 事件，避免实例构造函数重复订阅导致的内存泄漏
		/// </summary>
		static BaseController()
		{
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
		}

		/// <summary>
		/// sql执行前
		/// </summary>
		/// <param name="command"></param>
		private static void Aop_OnExecuting(ref CommandDefinition command)
		{
			if (command.CommandText.Contains("SystemLog"))
			{
				command.IsUnifOfWork = true;
			}
		}
	}
}