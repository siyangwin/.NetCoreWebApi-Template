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
    /// 植物列表
    /// </summary>
    [Display(Rename = "vm_app_PlantList")]
    public class vm_app_PlantList
    {
        /// <summary>
        /// 植物编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 植物名称
        /// </summary>
        public string PlantName { get; set; }

        /// <summary>
        /// 植物图片
        /// </summary>
        public string PlantPictures { get; set; }

        /// <summary>
        /// 植物科属编号
        /// </summary>
        public int PlantFamilyId { get; set; }
    }
}
