using MvcCore.Extension.Filter;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Model.EnumModel;

namespace Project.AppApi.Controllers
{
	/// <summary>
	/// 控制器基類
	/// </summary>
	//[ApiFilter]
	//[ErrorFilter]
	//[AuthValidator]
	[ApiController]
	public class BaseController : ControllerBase
	{
		private string _userid { get => base.HttpContext.Request.Headers["UserId"].ToString(); }
		//private string _username { get => base.HttpContext.Request.Headers["UserName"].ToString(); }
		private string _token { get => base.HttpContext.Request.Headers["Token"].ToString(); }
		private string _language { get => base.HttpContext.Request.Headers["Language"].ToString(); }
		private string _marketId { get => base.HttpContext.Request.Headers["marketId"].ToString(); }

		/// <summary>
		/// 用戶id
		/// </summary>
		public int UserId { get => !string.IsNullOrEmpty(_userid) ? Convert.ToInt32(_userid) : 0; }

		///// <summary>
		///// 用戶名稱
		///// </summary>
		//public string UserName { get => _username; }

		/// <summary>
		/// token
		/// </summary>
		public string Token { get => _token; }

		/// <summary>
		/// 語言
		/// </summary>
		public LanguageEnum Language { get => !string.IsNullOrEmpty(_language) ? (LanguageEnum)Convert.ToInt32(_language) : LanguageEnum.CN; }

		/// <summary>
		/// 街市id
		/// </summary>
		public int MarketId { get => !string.IsNullOrEmpty(_marketId) ? Convert.ToInt32(_marketId) : 0; }

		/// <summary>
		/// 
		/// </summary>
		public BaseController()
		{
			SqlMapper.Aop.OnExecuting += Aop_OnExecuting;
		}

		/// <summary>
		/// sql执行前
		/// </summary>
		/// <param name="command"></param>
		private void Aop_OnExecuting(ref CommandDefinition command)
		{
			if (command.CommandText.Contains("SystemLog"))
			{
				command.IsUnifOfWork = true;
			}
		}
	}
}