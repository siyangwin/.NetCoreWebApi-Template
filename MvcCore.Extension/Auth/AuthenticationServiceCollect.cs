using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace MvcCore.Extension.Auth
{
	public static class AuthenticationServiceCollect
	{
		/// <summary>
		/// 加入身份認證空間
		/// </summary>
		/// <param name="services"></param>
		public static void AddAuthentications(this IServiceCollection services,IConfiguration configuration)
		{
            //注入jwt
            services.AddScoped<GenerateJwt>();

            // 注册 JwtConfig 到依赖注入容器中
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            // 实例化 JwtConfig 对象并读取配置
            var jwtConfig = new JwtConfig();
            configuration.Bind("JwtConfig", jwtConfig);


            services.AddAuthentication(options =>
            {
                //认证middleware配置
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //指定Jwt的验证
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //Token颁发机构
                    //指定 JWT 的颁发者（issuer），即表示 JWT 令牌的来源。
                    ValidIssuer = jwtConfig.Issuer, // 设置有效的 Issuer

                    //颁发给谁
                    //指定 JWT 的受众（audience），即表示 JWT 令牌应该被哪些客户端使用。
                    ValidAudience = jwtConfig.Audience, // 设置有效的 Audience

                    //这里的key要进行加密
                    //指定用于对 JWT 签名进行验证的密钥。在这里使用 SymmetricSecurityKey 类型来指定密钥，其可以是任何字节数组，例如使用 Encoding.UTF8.GetBytes() 方法将字符串转换为字节数组。
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)), // 设置密钥

                    //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    //指示是否验证 JWT 令牌的有效期。当服务器收到一个 JWT 令牌时，会检查其 Claims 中的 NotBefore 和 Expires 是否在当前时间范围内。如果 ValidateLifetime 设置为 true，则服务器将拒绝过期的 JWT 令牌，并认为它是无效的。
                    ValidateLifetime = false,

                    //指示是否验证 JWT 的签名密钥。
                    ValidateIssuerSigningKey = false,

                    //指示是否验证 JWT 的颁发者。
                    ValidateIssuer = false,

                    //指示是否验证 JWT 的受众。
                    ValidateAudience = false,
                };
                //指定Jwt的返回内容
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        //此处可以更换请求的参数认证参数

                        // 从请求头中获取 Token
                        //string token = context.Request.Headers["Token"].FirstOrDefault()?.Trim();

                        //// 将 Token 放置到授权的 Header 中
                        //context.Token = token;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // 在 Token 验证通过后的自定义逻辑
                        // 例如，从数据库中获取用户信息，并将其存储到 HttpContext.User 中
                        // context.Principal 是验证通过后的用户信息
                        // 你可以在这里做你的自定义逻辑


                        // 在 Token 验证通过后的自定义逻辑
                        // 例如，检查令牌的有效期，如果过期则执行相应的操作
                        //var validTo = context.SecurityToken.ValidTo;

                        //if (validTo < DateTime.UtcNow)
                        //{
                        //    // Token 已过期，您可以在这里执行相应的操作，例如返回适当的错误响应或者刷新 Token 等
                        //    context.Fail("Token expired");
                        //}
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // 验证失败时的自定义逻辑
                        // 例如，可以在这里记录日志等
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        //当用户的Jwt通过验证,但是状态已过期的情况了
                        //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                        context.HandleResponse();

                        //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                        var payload = JsonConvert.SerializeObject(new { api_version = "v1", success = false, code = "401", message = "很抱歉,您无权访问,请授权!" });
                        //var payload = "no";
                        //自定义返回的数据类型
                        context.Response.ContentType = "application/json";
                        //自定义返回状态码，默认为401 我这里改成 200
                        //StatusCodes.Status401Unauthorized;
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        //输出Json数据结果
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    }
                };
            });
        }
    }
}
