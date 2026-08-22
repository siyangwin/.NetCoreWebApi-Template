using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// Jwt配置类
    /// </summary>
    public class JwtConfig
    {
        //密钥  可以是Guid 也可以是随便一个字符串
        public string SecretKey { get; set; }

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// AccessToken 过期时间（分钟）
        /// </summary>
        public int AccessExpired { get; set; }

        /// <summary>
        /// RefreshToken 过期时间（分钟，默认 10080 = 7 天）
        /// </summary>
        public int RefreshExpired { get; set; } = 10080;


        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public DateTime Expiration => IssuedAt.AddMinutes(AccessExpired);
        private SecurityKey SigningKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        public SigningCredentials SigningCredentials =>
            new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256);
    }
}
