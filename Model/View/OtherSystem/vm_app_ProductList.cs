using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.View
{
    /// <summary>
    /// 所有产品编号数据
    /// </summary>
    [Display(Rename = "vm_app_ProductList")]
    public class vm_app_ProductList
    {
        //植物编号
        public int Productid { get; set; }
    }
}
