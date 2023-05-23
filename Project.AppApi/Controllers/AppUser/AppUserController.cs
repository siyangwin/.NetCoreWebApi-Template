using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MvcCore.Extension.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
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
        private readonly GenerateJwt generateJwt;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="generateJwt">Jwt认证</param>
        public AppUserController(GenerateJwt generateJwt)
        {
            this.generateJwt = generateJwt;
        }


        /// <summary>
        /// 获取授权
        /// </summary>
        /// <param name="userid">用户编号</param>
        /// <returns></returns>
        [Route("/api/appuser/authorization")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationTokenResDto>> login(int userid)
        {
            ResultModel<AuthorizationTokenResDto> resultModel = new ResultModel<AuthorizationTokenResDto>();
            AuthorizationTokenResDto authorizationTokenResDto = new AuthorizationTokenResDto();
            authorizationTokenResDto.Token = Guid.NewGuid().ToString();

            //寫入身份信息到認證中心
            var claims = new[]
            {
                new Claim("UserId",1.ToString()),
                new Claim("Token",authorizationTokenResDto.Token)
            };

            var refreshToken = Guid.NewGuid().ToString();

            var jwtTokenResult = generateJwt.GenerateEncodedTokenAsync(userid.ToString(), userid);

            #region 故意制造错误
            ////故意制造错误
            //try
            //{
            //int a = 1; int b = 0;
            //int c = a / b;
            //}
            //catch (Exception ex)
            //{
            //    //写入错误日志
            //    await systemLogService.LogAdd(ex);
            //}
            #endregion

            authorizationTokenResDto.Token = jwtTokenResult;

            resultModel.data = authorizationTokenResDto;
            return resultModel;
        }


        /// <summary>
        /// 查看数据
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checkuserinfo")]
        [HttpGet]
        public async Task<ResultModel<AuthorizationTokenResDto>> check()
        {
            ResultModel<AuthorizationTokenResDto> resultModel = new ResultModel<AuthorizationTokenResDto>();
            AuthorizationTokenResDto authorizationTokenResDto = new AuthorizationTokenResDto();

            var auth = Token.Split(" ")[1];
            var jwtArr = auth.Split(".");
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[1]));

            string Userid = string.IsNullOrEmpty(dic["userid"]) ? "1" : dic["userid"];
            authorizationTokenResDto.Token = "校验通过:" + Userid;
            resultModel.data = authorizationTokenResDto;
            return resultModel;
        }


        /// <summary>
        /// 查看数据
        /// </summary>
        /// <returns></returns>
        [Route("/api/appuser/checkuserinfonew")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<AuthorizationTokenResDto>> checknew()
        {
            ResultModel<AuthorizationTokenResDto> resultModel = new ResultModel<AuthorizationTokenResDto>();
            AuthorizationTokenResDto authorizationTokenResDto = new AuthorizationTokenResDto();

            var auth = Token.Split(" ")[1];
            var jwtArr = auth.Split(".");
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(Base64UrlEncoder.Decode(jwtArr[1]));

            string Userid = string.IsNullOrEmpty(dic["userid"]) ? "1" : dic["userid"];
            authorizationTokenResDto.Token = "校验通过:" + Userid;
            resultModel.data = authorizationTokenResDto;
            return resultModel;
        }
    }
}