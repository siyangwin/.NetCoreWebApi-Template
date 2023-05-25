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
                //检查当前的接口是否需要验证
                // 检查当前 Action 是否带有 [AllowAnonymous] 特性
                var actionDescriptor = context.GetEndpoint()?.Metadata?.GetMetadata<ControllerActionDescriptor>();
                var allowAnonymous = actionDescriptor != null && actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() || actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

                // 如果当前 Action 不需要身份验证，则直接调用下一个中间件或控制器
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
                    throw ex;
                }


                //// JWT 校验失败，构造自定义错误信息
                //var error = new { message = "Invalid token." };
                //var json = JsonConvert.SerializeObject(error);

                //// 设置响应状态码和内容
                //context.Response.StatusCode = 401;
                //context.Response.ContentType = "application/json";
                //await context.Response.WriteAsync(json);
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

            //如果为空,变从Query中获取
            if (string.IsNullOrEmpty(token))
                token = context.HttpContext.Request.Query["Token"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    ////获取Token缓存数据
                    //var claims = MemoryCacheHelper.Get<ClaimsIdentity>(token);

                    #region 新添加逻辑：cms用token登录，并且不用保存到db中的判断
                    //新添加逻辑：cms用token登录，并且不用保存到db中的判断
                    if (context.HttpContext.Request.Headers["ClientType"] == "CMS")
                    {
                      
                    }
                    #endregion
                    else
                    {
                       
                    }
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
