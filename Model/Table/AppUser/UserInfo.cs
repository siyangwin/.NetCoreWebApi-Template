using Kogel.Dapper.Extension.Attributes;
using Model.EnumModel;

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

        /// <summary>
        /// 角色（1=Admin, 2=User, 3=Guest）
        /// </summary>
        public int Role { get; set; } = (int)RoleEnum.User;
    }
}
