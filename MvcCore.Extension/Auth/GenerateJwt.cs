using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// Jwt工具类
    /// </summary>
    public class GenerateJwt
    {
        private readonly IOptionsMonitor<JwtConfig> _jwtConfigMonitor;

        public GenerateJwt(IOptionsMonitor<JwtConfig> jwtConfigMonitor)
        {
            _jwtConfigMonitor = jwtConfigMonitor;
        }

        //每次签发读取最新配置（IOptionsMonitor 支持运行期配置热更新）
        private JwtConfig Config => _jwtConfigMonitor.CurrentValue;

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="UserId">携带的用户信息</param>
        /// <returns></returns>
        public string GenerateEncodedToken(int UserId)
        {
            //创建用户身份标识，可按需要添加更多信息
            var claims = new List<Claim>
            {
                new Claim("UserId", UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, UserId.ToString())
            };
            return BuildToken(claims);
        }


        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="UserId">携带的用户信息</param>
        /// <returns></returns>
        public string GenerateEncodedToken(Claim[] claim)
        {
            return BuildToken(claim.ToList());
        }

        private string BuildToken(List<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: Config.Issuer,
                audience: Config.Audience,
                claims: claims,
                notBefore: Config.NotBefore,
                expires: Config.Expiration,
                signingCredentials: Config.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
