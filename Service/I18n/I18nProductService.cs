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
    /// 商品多语言服务实现
    /// </summary>
    public class I18nProductService : II18nProductService
    {
        public ResultModel<int> Create(CreateProductI18nDto dto, LanguageEnum language)
        {
            var result = new ResultModel<int>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                int productId = conn.ExecuteScalar<int>(@"
                    INSERT INTO I18nProduct (Price, Stock, ImageUrl, IsDelete, CreatedAt)
                    VALUES (@Price, @Stock, @ImageUrl, 0, GETUTCDATE());
                    SELECT SCOPE_IDENTITY();",
                    new { dto.Price, dto.Stock, dto.ImageUrl }, tran);

                conn.Execute(@"
                    INSERT INTO I18nProductTranslation (ProductId, Language, Name, Description)
                    VALUES (@ProductId, @Language, @Name, @Description)",
                    new { ProductId = productId, Language = langCode, dto.Name, dto.Description }, tran);

                tran.Commit();
                result.Data = productId;
                result.Message = Lang.Get("db_demo:product_created", language);
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            return result;
        }

        public ResultModel<ProductI18nDto> GetById(int id, LanguageEnum language)
        {
            var result = new ResultModel<ProductI18nDto>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            var dto = conn.QueryFirstOrDefault<ProductI18nDto>(@"
                SELECT p.Id, p.Price, p.Stock, p.ImageUrl, t.Name, t.Description
                FROM I18nProduct p
                LEFT JOIN I18nProductTranslation t ON p.Id = t.ProductId AND t.Language = @Language
                WHERE p.Id = @Id AND p.IsDelete = 0",
                new { Id = id, Language = langCode });

            if (dto == null)
            {
                result.Success = false;
                result.Message = Lang.Get("db_demo:product_not_found", language);
                return result;
            }
            result.Data = dto;
            return result;
        }

        public ResultModel<List<ProductI18nDto>> GetList(LanguageEnum language)
        {
            var result = new ResultModel<List<ProductI18nDto>>();
            string langCode = LanguageHelper.ToCode(language);

            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            result.Data = conn.Query<ProductI18nDto>(@"
                SELECT p.Id, p.Price, p.Stock, p.ImageUrl, t.Name, t.Description
                FROM I18nProduct p
                LEFT JOIN I18nProductTranslation t ON p.Id = t.ProductId AND t.Language = @Language
                WHERE p.IsDelete = 0",
                new { Language = langCode }).ToList();
            return result;
        }

        public ResultModel SaveTranslation(int productId, SaveTranslationDto dto)
        {
            var result = new ResultModel();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);

            var existing = conn.QueryFirstOrDefault(
                "SELECT Id FROM I18nProductTranslation WHERE ProductId = @ProductId AND Language = @Language",
                new { ProductId = productId, Language = dto.Language });

            if (existing != null)
            {
                conn.Execute(
                    "UPDATE I18nProductTranslation SET Name = @Name, Description = @Description WHERE ProductId = @ProductId AND Language = @Language",
                    new { ProductId = productId, dto.Language, dto.Name, dto.Description });
            }
            else
            {
                conn.Execute(
                    "INSERT INTO I18nProductTranslation (ProductId, Language, Name, Description) VALUES (@ProductId, @Language, @Name, @Description)",
                    new { ProductId = productId, dto.Language, dto.Name, dto.Description });
            }

            result.Message = Lang.Get("db_demo:translation_saved");
            return result;
        }

        public ResultModel<List<TranslationDetailDto>> GetTranslations(int productId)
        {
            var result = new ResultModel<List<TranslationDetailDto>>();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            result.Data = conn.Query<TranslationDetailDto>(
                "SELECT Language, Name, Description FROM I18nProductTranslation WHERE ProductId = @ProductId",
                new { ProductId = productId }).ToList();
            return result;
        }

        public ResultModel Delete(int id)
        {
            var result = new ResultModel();
            using var conn = new SqlConnection(GlobalConfig.ConnectionString);
            conn.Execute("UPDATE I18nProduct SET IsDelete = 1 WHERE Id = @Id", new { Id = id });
            result.Message = Lang.Get("common:deleted");
            return result;
        }
    }
}
