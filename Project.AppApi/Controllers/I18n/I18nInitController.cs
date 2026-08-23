using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ViewModel;

namespace Project.AppApi.Controllers.I18n
{
    /// <summary>
    /// I18n 数据库多语言初始化接口
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    [AllowAnonymous]
    public class I18nInitController : ControllerBase
    {
        /// <summary>
        /// 初始化 I18n 多语言数据库表（幂等）
        /// </summary>
        [HttpPost]
        [Route("api/v2/i18n/init")]
        public ResultModel InitI18nTables()
        {
            var result = new ResultModel();
            try
            {
                using var conn = new SqlConnection(GlobalConfig.ConnectionString);
                conn.Open();

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'I18nProduct')
                    BEGIN
                        CREATE TABLE I18nProduct (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Price DECIMAL(10,2) NOT NULL,
                            Stock INT NOT NULL DEFAULT 0,
                            ImageUrl NVARCHAR(500) NULL,
                            IsDelete BIT NOT NULL DEFAULT 0,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                        );
                    END");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'I18nProductTranslation')
                    BEGIN
                        CREATE TABLE I18nProductTranslation (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ProductId INT NOT NULL,
                            Language NVARCHAR(10) NOT NULL,
                            Name NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(MAX) NULL
                        );
                    END");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_I18nProductTranslation' AND object_id = OBJECT_ID('I18nProductTranslation'))
                    ALTER TABLE I18nProductTranslation ADD CONSTRAINT UQ_I18nProductTranslation UNIQUE (ProductId, Language);");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'I18nArticle')
                    BEGIN
                        CREATE TABLE I18nArticle (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            CategoryId INT NULL,
                            CoverUrl NVARCHAR(500) NULL,
                            ViewCount INT NOT NULL DEFAULT 0,
                            IsDelete BIT NOT NULL DEFAULT 0,
                            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                        );
                    END");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'I18nArticleTranslation')
                    BEGIN
                        CREATE TABLE I18nArticleTranslation (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            ArticleId INT NOT NULL,
                            Language NVARCHAR(10) NOT NULL,
                            Title NVARCHAR(200) NOT NULL,
                            Summary NVARCHAR(500) NULL,
                            Content NVARCHAR(MAX) NULL
                        );
                    END");

                ExecuteIdempotent(conn, @"
                    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_I18nArticleTranslation' AND object_id = OBJECT_ID('I18nArticleTranslation'))
                    ALTER TABLE I18nArticleTranslation ADD CONSTRAINT UQ_I18nArticleTranslation UNIQUE (ArticleId, Language);");

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

        private void ExecuteIdempotent(SqlConnection conn, string sql)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning("I18n init: {Message}", ex.Message);
            }
        }
    }
}
