using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.View
{
    /// <summary>
    /// 植物科属
    /// </summary>
    [Display(Rename = "vm_app_PlantFamily")]
    public class vm_app_PlantFamily
    {
        /// <summary>
        /// 植物编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 植物科属中文名称
        /// </summary>
        public string NameCN { get; set; }
    }
}
