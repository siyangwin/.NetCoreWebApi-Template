using System.ComponentModel.DataAnnotations;

namespace ViewModel.I18n
{
    /// <summary>
    /// 创建文章请求 DTO
    /// </summary>
    public class CreateArticleI18nDto
    {
        public int? CategoryId { get; set; }
        public string? CoverUrl { get; set; }
        [Required]
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string? Content { get; set; }
    }
}
