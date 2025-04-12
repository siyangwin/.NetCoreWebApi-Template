using Microsoft.AspNetCore.Mvc;
using Dapper;
using Model.EnumModel;
using Microsoft.AspNetCore.Authorization;
using MvcCore.Extension.Filter;
using MvcCore.Extension.Auth;

namespace Project.AppApi.Controllers
{
	/// <summary>
	/// 控制器基類
	/// </summary>
	//[ApiController]
	[Authorize]  //加了这个，所有的API都会需要鉴权
    public class BaseController : ControllerBase
	{
		private string _userid { get => base.HttpContext.Request.Headers["UserId"].ToString(); }
		private string _token { get => base.HttpContext.Request.Headers["Token"].ToString(); }
		private string _language { get => base.HttpContext.Request.Headers["Language"].ToString(); }

		/// <summary>
		/// 用戶id
		/// </summary>
		public int UserId { get => !string.IsNullOrEmpty(_userid) ? Convert.ToInt32(_userid) : 0; }

		/// <summary>
		/// token
		/// </summary>
		public string Token { get => _token; }

		/// <summary>
		/// 語言
		/// </summary>
		public LanguageEnum Language { get => !string.IsNullOrEmpty(_language) ? (LanguageEnum)Convert.ToInt32(_language) : LanguageEnum.CN; }

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