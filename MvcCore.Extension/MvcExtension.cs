using Microsoft.AspNetCore.Http;

namespace MvcCore.Extension
{
    /// <summary>
    /// mvc扩展方法
    /// </summary>
    public static class MvcExtension
    {
        /// <summary>
        /// 获取路径或者头部的参数
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="paramName">参数名称</param>
        /// <returns></returns>
        public static string QueryOrHeaders(this HttpContext httpContext, string paramName)
        {
            string data = httpContext.Request.Query[paramName];
            if (string.IsNullOrEmpty(data))
            {
                data = httpContext.Request.Headers[paramName];
            }
            if (string.IsNullOrEmpty(data))
            {
                if (httpContext.Request.HasFormContentType)
                {
                    data = httpContext.Request.Form[paramName];
                }
            }
            return data;
        }

        /// <summary>
        /// 设置头部值
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public static void SetHeaders(this HttpContext httpContext, string paramName, string value)
        {
            if (httpContext.Request.Headers.Keys.Any(x => x.ToUpper().Equals(paramName.ToUpper())))
            {
                httpContext.Request.Headers[paramName] = value;
            }
            else
            {
                httpContext.Request.Headers.Add(paramName, value);
            }
        }
    }
}
