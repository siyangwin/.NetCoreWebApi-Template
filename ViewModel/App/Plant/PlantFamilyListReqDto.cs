using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 植物科属请求类
    /// </summary>
    public class PlantFamilyListReqDto
    {
        /// <summary>
        /// 页码 当前页 从1开始
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页显示数 （默认15）
        /// </summary>
        public int PageSize { get; set; } = 15;
    }
}
