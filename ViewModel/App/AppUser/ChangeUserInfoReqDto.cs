using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 修改用户信息请求类
    /// </summary>
    public class ChangeUserInfoReqDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }
}
