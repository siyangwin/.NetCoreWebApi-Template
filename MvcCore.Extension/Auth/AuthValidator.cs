using Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// 自定义身份认证
    /// </summary>
    public class AuthValidator
    {
        private readonly RequestDelegate _next;

        public AuthValidator(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 获取用户 ID (sub) 声明值
                var UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                    await _next(context);
                }
                else
                {
                    //依然判断需要验证的API方法
                    var principal = context.User;

                    // 如果 JWT 验证成功并且用户已经被认证，则调用黑名单服务进行检查
                    if (principal.Identity.IsAuthenticated)  //&& !blacklistService.IsTokenBlacklisted(principal)
                    {
                        await _next(context);
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
                    //throw ex;
                }
            }
        }


        /// <summary>
        /// 重新自定義認證
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //是否需要授权的Api
            bool isAllowAnonymous = context.ActionDescriptor.FilterDescriptors.Any(x => x.Filter.ToString() == typeof(AllowAnonymousFilter).ToString());

            //从Header中获取token
            string token = context.HttpContext.Request.Headers["Token"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                   //此处可以分别为APP和CMS做不同的操作
                }
                catch (Exception ex)
                {
            
                }
            }
            else
            {
                
            }
        }
    }
}
