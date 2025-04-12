using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.View
{
    /// <summary>
    /// 配置
    /// </summary>
    [Display(Rename = "vw_app_Config")]
    public class vw_public_Config
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称(code)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Values { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
