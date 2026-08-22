using System.ComponentModel.DataAnnotations;

namespace ViewModel.Auth
{
    /// <summary>
    /// 登出请求 DTO
    /// </summary>
    public class LogoutReqDto
    {
        /// <summary>
        /// 刷新令牌（传入则吊销该 Token；不传则吊销当前用户所有 Token）
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// 是否吊销同 FamilyId 下所有 Token（全设备登出）
        /// </summary>
        public bool AllDevices { get; set; }
    }
}
