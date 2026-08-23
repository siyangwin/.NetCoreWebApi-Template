using Core;
using IService.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel;
using ViewModel.I18n;

namespace Project.AppApi.Controllers.I18n
{
    /// <summary>
    /// 商品多语言接口
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    [AllowAnonymous]
    public class ProductI18nController(II18nProductService productService) : BaseController
    {
        /// <summary>
        /// 创建商品（含默认语言翻译）
        /// </summary>
        [HttpPost]
        [Route("api/v2/i18n/product")]
        public ResultModel<int> Create([FromBody] CreateProductI18nDto dto)
        {
            return productService.Create(dto, Language);
        }

        /// <summary>
        /// 查询商品（返回当前语言翻译）
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/product/{id}")]
        public ResultModel<ProductI18nDto> GetById(int id)
        {
            return productService.GetById(id, Language);
        }

        /// <summary>
        /// 查询商品列表（返回当前语言翻译）
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/products")]
        public ResultModel<List<ProductI18nDto>> GetList()
        {
            return productService.GetList(Language);
        }

        /// <summary>
        /// 添加/更新翻译
        /// </summary>
        [HttpPost]
        [Route("api/v2/i18n/product/{id}/translation")]
        public ResultModel SaveTranslation(int id, [FromBody] SaveTranslationDto dto)
        {
            return productService.SaveTranslation(id, dto);
        }

        /// <summary>
        /// 获取所有翻译
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/product/{id}/translations")]
        public ResultModel<List<TranslationDetailDto>> GetTranslations(int id)
        {
            return productService.GetTranslations(id);
        }

        /// <summary>
        /// 软删除商品
        /// </summary>
        [HttpDelete]
        [Route("api/v2/i18n/product/{id}")]
        public ResultModel Delete(int id)
        {
            return productService.Delete(id);
        }
    }
}
