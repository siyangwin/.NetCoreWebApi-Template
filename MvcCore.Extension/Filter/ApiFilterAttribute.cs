using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Model.Table;
using Core;
using Microsoft.Extensions.Logging;
using IService;
using Model.EnumModel;
using Azure.Core;

namespace MvcCore.Extension.Filter
{
    /// <summary>
    /// 請求響應攔截器
    /// </summary>
    public class ApiFilterAttribute : Attribute, IActionFilter, IAsyncResourceFilter
    {
        /// <summary>
        ///日志
        /// </summary>
        private readonly ISystemLogService systemLogService;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="systemLogService">日志</param>
        public ApiFilterAttribute(ISystemLogService systemLogService)
        {
            //日志
            this.systemLogService = systemLogService;
        }


        /// <summary>
        /// 執行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
             //Console.Out.WriteLineAsync("OnActionExecuting");

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
            //Console.Out.WriteLineAsync("OnActionExecuted");
        }


        /// <summary>
        /// 請求Api 資源時
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //await Console.Out.WriteLineAsync("OnResourceExecutionAsync - Before");

            List<object> apiRequest = new List<object>();

            //记录当前时间
            DateTime ReqTime = DateTime.Now;

            //記錄參數日志
            var logData = new
            {
                RequestQurey = context.HttpContext.Request.QueryString.ToString(),
                RequestContextType = context.HttpContext.Request.ContentType,
                RequestHost = context.HttpContext.Request.Host.ToString(),
                RequestPath = context.HttpContext.Request.Path,
                RequestLocalIp = (context.HttpContext.Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + context.HttpContext.Request.HttpContext.Connection.LocalPort),
                RequestRemoteIp = (context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString() + ":" + context.HttpContext.Request.HttpContext.Connection.RemotePort),
                RequestParam = GetParamString(context.HttpContext)
            };


            //var logData = new
            //{
            //    kjjj = 1
            //};

            string responseJson = string.Empty;

            // 執行前
            try
            {
                apiRequest.Add(new{Title = "请求信息",Data = logData});

                object responseValue = null;

                var executedContext = await next.Invoke();
                //await Console.Out.WriteLineAsync("OnResourceExecutionAsync - After");

                responseValue = executedContext.Result;
                responseJson = JsonConvert.SerializeObject((responseValue as ObjectResult) is null ? responseValue : (responseValue as ObjectResult).Value);

                apiRequest.Add(new{Title = "返回信息",Data = responseJson});
            }
            catch (Exception ex)
            {
                //获取这个API执行到这里的时间
                string Time = (DateTime.Now - ReqTime).ToString();

                apiRequest.Add(new{Title = "返回信息",Data = "异常"});

                //写入日志
                await systemLogService.LogAdd(SystemLogTypeEnum.Information, context.HttpContext, "异常", JsonConvert.SerializeObject(logData), responseJson, Time, ex);
            }
            finally
            {
                //获取这个API执行的时间
                string Time = (DateTime.Now - ReqTime).ToString();

                //写入日志
                await systemLogService.LogAdd(SystemLogTypeEnum.Information, context.HttpContext, "请求-返回", JsonConvert.SerializeObject(logData), responseJson, Time,null);
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
            {
                foreach (var key in context.Request.Form.Keys)
                {
                    builder.Append(key + ":" + context.Request.Form[key].ToString() + "|");
                }
            }
            if (context.Request.Query != null)
            {
                foreach (var key in context.Request.Query.Keys)
                {
                    builder.Append(key + ":" + context.Request.Query[key].ToString() + "|");
                }
            }

            ////验证是否存在Raw参数
            //if (context.Request.Body.CanRead)
            //{
            //    var memery = new System.IO.MemoryStream();
            //    context.Request.Body.CopyTo(memery);
            //    memery.Position = 0;
            //    //记录head
            //    string header = JsonConvert.SerializeObject(context.Request.Headers);
            //    //记录参数内容
            //    string content = new StreamReader(memery, UTF8Encoding.UTF8).ReadToEnd();
            //    builder.Append(JsonConvert.SerializeObject(new { header, content }));
            //    builder.Append(Environment.NewLine);
            //    memery.Position = 0;
            //    context.Request.Body = memery;
            //}

         
            ////检查 HTTP 请求体是否可读
            //if (context.Request.Body.CanRead)
            //{
            //    //记录head
            //    string header = JsonConvert.SerializeObject(context.Request.Headers);

            //    //记录参数内容
            //    var content = new StreamReader(context.Request.Body, UTF8Encoding.UTF8).ReadToEndAsync().Result;
            //    //context.Request.Body.Dispose();
            //    builder.Append(JsonConvert.SerializeObject(new { header, content }));
            //    //builder.Append(Environment.NewLine);
            //    context.Request.Body.Seek(0, SeekOrigin.Begin);
            //}


            var requestBody = string.Empty;
            if (context.Request.Body.CanRead)
            {
                // 记录请求头部信息
                var headers = JsonConvert.SerializeObject(context.Request.Headers);

                // 读取请求体参数内容
                var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
                requestBody = reader.ReadToEndAsync().Result;

                // 重置请求流位置
                if (context.Request.Body.CanSeek)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }

                // 记录请求内容
                var requestJson = JsonConvert.SerializeObject(new { Headers = headers, Body = requestBody });
                builder.Append(requestJson);
            }

            ////检查 HTTP 请求体是否可读
            //if (context.Request.Body.CanRead)
            //{
            //     //记录head
            //    string header = JsonConvert.SerializeObject(context.Request.Headers);
            //    //记录参数内容
            //    var content = new StreamReader(context.Request.Body, UTF8Encoding.UTF8).ReadToEndAsync().Result;
            //    //context.Request.Body.Dispose();
            //    builder.Append(JsonConvert.SerializeObject(new { header, content }));
            //    builder.Append(Environment.NewLine);
            //}
            return builder.ToString();
        }
    }
}
