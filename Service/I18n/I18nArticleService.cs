using Core;
using Dapper;
using IService.I18n;
using Model.EnumModel;
using Microsoft.Data.SqlClient;
using ViewModel;
using ViewModel.I18n;

namespace Service.I18n
{
    /// <summary>
    /// 文章多语言服务实现
    /// </summary>
    public class I18nArticleService : II18nArticleService
    {
        public ResultModel<int> Create(CreateArticleI18nDto dto, LanguageEnum language)
        {
            var result = new ResultModel<int>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int articleId = conn.ExecuteScalar<int>(@"
                    INSERT INTO I18nArticle (CategoryId, CoverUrl, ViewCount, IsDelete, CreatedAt)
                    VALUES (@CategoryId, @CoverUrl, 0, 0, GETUTCDATE());
                    SELECT SCOPE_IDENTITY();",
                    new { dto.CategoryId, dto.CoverUrl }, tran);

                conn.Execute(@"
                    INSERT INTO I18nArticleTranslation (ArticleId, Language, Title, Summary, Content)
                    VALUES (@ArticleId, @Language, @Title, @Summary, @Content)",
                    new { ArticleId = articleId, Language = langCode, dto.Title, dto.Summary, dto.Content }, tran);

                tran.Commit();
                result.Data = articleId;
                result.Message = Lang.Get("db_demo:article_created", language);
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            return result;
        }

        public ResultModel<ArticleI18nDto> GetById(int id, LanguageEnum language)
        {
            var result = new ResultModel<ArticleI18nDto>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            var dto = conn.QueryFirstOrDefault<ArticleI18nDto>(@"
                SELECT a.Id, a.CategoryId, a.CoverUrl, a.ViewCount, t.Title, t.Summary, t.Content
                FROM I18nArticle a
                LEFT JOIN I18nArticleTranslation t ON a.Id = t.ArticleId AND t.Language = @Language
                WHERE a.Id = @Id AND a.IsDelete = 0",
                new { Id = id, Language = langCode });

            if (dto == null)
            {
                result.Success = false;
                result.Message = Lang.Get("db_demo:article_not_found", language);
                return result;
            }
            result.Data = dto;
            return result;
        }

        public ResultModel<List<ArticleI18nDto>> GetList(LanguageEnum language)
        {
            var result = new ResultModel<List<ArticleI18nDto>>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            result.Data = conn.Query<ArticleI18nDto>(@"
                SELECT a.Id, a.CategoryId, a.CoverUrl, a.ViewCount, t.Title, t.Summary, t.Content
                FROM I18nArticle a
                LEFT JOIN I18nArticleTranslation t ON a.Id = t.ArticleId AND t.Language = @Language
                WHERE a.IsDelete = 0",
                new { Language = langCode }).ToList();
            return result;
        }

        public ResultModel SaveTranslation(int articleId, SaveArticleTranslationDto dto)
        {
            var result = new ResultModel();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);

            var existing = conn.QueryFirstOrDefault(
                "SELECT Id FROM I18nArticleTranslation WHERE ArticleId = @ArticleId AND Language = @Language",
                new { ArticleId = articleId, Language = dto.Language });

            if (existing != null)
            {
                conn.Execute(
                    "UPDATE I18nArticleTranslation SET Title = @Title, Summary = @Summary, Content = @Content WHERE ArticleId = @ArticleId AND Language = @Language",
                    new { ArticleId = articleId, dto.Language, dto.Title, dto.Summary, dto.Content });
            }
            else
            {
                conn.Execute(
                    "INSERT INTO I18nArticleTranslation (ArticleId, Language, Title, Summary, Content) VALUES (@ArticleId, @Language, @Title, @Summary, @Content)",
                    new { ArticleId = articleId, dto.Language, dto.Title, dto.Summary, dto.Content });
            }

            result.Message = Lang.Get("db_demo:translation_saved");
            return result;
        }

        public ResultModel<List<TranslationDetailDto>> GetTranslations(int articleId)
        {
            var result = new ResultModel<List<TranslationDetailDto>>();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            result.Data = conn.Query<TranslationDetailDto>(
                "SELECT Language, Title AS Name, Summary AS Description FROM I18nArticleTranslation WHERE ArticleId = @ArticleId",
                new { ArticleId = articleId }).ToList();
            return result;
        }

        public ResultModel Delete(int id)
        {
            var result = new ResultModel();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Execute("UPDATE I18nArticle SET IsDelete = 1 WHERE Id = @Id", new { Id = id });
            result.Message = Lang.Get("common:deleted");
            return result;
        }
    }
}
