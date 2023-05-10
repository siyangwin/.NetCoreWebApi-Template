using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Model.Table;
using Core;
using System.Diagnostics;

namespace MvcCore.Extension.Filter
{
    /// <summary>
    /// 請求響應攔截器
    /// </summary>
    public class ApiFilterAttribute : Attribute, IActionFilter, IAsyncResourceFilter
    {
        /// <summary>
        /// 執行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
             Console.Out.WriteLineAsync("OnActionExecuting");

            //驗證參數
            if (!context.ModelState.IsValid)
            {
                string message = "";
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        message += error.ErrorMessage + "|";
                    }
                }
                message = message.TrimEnd(new char[] { ' ', '|' });
                throw new Exception(message);
            }
        }

        /// <summary>
        /// 執行后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.Out.WriteLineAsync("OnActionExecuted");
        }


        /// <summary>
        /// 請求Api 資源時
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await Console.Out.WriteLineAsync("OnResourceExecutionAsync - Before");

            List<object> apiRequest = new List<object>();
            //記錄參數日志
            //var logData = new
            //{
            //    RequestQurey = context.HttpContext.Request.QueryString.ToString(),
            //    RequestContextType = context.HttpContext.Request.ContentType,
            //    RequestHost = context.HttpContext.Request.Host.ToString(),
            //    RequestPath = context.HttpContext.Request.Path,
            //    RequestLocalIp = (context.HttpContext.Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + context.HttpContext.Request.HttpContext.Connection.LocalPort),
            //    RequestRemoteIp = (context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + ":" + context.HttpContext.Request.HttpContext.Connection.RemotePort),
            //    RequestParam = GetParamString(context.HttpContext)
            //};
            var logData = new
            {
             
            };
            apiRequest.Add(new
            {
                Title = "请求信息",
                Data = logData
            });

            //记录当前时间
            DateTime ReqTime = DateTime.Now;

            object responseValue = null;
            string responseJson = string.Empty;
            // 執行前
            try
            {
                var executedContext = await next.Invoke();
                await Console.Out.WriteLineAsync("OnResourceExecutionAsync - After");
                responseValue = executedContext.Result;
                responseJson = JsonConvert.SerializeObject((responseValue as ObjectResult) is null ? responseValue : (responseValue as ObjectResult).Value);
                apiRequest.Add(new
                {
                    Title = "返回信息",
                    Data = responseJson
                });
            }
            catch (Exception ex)
            {
                apiRequest.Add(new
                {
                    Title = "返回信息",
                    Data = "异常"
                });
                //LogExtension.Error(ex);
            }
            finally
            {

                string Time = (DateTime.Now - ReqTime).ToString();

                //LogExtension.Debug(JsonConvert.SerializeObject(apiRequest));
                string ip = context.HttpContext.Connection.RemoteIpAddress.ToString();

                ////写入日志
                //await GlobalConfig.SystemLogService.LocalAndSqlLogAdd(new SystemLog { Guid = context.HttpContext.Request.Headers["GuidPwd"].ToString(), ClientType = context.HttpContext.Request.Headers["ClientType"].ToString(), APIName = context.HttpContext.Request.Path, UserId = context.HttpContext.Request.Headers["UserId"].ToString() == "" ? 0 : Convert.ToInt32(context.HttpContext.Request.Headers["UserId"]), DeviceId = context.HttpContext.Request.Headers["DeviceId"].ToString() == "" ? "0" : context.HttpContext.Request.Headers["DeviceId"].ToString(), Instructions = "请求-返回", ReqParameter = JsonConvert.SerializeObject(logData), ResParameter = responseJson, Time = Time,IP= ip });
            }
        }

        /// <summary>
        ///  獲取參數字符串
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetParamString(HttpContext context)
        {
            StringBuilder builder = new StringBuilder();
            if (context.Request.HasFormContentType && context.Request.Form != null)
                foreach (var key in context.Request.Form.Keys)
                {
                    builder.Append(key + ":" + context.Request.Form[key].ToString() + "|");
                }
            if (context.Request.Query != null)
                foreach (var key in context.Request.Query.Keys)
                {
                    builder.Append(key + ":" + context.Request.Query[key].ToString() + "|");
                }
            //验证是否存在Raw参数
            if (context.Request.Body.CanRead)
            {
                var memery = new System.IO.MemoryStream();
                context.Request.Body.CopyTo(memery);
                memery.Position = 0;
                //记录head
                string header = JsonConvert.SerializeObject(context.Request.Headers);
                //记录参数内容
                string content = new StreamReader(memery, UTF8Encoding.UTF8).ReadToEnd();
                builder.Append(JsonConvert.SerializeObject(new { header, content }));
                builder.Append(Environment.NewLine);
                memery.Position = 0;
                context.Request.Body = memery;
            }
            return builder.ToString();
        }
    }
}
