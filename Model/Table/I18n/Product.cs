using System;

namespace Model.Table.I18n
{
    /// <summary>
    /// 商品主表（存不可翻译的字段）
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsDelete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
