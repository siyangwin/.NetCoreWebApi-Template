using Model.EnumModel;
using ViewModel;
using ViewModel.I18n;

namespace IService.I18n
{
    /// <summary>
    /// 文章多语言服务接口
    /// </summary>
    public interface II18nArticleService : IBaseService
    {
        ResultModel<int> Create(CreateArticleI18nDto dto, LanguageEnum language);
        ResultModel<ArticleI18nDto> GetById(int id, LanguageEnum language);
        ResultModel<List<ArticleI18nDto>> GetList(LanguageEnum language);
        ResultModel SaveTranslation(int articleId, SaveArticleTranslationDto dto);
        ResultModel<List<TranslationDetailDto>> GetTranslations(int articleId);
        ResultModel Delete(int id);
    }
}
