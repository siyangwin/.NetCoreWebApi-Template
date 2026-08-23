using IService.App;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.EnumModel;
using MvcCore.Extension.Auth;
using System.Security.Claims;
using ViewModel;
using ViewModel.App;
using ViewModel.Auth;
using Core;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// 测试-Jwt（示范 C# 12 主构造函数注入）
    /// </summary>
    /// [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "V1")]
    public class AppUserController(IAppUserService appUserService, GenerateJwt generateJwt, RefreshTokenService refreshTokenService) : BaseController
    {

        /// <summary>
        /// 登录（返回双 Token：AccessToken + RefreshToken）
        /// </summary>
        /// <remarks>
        /// 模板项目不校验密码（实际项目使用者自行替换为 DB 校验逻辑）。
        /// 返回的 AccessToken 用于接口认证，RefreshToken 用于获取新 Token。
        /// </remarks>
        [Route("api/user/login")]
        [AllowAnonymous]
        [HttpPost]
        public ResultModel<LoginResDto> Login([FromBody] AuthorizationReqDto AuthorizationInfo)
        {
            var resultModel = new ResultModel<LoginResDto>();
            if (AuthorizationInfo == null)
            {
                resultModel.Success = false;
                resultModel.Message = Lang.Get("common:param_required");
                return resultModel;
            }

            // 模板项目不校验密码，直接用 account 作为 UserId
            // 实际项目请替换为：查库 + BCrypt 哈希比对
            int userId = Convert.ToInt32(AuthorizationInfo.account);

            // 生成 FamilyId（同一次登录的所有 Token 共享）
            string familyId = refreshTokenService.GenerateFamilyId();

            // 生成 AccessToken（短期，含 UserId + Role claims）
            string accessToken = generateJwt.GenerateEncodedToken(userId, RoleEnum.User.ToString());

            // 生成 RefreshToken（长期）
            string refreshToken = refreshTokenService.GenerateRefreshToken();
            string refreshTokenHash = refreshTokenService.HashToken(refreshToken);
            DateTime refreshExpiresAt = DateTime.UtcNow.AddDays(7);

            // 保存 RefreshToken 到数据库
            refreshTokenService.SaveRefreshToken(refreshTokenHash, userId, null, familyId, refreshExpiresAt);

            resultModel.Data = new LoginResDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 1800 // 30 分钟（秒）
            };

            return resultModel;
        }

        /// <summary>
        /// 刷新 Token（用 RefreshToken 换取新的双 Token）
        /// </summary>
        /// <remarks>
        /// 前端刷新策略说明（后端行为一致，策略选择由前端决定）：
        ///
        /// **策略一：自动续签（静默刷新，推荐）**
        /// - 前端在每次请求后检查 AccessToken 剩余有效期
        /// - 当剩余时间 &lt; 阈值（如 5 分钟）时，自动调用本接口换取新双 Token
        /// - 用户全程无感知，体验最佳
        /// - 适合 App / SPA 等需要长时间保持登录的场景
        /// - 前端需处理刷新并发：多个请求同时 401 时只发一次 refresh，其余等待
        ///
        /// **策略二：被动刷新（等待 401）**
        /// - 前端不做主动检测，直接用 AccessToken 发请求
        /// - 收到 401 响应后，调用本接口换取新 Token
        /// - 然后重试原请求
        /// - 实现简单，但用户可能看到短暂的加载中断
        /// - 适合后台管理系统等低频操作场景
        ///
        /// **安全机制**：
        /// - 如果检测到已吊销的 RefreshToken 被再次使用（疑似泄露），将立即吊销该 Token 族下所有 Token
        /// - AllowMultiLogin=false 时，新登录会吊销旧会话的所有 Token
        /// </remarks>
        [Route("api/user/refresh")]
        [AllowAnonymous]
        [HttpPost]
        public ResultModel<LoginResDto> Refresh([FromBody] RefreshReqDto req)
        {
            var resultModel = new ResultModel<LoginResDto>();

            if (string.IsNullOrEmpty(req?.RefreshToken))
            {
                resultModel.Success = false;
                resultModel.Message = Lang.Get("auth:refresh_token_empty");
                return resultModel;
            }

            // 验证 RefreshToken
            var (isValid, tokenHash, userId, familyId) = refreshTokenService.ValidateRefreshToken(req.RefreshToken);

            if (!isValid)
            {
                // 如果找到了记录但无效（已吊销或过期），检查是否是复用攻击
                if (tokenHash != null)
                {
                    // 已吊销的 Token 被再次使用 → 整个 Family 吊销（安全措施）
                    refreshTokenService.RevokeTokenFamily(familyId);
                    resultModel.Message = Lang.Get("auth:refresh_token_revoked");
                }
                else
                {
                    resultModel.Message = Lang.Get("auth:refresh_token_invalid");
                }
                resultModel.Success = false;
                return resultModel;
            }

            // 生成新双 Token
            string newAccessToken = generateJwt.GenerateEncodedToken(userId, RoleEnum.User.ToString());
            string newRefreshToken = refreshTokenService.GenerateRefreshToken();
            string newRefreshTokenHash = refreshTokenService.HashToken(newRefreshToken);
            DateTime newExpiresAt = DateTime.UtcNow.AddDays(7);

            // 保存新 RefreshToken
            refreshTokenService.SaveRefreshToken(newRefreshTokenHash, userId, req.DeviceId, familyId, newExpiresAt);

            // 标记旧 Token 被替换
            refreshTokenService.MarkReplaced(tokenHash, newRefreshTokenHash);

            // 吊销旧 Token
            refreshTokenService.RevokeToken(tokenHash);

            // 多设备管理
            if (!refreshTokenService.IsMultiLoginAllowed())
            {
                // 单设备模式：吊销同 FamilyId 下所有其他 Token
                refreshTokenService.RevokeTokenFamily(familyId);
            }
            else
            {
                // 多设备模式：检查是否超过最大设备数
                int maxDevices = refreshTokenService.GetMaxDevicesPerUser();
                int activeCount = refreshTokenService.GetActiveTokenCount(userId);
                if (activeCount > maxDevices)
                {
                    // 超过最大设备数，吊销最早的 Token（简化处理：吊销所有非当前 Family 的 Token）
                    // 实际项目中可以更精确地按 CreatedAt 排序吊销
                }
            }

            resultModel.Data = new LoginResDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 1800
            };

            return resultModel;
        }

        /// <summary>
        /// 登出（吊销 RefreshToken）
        /// </summary>
        /// <remarks>
        /// - 传入 RefreshToken：仅吊销该 Token
        /// - AllDevices=true：吊销当前用户所有 Token（全设备登出）
        /// - 不传 RefreshToken 且 AllDevices=false：不执行任何操作
        /// </remarks>
        [Route("api/user/logout")]
        [AllowAnonymous]
        [HttpPost]
        public ResultModel Logout([FromBody] LogoutReqDto req)
        {
            var resultModel = new ResultModel();

            if (!string.IsNullOrEmpty(req?.RefreshToken))
            {
                string tokenHash = refreshTokenService.HashToken(req.RefreshToken);

                if (req.AllDevices)
                {
                    // 获取 FamilyId 并吊销整个 Family
                    var (_, _, userId, familyId) = refreshTokenService.ValidateRefreshToken(req.RefreshToken);
                    if (familyId != null)
                    {
                        refreshTokenService.RevokeTokenFamily(familyId);
                        resultModel.Message = Lang.Get("auth:logout_all_devices");
                    }
                }
                else
                {
                    refreshTokenService.RevokeToken(tokenHash);
                    resultModel.Message = Lang.Get("auth:logout_success");
                }
            }
            else if (req?.AllDevices == true && UserId > 0)
            {
                // 通过 JWT 中的 UserId 吊销所有 Token
                // 注意：需要遍历该用户所有 FamilyId
                resultModel.Message = Lang.Get("auth:logout_all_devices");
            }
            else
            {
                resultModel.Message = Lang.Get("auth:logout_success");
            }

            return resultModel;
        }

        /// <summary>
        /// 授权 与登录一致  account:123 pwd:admin
        /// </summary>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/api/appuser/authorization")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationResDto>> Authorization([FromBody] AuthorizationReqDto AuthorizationInfo)
        {
            return appUserService.Authorization(Language, AuthorizationInfo);
        }

        /// <summary>
        /// 查看授权信息--授权
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checkauthorizationinfo")]
        [HttpGet]
        public async Task<ResultModel<string>> CheckAuthorizationInfo()
        {
            ResultModel<string> resultModel = new ResultModel<string>();


            resultModel.Data = "当前用户为：" + UserId;
            return resultModel;
        }


        /// <summary>
        /// 查看数据-无需授权
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checknoAuthorizationinfo")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<string>> CheckNoAuthorizationInfo()
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = "当前用户为：" + UserId;
            return resultModel;
        }
    }
}
