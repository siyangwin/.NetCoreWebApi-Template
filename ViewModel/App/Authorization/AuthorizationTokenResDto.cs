using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 用户令牌
    /// </summary>
    public class AuthorizationTokenResDto
    {
        /// <summary>
        /// 当前用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 当前用户令牌
        /// </summary>
        public string Token { get; set; }
    }
}
