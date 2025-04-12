using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// 自定义身份认证
    /// </summary>
    public class AuthValidator : Attribute, IAuthorizationFilter
    {
        //private readonly RequestDelegate _next;

        //public AuthValidator(RequestDelegate next)
        //{
        //    _next = next;
        //}

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 获取用户 ID (sub) 声明值
               // var UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // 获取 UserId 声明
                var UserId = context.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (UserId != null && !string.IsNullOrEmpty(UserId))
                {
                    //身份写入Header
                    context.Request.Headers.Add("UserId", UserId);
                }

                //检查当前的接口是否需要验证
                // 检查当前 Action 是否带有 [AllowAnonymous] 特性
                var actionDescriptor = context.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>();
                var allowAnonymous = actionDescriptor != null && actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() || actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

                // 如果当前Action 不需要身份验证，则直接调用下一个中间件或控制器
                if (allowAnonymous || !context.User.Identity.IsAuthenticated)
                {
                    //await _next(context);
                }
                else
                {
                    //依然判断需要验证的API方法
                    var principal = context.User;

                    // 如果 JWT 验证成功并且用户已经被认证，则调用黑名单服务进行检查
                    if (principal.Identity.IsAuthenticated)  //&& !blacklistService.IsTokenBlacklisted(principal)
                    {
                        //await _next(context);
                    }
                    else
                    {
                        // 如果 JWT 不在黑名单中，则返回 401 Unauthorized 响应
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message = "Unauthorized." }));
                    }
                }
            }
            catch (Exception ex)
            {

                if (ex is SecurityTokenExpiredException)
                {
                    // JWT 过期，构造自定义错误消息
                    var error = new { message = "Token has expired." };
                    var json = JsonConvert.SerializeObject(error);

                    // 设置响应状态码和内容
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                }
                else if (ex is SecurityTokenInvalidSignatureException)
                {
                    // JWT 签名无效，构造自定义错误消息
                    var error = new { message = "Token signature is invalid." };
                    var json = JsonConvert.SerializeObject(error);

                    // 设置响应状态码和内容
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    // 其他错误，交给 ASP.NET Core 处理
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 重新自定義認證
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            this.Invoke(context.HttpContext);
            ////检查当前的接口是否需要验证
            //// 检查当前 Action 是否带有 [AllowAnonymous] 特性
            //var actionDescriptor = context.HttpContext.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>();
            //var allowAnonymous = actionDescriptor != null && actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() || actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

            //// 如果当前Action 不需要身份验证，则直接调用下一个中间件或控制器
            //// context.HttpContext.User.Identity.IsAuthenticated 标识是否通过身份验证
            //if (allowAnonymous || !context.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    //API不需要校验
            //}
            //else
            //{
            //    //API需要校验
            //    //从Header中获取token
            //    string token = context.HttpContext.Request.Headers["Token"];

            //    //如果为空，则尝试从Query中获取
            //    if (string.IsNullOrEmpty(token))
            //        token = context.HttpContext.Request.Query["Token"];

            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        try
            //        {
            //            //this.Invoke(context.HttpContext);
            //            //此处可以分别为APP和CMS做不同的操作
            //            //获取Token缓存数据
            //            var claims = MemoryCacheHelper.Get<ClaimsIdentity>(token);
            //            //获取请求目标，判断何种检验方式
            //            //if (context.HttpContext.Request.Headers["ClientType"] == "APP")
            //            //{ 

            //            //}

            //            if (claims != null)
            //            {
            //                context.HttpContext.Request.Headers.Add("UserId", claims.Claims.FirstOrDefault(a => a.Type == "UserId")?.Value);
            //            }
            //            else
            //            {
            //                if (allowAnonymous)
            //                    return;
            //                if (context.HttpContext.Request.Path != "/api/appuser/loginout")
            //                {
            //                    //不存在的token直接拒絕
            //                    context.Result = new JsonResult(new { success = false, Code = "401", Message = "沒有授權" });
            //                    context.HttpContext.Response.StatusCode = 200;

            //                    //写入日志
            //                    //NewMethod(context);
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            if (allowAnonymous)
            //                return;
            //            if (context.HttpContext.Request.Path != "/api/appuser/loginout")
            //            {
            //                //不存在的token直接拒絕
            //                context.Result = new JsonResult(new { success = false, Code = "401", Message = "沒有授權" });
            //                context.HttpContext.Response.StatusCode = 200;
            //                //写入日志
            //                //NewMethod(context);
            //            }
            //        }
            //    }
            //    else
            //    {

            //    }
            //}
        }
    }
}
