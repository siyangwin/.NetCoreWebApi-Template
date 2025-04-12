using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// 認證用戶
    /// </summary>
    public static class AuthUsers
    {
        private static object signLock = new object();

        /// <summary>
        /// 登入用戶
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="claims"></param>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <param name="timeOut"></param>
        /// <param name="isForever"></param>
        /// <returns></returns>
        public static void SignInAsync(this HttpContext httpContext, Claim[] claims)
        {
            //string Authorization = generateJwt.GenerateEncodedTokenAsync(claims);
        }


        ///// <summary>
        ///// 登入用戶
        ///// </summary>
        ///// <param name="httpContext"></param>
        ///// <param name="claims"></param>
        ///// <param name="token"></param>
        ///// <param name="userId"></param>
        ///// <param name="timeOut"></param>
        ///// <param name="isForever"></param>
        ///// <returns></returns>
        //public static void SignInAsync(this HttpContext httpContext, Claim[] claims, string token = null, string userId = null, int timeOut = 12, bool isForever = false)
        //{
        //    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    //根據token寫入令牌到緩存
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        //是否永不过期
        //        if (isForever)
        //        {
        //            MemoryCacheHelper.Set(token, claimsIdentity);
        //        }
        //        else
        //        {
        //            MemoryCacheHelper.Set(token, claimsIdentity, TimeSpan.FromHours(timeOut), true);
        //        }
        //        //根據userid寫入所有token到緩存,可以用于单个账户多个设备登录
        //        if (!string.IsNullOrEmpty(userId))
        //        {
        //            var tokenList = MemoryCacheHelper.Get<List<string>>(userId);
        //            if (tokenList == null)
        //                tokenList = new List<string>();
        //            tokenList.Add(token);
        //            //是否永不过期
        //            if (isForever)
        //            {
        //                MemoryCacheHelper.Set(userId, tokenList);
        //            }
        //            else
        //            {
        //                MemoryCacheHelper.Set(userId, tokenList, TimeSpan.FromHours(timeOut), true);
        //            }
        //        }
        //    }
        //    TaskFactory taskFactory = new TaskFactory();
        //    List<Task> taskList = new List<Task>();
        //    //證件所有者
        //    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        //    taskList.Add(taskFactory.StartNew(() =>
        //         //寫入證件到應用程序中
        //         AuthenticationHttpContextExtensions.SignInAsync(httpContext, "CookieAuthenticationScheme", claimsPrincipal, new AuthenticationProperties
        //         {
        //             IsPersistent = true,//持久Cookie
        //             ExpiresUtc = DateTime.Now.AddHours(timeOut),//設置cookie過期時間
        //             AllowRefresh = false,
        //         })
        //     ));
        //    //等待完成才执行后续操作
        //    Task.WaitAny(taskList.ToArray());
        //    ////沉睡100毫秒，保证登录缓存成功写入
        //    //Thread.Sleep(100);
        //}

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static void SignOutAsync(this HttpContext httpContext, string token = null)
        {
            //單用戶注銷token
            if (!string.IsNullOrEmpty(token))
            {
                MemoryCacheHelper.Remove(token);
            }
            else
            {
                AuthenticationHttpContextExtensions.SignOutAsync(httpContext);
            }
        }

        /// <summary>
        /// 登出所有登錄此用戶的token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static void SignOutByUserAsync(this HttpContext httpContext, string userId)
        {
            //找所有登錄的用戶token
            var tokenList = MemoryCacheHelper.Get<List<string>>(userId);
            if (tokenList != null)
            {
                foreach (var token in tokenList)
                {
                    //清除token緩存
                    MemoryCacheHelper.Remove(token);
                }
            }
            //清除userId緩存
            MemoryCacheHelper.Remove(userId);
            //登出Authentication认证
            httpContext.SignOutAsync();
        }
    }
}
