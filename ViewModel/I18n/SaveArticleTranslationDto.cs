using System.ComponentModel.DataAnnotations;

namespace ViewModel.I18n
{
    /// <summary>
    /// 保存文章翻译请求 DTO
    /// </summary>
    public class SaveArticleTranslationDto
    {
        [Required]
        public string Language { get; set; } = "";
        [Required]
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string? Content { get; set; }
    }
}
