using System;

namespace ViewModel.Auth
{
    /// <summary>
    /// API Key 返回 DTO
    /// </summary>
    public class ApiKeyResDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// API Key 明文（仅在创建时返回一次，后续无法再获取）
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Key 前缀（如"sk_test_"）
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限范围
        /// </summary>
        public string Scopes { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>
        public DateTime? LastUsedAt { get; set; }
    }
}
