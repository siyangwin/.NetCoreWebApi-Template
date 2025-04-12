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
    /// 测试-Jwt
    /// </summary>
    /// [Route("[controller]")]
    public class AppUserController : BaseController
    {
        private readonly IAppUserService appUserService;

        /// <summary>
        /// Jwt工具类
        /// </summary>
        private readonly GenerateJwt generateJwt;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="appUserService">用户类</param>
        public AppUserController(IAppUserService appUserService, GenerateJwt generateJwt)
        {
            this.appUserService = appUserService;
            this.generateJwt = generateJwt;
        }


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
                resultModel.success = false;
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
                //userResDto.Token = token;
                //HttpContext.SignInAsync(claims, token, userResDto.Id.ToString(), 240, true);
                resultModel.data = generateJwt.GenerateEncodedTokenAsync(claims);
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
            resultModel.message = "登出成功";
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


            resultModel.data = "当前用户为：" + UserId;
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
            resultModel.data = "当前用户为：" + UserId;
            return resultModel;
        }
    }
}