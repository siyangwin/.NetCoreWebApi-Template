using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.Table
{
    /// <summary>   
    /// 用户表
    /// </summary>
    [Display(Rename = "UserInfo")]
    public class UserInfo : BaseModel
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 微信小程序唯一编号
        /// </summary>
        public string WechatOpenid { get; set; }

        /// <summary>
        /// 抖音小程序唯一编号
        /// </summary>
        public string TikTokOpenid { get; set; }
    }
}
