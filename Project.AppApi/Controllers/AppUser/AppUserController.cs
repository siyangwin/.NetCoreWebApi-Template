using IService.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModel;
using ViewModel.App;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    //[Route("[controller]")]
    public class AppUserController: BaseController
    {
        private readonly IAppUserService appUserService;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="appUserService">用户类</param>
        public AppUserController(IAppUserService appUserService)
        {
            this.appUserService = appUserService;
        }


        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        [Route("/api/appuser/authorization")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationResDto>> Authorization(string account,string password)
        {
            //return null;
            AuthorizationReqDto AuthorizationInfo = new AuthorizationReqDto();
            AuthorizationInfo.account = account;
            AuthorizationInfo.password = password;

            return appUserService.Authorization(Language, AuthorizationInfo);
        }



        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        [Route("/api/appuser/authorizationnew")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationResDto>> AuthorizationNew(AuthorizationReqDto req)
        {
            return appUserService.Authorization(Language, req);
        }

        /// <summary>
        /// 查看数据
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checkuserinfo")]
        [HttpGet]
        public async Task<ResultModel<string>> check()
        {
            ResultModel<string> resultModel = new ResultModel<string>();


            resultModel.data = "当前用户为：" + UserId;
            return resultModel;
        }


        /// <summary>
        /// 查看数据
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checkuserinfonew")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<string>> checknew()
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.data = "当前用户为：" + UserId;
            return resultModel;
        }
    }
}