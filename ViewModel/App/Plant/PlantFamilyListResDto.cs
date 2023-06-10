using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 植物科属返回类
    /// </summary>
    public class PlantFamilyListResDto
    {
        /// <summary>
        /// 植物编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 植物科属名称
        /// </summary>
        public string Name { get; set; }
    }
}
