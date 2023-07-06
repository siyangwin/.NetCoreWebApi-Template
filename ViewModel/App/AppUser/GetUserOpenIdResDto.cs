using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 获取Openid请求类
    /// </summary>
    public class GetUserOpenIdResDto
    {
        /// <summary>
        /// 临时Code(五分钟有效)
        /// </summary>
        public string Code{ get; set; }

        /// <summary>
        /// 1:Wechat  2：TikTok
        /// </summary>
        public int Type { get; set; }
    }
}
