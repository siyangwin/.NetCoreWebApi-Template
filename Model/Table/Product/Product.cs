using Kogel.Dapper.Extension.Attributes;

namespace Model.Table
{
    /// <summary>
    /// 产品表
    /// </summary>
    [Display(Rename = "Product")]
    public class Product:BaseModel
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ProductName { get; set; }
    }
}
