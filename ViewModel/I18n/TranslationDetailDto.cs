namespace ViewModel.I18n
{
    /// <summary>
    /// 翻译详情 DTO（通用）
    /// </summary>
    public class TranslationDetailDto
    {
        public string Language { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
    }
}
