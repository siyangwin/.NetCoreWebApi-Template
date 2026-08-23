namespace Model.Table.I18n
{
    /// <summary>
    /// 商品翻译表（存可翻译的字段）
    /// </summary>
    public class ProductTranslation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Language { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
