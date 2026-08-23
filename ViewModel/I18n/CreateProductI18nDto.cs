using System.ComponentModel.DataAnnotations;

namespace ViewModel.I18n
{
    /// <summary>
    /// 创建商品请求 DTO
    /// </summary>
    public class CreateProductI18nDto
    {
        [Required]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
