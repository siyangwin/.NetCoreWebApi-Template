using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// TikTok获取OpenId返回类
    /// </summary>
    public class TikTokCode2SessionResDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int err_no { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string err_tips { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TikTokCode2SessionData TikTokCode2SessionData { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TikTokCode2SessionData
    {
        /// <summary>
        /// 
        /// </summary>
        public string session_key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string anonymous_openid { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string unionid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string dopenid { get; set; }
    }
}
