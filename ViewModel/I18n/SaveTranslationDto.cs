using System.ComponentModel.DataAnnotations;

namespace ViewModel.I18n
{
    /// <summary>
    /// 保存商品翻译请求 DTO
    /// </summary>
    public class SaveTranslationDto
    {
        [Required]
        public string Language { get; set; } = "";
        [Required]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
