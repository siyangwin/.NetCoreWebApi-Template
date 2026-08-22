using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// RefreshToken 管理服务（生成/验证/吊销/续签）
    /// </summary>
    public class RefreshTokenService
    {
        private readonly IConfiguration _configuration;

        public RefreshTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成 RefreshToken（随机 64 字节 → Base64）
        /// </summary>
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// 对 Token 进行 SHA256 哈希（存储用，不存明文）
        /// </summary>
        public string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 生成 FamilyId（同一次登录的 Token 族标识）
        /// </summary>
        public string GenerateFamilyId()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 保存 RefreshToken 到数据库
        /// </summary>
        public void SaveRefreshToken(string tokenHash, int userId, string deviceId, string familyId, DateTime expiresAt)
        {
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO RefreshToken (Token, UserId, DeviceId, FamilyId, ExpiresAt, CreatedAt, IsRevoked)
                VALUES (@Token, @UserId, @DeviceId, @FamilyId, @ExpiresAt, GETUTCDATE(), 0)";
            cmd.Parameters.AddWithValue("@Token", tokenHash);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@DeviceId", (object)deviceId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FamilyId", familyId);
            cmd.Parameters.AddWithValue("@ExpiresAt", expiresAt);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 验证 RefreshToken 是否有效
        /// </summary>
        /// <returns>(isValid, tokenHash, userId, familyId)</returns>
        public (bool IsValid, string TokenHash, int UserId, string FamilyId) ValidateRefreshToken(string token)
        {
            var tokenHash = HashToken(token);
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Token, UserId, FamilyId, ExpiresAt, IsRevoked
                FROM RefreshToken
                WHERE Token = @Token";
            cmd.Parameters.AddWithValue("@Token", tokenHash);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return (false, null, 0, null);

            var isRevoked = reader.GetBoolean(4);
            var expiresAt = reader.GetDateTime(3);
            var userId = reader.GetInt32(1);
            var familyId = reader.GetString(2);
            var storedHash = reader.GetString(0);

            // 如果 Token 已被吊销，说明可能泄露
            if (isRevoked)
                return (false, storedHash, userId, familyId);

            // 如果已过期
            if (expiresAt < DateTime.UtcNow)
                return (false, storedHash, userId, familyId);

            return (true, storedHash, userId, familyId);
        }

        /// <summary>
        /// 吊销 RefreshToken（标记 IsRevoked=true）
        /// </summary>
        public void RevokeToken(string tokenHash)
        {
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE RefreshToken SET IsRevoked = 1 WHERE Token = @Token";
            cmd.Parameters.AddWithValue("@Token", tokenHash);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 吊销同 FamilyId 下所有 Token
        /// </summary>
        public void RevokeTokenFamily(string familyId)
        {
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE RefreshToken SET IsRevoked = 1 WHERE FamilyId = @FamilyId";
            cmd.Parameters.AddWithValue("@FamilyId", familyId);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 标记旧 Token 被新 Token 替换（续签链追踪）
        /// </summary>
        public void MarkReplaced(string oldTokenHash, string newTokenHash)
        {
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE RefreshToken SET ReplacedBy = @NewToken WHERE Token = @OldToken";
            cmd.Parameters.AddWithValue("@OldToken", oldTokenHash);
            cmd.Parameters.AddWithValue("@NewToken", newTokenHash);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取用户当前有效的 RefreshToken 数量
        /// </summary>
        public int GetActiveTokenCount(int userId)
        {
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*) FROM RefreshToken
                WHERE UserId = @UserId AND IsRevoked = 0 AND ExpiresAt > GETUTCDATE()";
            cmd.Parameters.AddWithValue("@UserId", userId);
            return (int)cmd.ExecuteScalar();
        }

        /// <summary>
        /// 获取 AllowMultiLogin 配置
        /// </summary>
        public bool IsMultiLoginAllowed()
        {
            return _configuration.GetValue<bool>("AuthSettings:AllowMultiLogin");
        }

        /// <summary>
        /// 获取 MaxDevicesPerUser 配置
        /// </summary>
        public int GetMaxDevicesPerUser()
        {
            return _configuration.GetValue<int>("AuthSettings:MaxDevicesPerUser");
        }
    }
}
