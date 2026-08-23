using System;

namespace Model.Table.I18n
{
    /// <summary>
    /// 文章主表（存不可翻译的字段）
    /// </summary>
    public class Article
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string? CoverUrl { get; set; }
        public int ViewCount { get; set; } = 0;
        public bool IsDelete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
