namespace ViewModel.I18n
{
    /// <summary>
    /// 商品返回 DTO（含当前语言翻译）
    /// </summary>
    public class ProductI18nDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
