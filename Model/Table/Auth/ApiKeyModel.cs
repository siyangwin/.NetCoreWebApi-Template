using Kogel.Dapper.Extension.Attributes;
using System;

namespace Model.Table
{
    /// <summary>
    /// ApiKey 表实体
    /// </summary>
    [Display(Rename = "ApiKey")]
    public class ApiKeyModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 名称（如"订单服务"）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// API Key 的 SHA256 哈希
        /// </summary>
        public string KeyHash { get; set; }

        /// <summary>
        /// 前缀（如"sk_live_"），用于识别/日志
        /// </summary>
        public string KeyPrefix { get; set; }

        /// <summary>
        /// 权限范围（逗号分隔，如"order:read,user:write"）
        /// </summary>
        public string Scopes { get; set; }

        /// <summary>
        /// 过期时间（null=永不过期）
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
