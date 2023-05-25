using System.ComponentModel.DataAnnotations;

namespace ViewModel.App
{
    /// <summary>
    /// 授权请求类
    /// </summary>
    public class AuthorizationReqDto
    {
        /// <summary>
        /// 賬號
        /// </summary>
        [Required]
        public string account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string password { get; set; }
    }
}
