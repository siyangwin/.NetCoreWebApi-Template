using System.ComponentModel.DataAnnotations;

namespace ViewModel.Auth
{
    /// <summary>
    /// 刷新 Token 请求 DTO
    /// </summary>
    public class RefreshReqDto
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 设备标识（可选，用于多设备管理）
        /// </summary>
        public string DeviceId { get; set; }
    }
}
