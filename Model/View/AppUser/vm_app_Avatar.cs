using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.View
{
    /// <summary>
    /// 头像List
    /// </summary>
    [Display(Rename = "vm_app_Avatar")]
    public class vm_app_Avatar
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Url { get; set; }
    }
}
