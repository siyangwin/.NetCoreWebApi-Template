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
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationResDto>> Authorization(AuthorizationReqDto AuthorizationInfo)
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

        /// <summary>
        /// 写入数据-无需授权
        /// </summary>
        /// <param name="Req">请求参数</param>
        /// <returns></returns>
        [Route("/api/appuser/productinsert")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel> ProductInsert(List<ProductInfoReqDto> Req)
        {
            return appUserService.ProductInsert(Language, Req); ;
        }

    }
}