namespace ViewModel.Auth
{
    /// <summary>
    /// 登录返回 DTO（双 Token）
    /// </summary>
    public class LoginResDto
    {
        /// <summary>
        /// 访问令牌（短期，如 30 分钟）
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌（长期，如 7 天）
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// AccessToken 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
    }
}
