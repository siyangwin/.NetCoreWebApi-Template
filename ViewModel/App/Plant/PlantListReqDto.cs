using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 植物列表请求类
    /// </summary>
    public class PlantListReqDto
    {
        /// <summary>
        /// 页码 当前页 从1开始
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页显示数 （默认15）
        /// </summary>
        public int PageSize { get; set; } = 15;

        /// <summary>
        /// 植物科属编号
        /// </summary>
        public int PlantFamilyId { get; set; }
    }
}
