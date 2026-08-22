using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ViewModel;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// API Key 认证中间件（在 JWT 之前执行）
    /// </summary>
    /// <remarks>
    /// 请求流程：
    /// 1. 检查是否有 X-Api-Key Header
    /// 2. 有 → 查库校验 → 有效则设置 HttpContext.User（ApiKey 认证身份）
    /// 3. 无 → 跳过（交给后续 JWT 认证处理）
    /// API Key 和 JWT 是互斥的认证方式，不同时使用。
    /// </remarks>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApiKeyService _apiKeyService;
        private readonly string _headerName;

        public ApiKeyMiddleware(RequestDelegate next, ApiKeyService apiKeyService, IConfiguration configuration)
        {
            _next = next;
            _apiKeyService = apiKeyService;
            _headerName = configuration.GetValue<string>("ApiKeySettings:HeaderName") ?? "X-Api-Key";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 检查是否有 API Key Header
            if (context.Request.Headers.TryGetValue(_headerName, out var apiKeyValues))
            {
                var apiKey = apiKeyValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(apiKey))
                {
                    // 校验 API Key
                    var (isValid, scopes, keyName) = _apiKeyService.ValidateApiKey(apiKey);

                    if (!isValid)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var error = new ResultModel { Success = false, Code = "401", Message = "Invalid API Key" };
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
                        return;
                    }

                    // 创建 API Key 认证身份
                    var claims = new List<Claim>
                    {
                        new Claim("AuthType", "ApiKey"),
                        new Claim("ApiKeyName", keyName ?? ""),
                        new Claim(ClaimTypes.Name, $"ApiKey:{keyName}")
                    };

                    if (!string.IsNullOrEmpty(scopes))
                    {
                        claims.Add(new Claim("ApiKeyScopes", scopes));
                    }

                    var identity = new ClaimsIdentity(claims, "ApiKey");
                    var principal = new ClaimsPrincipal(identity);
                    context.User = principal;
                }
            }

            // 继续管道（如果已通过 API Key 认证，后续 JWT 会识别已认证身份并跳过）
            await _next(context);
        }
    }
}
