using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using IService;
using Model.EnumModel;
using Core;

namespace MvcCore.Extension.Filter
{
	/// <summary>
	/// 異常攔截器
	/// </summary>
	public class ErrorFilterAttribute:  ExceptionFilterAttribute
	{
        /// <summary>
        ///日志
        /// </summary>
        private readonly ISystemLogService systemLogService;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="systemLogService">日志</param>
        public ErrorFilterAttribute(ISystemLogService systemLogService)
        {
            //日志
            this.systemLogService = systemLogService;
        }

        /// <summary>
        /// 異步獲取異常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
		{

            //写入日志
            systemLogService.LogAdd(SystemLogTypeEnum.Error, context.HttpContext, "异常", JsonConvert.SerializeObject(context.Exception), "", null, context.Exception);

			return base.OnExceptionAsync(context);
		}

		/// <summary>
		/// 返回異常信息
		/// </summary>
		/// <param name="context"></param>
		public override void OnException(ExceptionContext context)
		{
            context.ExceptionHandled = true;
			context.Result = new JsonResult(new
			{
                api_version="v1",
                code = "402",
				success = false,
				message = "Server internal error"
			});
        }
	}
}
