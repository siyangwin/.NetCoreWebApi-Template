using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.View
{
    /// <summary>
    /// 植物市场规格关系
    /// </summary>
    [Display(Rename = "vm_app_AreaInfoByPlant")]
    public class vm_app_AreaInfoByPlant
    {
        /// <summary>
        /// 植物科属编号
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
        /// 市场编号
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 市场名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 规格编号
        /// </summary>
        public int SpecId { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string SpecName { get; set; }

        /// <summary>
        /// 价格查询编号
        /// </summary>
        public long PriceId { get; set; }
    }
}
