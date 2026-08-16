using IService.App;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.EnumModel;
using MvcCore.Extension.Auth;
using System.Security.Claims;
using ViewModel;
using ViewModel.App;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// 测试-Jwt（示范 C# 12 主构造函数注入）
    /// </summary>
    /// [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "V1")]
    public class AppUserController(IAppUserService appUserService, GenerateJwt generateJwt) : BaseController
    {

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [Route("api/user/login")]
        [AllowAnonymous]
        [HttpPost]
        public ResultModel<string> Login([FromBody] AuthorizationReqDto AuthorizationInfo)
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            if (AuthorizationInfo == null)
            {
                resultModel.Success = false;
            }
            else
            {
                //string token = Guid.NewGuid().ToString();
                //寫入身份信息到認證中心
                var claims = new[]
                {
                    new Claim("UserId",AuthorizationInfo.account.ToString())
                };
                //登錄并獲取token
                resultModel.Data = generateJwt.GenerateEncodedToken(claims);
            }
            return resultModel;
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [Route("/api/user/loginout")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel> LoginOut()
        {
            ResultModel resultModel = new ResultModel();
            resultModel.Message = "登出成功";
            HttpContext.SignOutAsync();
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