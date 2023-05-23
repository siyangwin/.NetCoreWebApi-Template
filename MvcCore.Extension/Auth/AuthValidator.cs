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

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// 自定义身份认证
    /// </summary>
    public class AuthValidator : Attribute, IAuthorizationFilter
    {

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
