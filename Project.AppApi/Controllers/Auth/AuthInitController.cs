using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ViewModel;

namespace Project.AppApi.Controllers.Auth
{
    /// <summary>
    /// 认证体系初始化接口（建表）
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    [AllowAnonymous]
    public class AuthInitController : ControllerBase
    {
        /// <summary>
        /// 初始化认证体系数据库表（幂等：已存在则跳过）
        /// </summary>
        /// <remarks>
        /// 自动创建 RefreshToken 表、ApiKey 表、UserInfo 表新增 Role 列。可重复调用不会报错。
        /// 前置条件：UserInfo 表需已存在（先调用 POST api/v2/demo/init 或手动建表）。
        /// </remarks>
        [HttpPost]
        [Route("api/v1/auth/init")]
        public ResultModel InitAuthTables()
        {
            var result = new ResultModel();
            var messages = new List<string>();

            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();

                // 1. 创建 RefreshToken 表
                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshToken')
                    BEGIN
                        CREATE TABLE RefreshToken (
                            Id           INT IDENTITY(1,1) PRIMARY KEY,
                            Token        NVARCHAR(200) NOT NULL,
                            UserId       INT NOT NULL,
                            DeviceId     NVARCHAR(100) NULL,
                            FamilyId     NVARCHAR(50) NOT NULL,
                            ExpiresAt    DATETIME2 NOT NULL,
                            CreatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            IsRevoked    BIT NOT NULL DEFAULT 0,
                            ReplacedBy   NVARCHAR(200) NULL
                        );
                    END", "RefreshToken 表");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RefreshToken_Token' AND object_id = OBJECT_ID('RefreshToken'))
                        CREATE UNIQUE INDEX IX_RefreshToken_Token ON RefreshToken(Token);
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RefreshToken_UserId' AND object_id = OBJECT_ID('RefreshToken'))
                        CREATE INDEX IX_RefreshToken_UserId ON RefreshToken(UserId);
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RefreshToken_FamilyId' AND object_id = OBJECT_ID('RefreshToken'))
                        CREATE INDEX IX_RefreshToken_FamilyId ON RefreshToken(FamilyId);", "RefreshToken 索引");

                // 2. 创建 ApiKey 表
                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ApiKey')
                    BEGIN
                        CREATE TABLE ApiKey (
                            Id           INT IDENTITY(1,1) PRIMARY KEY,
                            Name         NVARCHAR(100) NOT NULL,
                            KeyHash      NVARCHAR(200) NOT NULL,
                            KeyPrefix    NVARCHAR(10) NOT NULL,
                            Scopes       NVARCHAR(500) NULL,
                            ExpiresAt    DATETIME2 NULL,
                            IsEnabled    BIT NOT NULL DEFAULT 1,
                            CreatedAt    DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                            LastUsedAt   DATETIME2 NULL
                        );
                    END", "ApiKey 表");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ApiKey_KeyHash' AND object_id = OBJECT_ID('ApiKey'))
                        CREATE UNIQUE INDEX IX_ApiKey_KeyHash ON ApiKey(KeyHash);", "ApiKey 索引");

                // 3. UserInfo 表新增 Role 列
                ExecuteIdempotent(conn, @"
                    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserInfo')
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('UserInfo') AND name = 'Role')
                        BEGIN
                            ALTER TABLE UserInfo ADD Role INT NOT NULL DEFAULT 2;
                        END
                    END", "UserInfo.Role 列");

                // 4. SystemLog 表新增 AuthType、AuthIdentity 列
                ExecuteIdempotent(conn, @"
                    IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SystemLog')
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SystemLog') AND name = 'AuthType')
                            ALTER TABLE SystemLog ADD AuthType NVARCHAR(20) NULL;
                        IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('SystemLog') AND name = 'AuthIdentity')
                            ALTER TABLE SystemLog ADD AuthIdentity NVARCHAR(200) NULL;
                    END", "SystemLog.AuthType/AuthIdentity 列");

                result.Success = true;
                result.Message = Lang.Get("db_demo:init_success");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = Lang.GetFormat("db_demo:init_failed", ex.Message);
            }

            return result;
        }

        private void ExecuteIdempotent(SqlConnection conn, string sql, string description)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // 单步失败不阻断整体流程，记录即可
                Serilog.Log.Warning("{Description} 操作异常（不影响启动）：{Message}", description, ex.Message);
            }
        }
    }
}
