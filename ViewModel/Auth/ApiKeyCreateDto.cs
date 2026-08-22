using System.ComponentModel.DataAnnotations;

namespace ViewModel.Auth
{
    /// <summary>
    /// 创建 API Key 请求 DTO
    /// </summary>
    public class ApiKeyCreateDto
    {
        /// <summary>
        /// 名称（如"订单服务"）
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 权限范围（逗号分隔，如"order:read,user:write"）
        /// </summary>
        public string Scopes { get; set; }

        /// <summary>
        /// 过期天数（null=永不过期）
        /// </summary>
        public int? ExpiresInDays { get; set; }
    }
}
