namespace Model.Table.I18n
{
    /// <summary>
    /// 文章翻译表（存可翻译的字段）
    /// </summary>
    public class ArticleTranslation
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string Language { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string? Content { get; set; }
    }
}
