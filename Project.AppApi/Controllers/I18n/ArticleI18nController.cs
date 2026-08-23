using Core;
using IService.I18n;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel;
using ViewModel.I18n;

namespace Project.AppApi.Controllers.I18n
{
    /// <summary>
    /// 文章多语言接口
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    [AllowAnonymous]
    public class ArticleI18nController(II18nArticleService articleService) : BaseController
    {
        /// <summary>
        /// 创建文章（含默认语言翻译）
        /// </summary>
        [HttpPost]
        [Route("api/v2/i18n/article")]
        public ResultModel<int> Create([FromBody] CreateArticleI18nDto dto)
        {
            return articleService.Create(dto, Language);
        }

        /// <summary>
        /// 查询文章（返回当前语言翻译）
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/article/{id}")]
        public ResultModel<ArticleI18nDto> GetById(int id)
        {
            return articleService.GetById(id, Language);
        }

        /// <summary>
        /// 查询文章列表（返回当前语言翻译）
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/articles")]
        public ResultModel<List<ArticleI18nDto>> GetList()
        {
            return articleService.GetList(Language);
        }

        /// <summary>
        /// 添加/更新翻译
        /// </summary>
        [HttpPost]
        [Route("api/v2/i18n/article/{id}/translation")]
        public ResultModel SaveTranslation(int id, [FromBody] SaveArticleTranslationDto dto)
        {
            return articleService.SaveTranslation(id, dto);
        }

        /// <summary>
        /// 获取所有翻译
        /// </summary>
        [HttpGet]
        [Route("api/v2/i18n/article/{id}/translations")]
        public ResultModel<List<TranslationDetailDto>> GetTranslations(int id)
        {
            return articleService.GetTranslations(id);
        }

        /// <summary>
        /// 软删除文章
        /// </summary>
        [HttpDelete]
        [Route("api/v2/i18n/article/{id}")]
        public ResultModel Delete(int id)
        {
            return articleService.Delete(id);
        }
    }
}
