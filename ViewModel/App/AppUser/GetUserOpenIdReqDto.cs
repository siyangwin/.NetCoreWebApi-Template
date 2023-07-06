using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 获取Openid返回类
    /// </summary>
    public class GetUserOpenIdReqDto
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

        /// <summary>
        /// 是否新用户
        /// </summary>
        public bool IsNewUser { get; set; }
    }
}
