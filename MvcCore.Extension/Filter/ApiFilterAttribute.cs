 using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Core;
using Microsoft.Extensions.Logging;
using IService;
using Model.EnumModel;
using Azure.Core;
using System.ComponentModel.DataAnnotations;

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
                //throw new ValidationException(message); // 使用自定义的 ValidationException
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
        public async Task<string> GetParamString(HttpContext context)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
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


                // 验证是否存在 Raw 参数
                SemaphoreSlim semaphore = new SemaphoreSlim(1);
                if (context.Request.Body.CanRead && context.Request.Body is not null) //&& context.Request.ContentLength > 0
                {
                    MemoryStream memory;
                    await semaphore.WaitAsync();
                    try
                    {
                        memory = new MemoryStream();
                        await context.Request.Body.CopyToAsync(memory);
                        memory.Position = 0;
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                    // 记录 header
                    string header = JsonConvert.SerializeObject(context.Request.Headers);
                    // 记录参数内容
                    string content = new StreamReader(memory, Encoding.UTF8).ReadToEnd();
                    builder.Append(JsonConvert.SerializeObject(new { header, content }));
                    builder.Append(Environment.NewLine);

                    // 恢复流位置
                    memory.Position = 0;
                    //context.Request.Body.Position = 0;
                    if (context.Request.Body.CanSeek)
                    {
                        context.Request.Body.Position = 0;
                    }

                    // 创建新的内存流对象并拷贝内容
                    var newMemory = new MemoryStream();
                    await memory.CopyToAsync(newMemory);
                    newMemory.Position = 0;
                    context.Request.Body = newMemory;
                }

            }
            catch (Exception ex)
            {
                //发送错误
                //写入日志
               await systemLogService.LogAdd(SystemLogTypeEnum.Information, context, "异常", JsonConvert.SerializeObject(ex), "", null, ex);
            }

            return builder.ToString();
        }
    }
}
