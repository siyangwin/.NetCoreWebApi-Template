using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 植物列表返回类
    /// </summary>
    public class PlantListResDto
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
    }
}
