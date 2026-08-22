using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core;
using ViewModel.Auth;

namespace MvcCore.Extension.Auth
{
    /// <summary>
    /// API Key 管理服务（创建/校验/哈希）
    /// </summary>
    public class ApiKeyService
    {
        private readonly IConfiguration _configuration;

        public ApiKeyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 对 API Key 进行 SHA256 哈希
        /// </summary>
        public string HashApiKey(string apiKey)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 生成 API Key（随机 32 字节 → Base64，带前缀）
        /// </summary>
        public string GenerateApiKey(string prefix = "sk_test_")
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return prefix + Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        /// <summary>
        /// 校验 API Key 是否有效
        /// </summary>
        /// <returns>(isValid, scopes, keyName)</returns>
        public (bool IsValid, string Scopes, string KeyName) ValidateApiKey(string apiKey)
        {
            var keyHash = HashApiKey(apiKey);

            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Name, Scopes, ExpiresAt, IsEnabled
                    FROM ApiKey
                    WHERE KeyHash = @KeyHash";
                cmd.Parameters.AddWithValue("@KeyHash", keyHash);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return (false, null, null);

                var isEnabled = reader.GetBoolean(3);
                if (!isEnabled)
                    return (false, null, null);

                if (!reader.IsDBNull(2))
                {
                    var expiresAt = reader.GetDateTime(2);
                    if (expiresAt < DateTime.UtcNow)
                        return (false, null, null);
                }

                var name = reader.GetString(0);
                var scopes = reader.IsDBNull(1) ? null : reader.GetString(1);

                // 更新最后使用时间
                UpdateLastUsedAt(keyHash);

                return (true, scopes, name);
            }
            catch
            {
                return (false, null, null);
            }
        }

        /// <summary>
        /// 创建 API Key
        /// </summary>
        /// <returns>(apiKey 明文, 创建结果)</returns>
        public (string ApiKey, ApiKeyResDto Result) CreateApiKey(string name, string scopes, int? expiresInDays)
        {
            string prefix = "sk_test_";
            string apiKey = GenerateApiKey(prefix);
            string keyHash = HashApiKey(apiKey);
            DateTime? expiresAt = expiresInDays.HasValue ? DateTime.UtcNow.AddDays(expiresInDays.Value) : (DateTime?)null;
            DateTime now = DateTime.UtcNow;

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO ApiKey (Name, KeyHash, KeyPrefix, Scopes, ExpiresAt, IsEnabled, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@Name, @KeyHash, @KeyPrefix, @Scopes, @ExpiresAt, 1, @CreatedAt)";
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@KeyHash", keyHash);
            cmd.Parameters.AddWithValue("@KeyPrefix", prefix);
            cmd.Parameters.AddWithValue("@Scopes", (object)scopes ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ExpiresAt", (object)expiresAt ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", now);
            int id = (int)cmd.ExecuteScalar();

            var dto = new ApiKeyResDto
            {
                Id = id,
                ApiKey = apiKey,
                KeyPrefix = prefix,
                Name = name,
                Scopes = scopes,
                ExpiresAt = expiresAt,
                IsEnabled = true,
                CreatedAt = now,
                LastUsedAt = null
            };

            return (apiKey, dto);
        }

        /// <summary>
        /// 查询所有 API Key 列表（不含明文）
        /// </summary>
        public List<ApiKeyResDto> GetAllApiKeys()
        {
            var list = new List<ApiKeyResDto>();
            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Id, Name, KeyPrefix, Scopes, ExpiresAt, IsEnabled, CreatedAt, LastUsedAt
                    FROM ApiKey
                    ORDER BY CreatedAt DESC";

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ApiKeyResDto
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        KeyPrefix = reader.GetString(2),
                        Scopes = reader.IsDBNull(3) ? null : reader.GetString(3),
                        ExpiresAt = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                        IsEnabled = reader.GetBoolean(5),
                        CreatedAt = reader.GetDateTime(6),
                        LastUsedAt = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7)
                    });
                }
            }
            catch
            {
                // 查询失败返回空列表
            }
            return list;
        }

        /// <summary>
        /// 吊销/删除指定 API Key
        /// </summary>
        public bool RevokeApiKey(int id)
        {
            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM ApiKey WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateLastUsedAt(string keyHash)
        {
            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE ApiKey SET LastUsedAt = GETUTCDATE() WHERE KeyHash = @KeyHash";
                cmd.Parameters.AddWithValue("@KeyHash", keyHash);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // 更新失败不影响请求，仅记录日志
            }
        }
    }
}
