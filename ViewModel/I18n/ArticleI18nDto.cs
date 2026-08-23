namespace ViewModel.I18n
{
    /// <summary>
    /// 文章返回 DTO（含当前语言翻译）
    /// </summary>
    public class ArticleI18nDto
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? CoverUrl { get; set; }
        public int ViewCount { get; set; }
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string? Content { get; set; }
    }
}
