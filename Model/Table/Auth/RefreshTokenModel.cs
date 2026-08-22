using Kogel.Dapper.Extension.Attributes;
using System;

namespace Model.Table
{
    /// <summary>
    /// RefreshToken 表实体
    /// </summary>
    [Display(Rename = "RefreshToken")]
    public class RefreshTokenModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// RefreshToken 哈希值（SHA256）
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 关联用户 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Token 族（同一次登录的所有 Token 共享）
        /// </summary>
        public string FamilyId { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 是否已吊销
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// 被哪个新 Token 替换（续签链追踪）
        /// </summary>
        public string ReplacedBy { get; set; }
    }
}
