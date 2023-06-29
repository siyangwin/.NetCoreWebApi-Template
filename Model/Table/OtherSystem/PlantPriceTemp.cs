using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Table
{
    /// <summary>
    /// 植物市场价格临时表
    /// </summary>
    [Display(Rename = "PlantPriceTemp")]
    public class PlantPriceTemp:BaseModel
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string MBID { get; set; }

        /// <summary>
        /// 市场编号
        /// </summary>
        public string MAreaID { get; set; }

        /// <summary>
        /// 市场名称
        /// </summary>
        public string MArea { get; set; }

        /// <summary>
        /// 规格编号
        /// </summary>
        public string MBSID { get; set; }

        /// <summary>
        /// 规格名称
        /// </summary>
        public string MSpec { get; set; }

        /// <summary>
        /// 价格查询编号
        /// </summary>
        public string mid { get; set; }
    }
}
