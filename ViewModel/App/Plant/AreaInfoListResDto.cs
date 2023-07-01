using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 植物市场规格信息
    /// </summary>
    public class AreaInfoListResDto
    {
        /// <summary>
        /// 植物名称
        /// </summary>
        public string PlantName { get; set; }

        /// <summary>
        /// 植物图片
        /// </summary>
        public string PlantPictures { get; set; }


        /// <summary>
        /// 植物市场规格详细信息列表
        /// </summary>
        public List<AreaInfoListsResDtos> areaInfoListsResDtos { get; set; }

    }

    /// <summary>
    /// 植物市场规格详细信息列表
    /// </summary>
    public class AreaInfoListsResDtos
    {
        /// <summary>
        /// 药市场名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 规格详情
        /// </summary>
        public List<AreaInfoSpecList> Spec { get; set; }
    }

    /// <summary>
    /// 规格详情
    /// </summary>
    public class AreaInfoSpecList
    {
        /// <summary>
        /// 规格名称
        /// </summary>
        public string SpecName { get; set; }

        /// <summary>
        /// 价格编号
        /// </summary>
        public long PriceId { get; set; }
    }
}
