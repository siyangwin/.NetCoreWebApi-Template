using Model.EnumModel;
using ViewModel;
using ViewModel.I18n;

namespace IService.I18n
{
    /// <summary>
    /// 商品多语言服务接口
    /// </summary>
    public interface II18nProductService : IBaseService
    {
        ResultModel<int> Create(CreateProductI18nDto dto, LanguageEnum language);
        ResultModel<ProductI18nDto> GetById(int id, LanguageEnum language);
        ResultModel<List<ProductI18nDto>> GetList(LanguageEnum language);
        ResultModel SaveTranslation(int productId, SaveTranslationDto dto);
        ResultModel<List<TranslationDetailDto>> GetTranslations(int productId);
        ResultModel Delete(int id);
    }
}
