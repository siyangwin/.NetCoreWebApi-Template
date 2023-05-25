﻿using IService;
using IService.App;
using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using MvcCore.Extension.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.App;

namespace Service.App
{
    /// <summary>
    /// 用户操作
    /// </summary>
    public class AppUserService : IAppUserService
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly IRepository connection;

        /// <summary>
        /// Jwt工具类
        /// </summary>
        private readonly GenerateJwt generateJwt;

        public AppUserService(IRepository connection, GenerateJwt generateJwt)
        {
            this.connection = connection;
            this.generateJwt = generateJwt;
        }


        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="language">CN:1  EN:2</param>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        public ResultModel<AuthorizationResDto> Authorization(LanguageEnum language, AuthorizationReqDto AuthorizationInfo)
        {
            //定义返回类
            ResultModel<AuthorizationResDto> resultModel = new ResultModel<AuthorizationResDto>();

            //定义返回数据类
            AuthorizationResDto authorizationTokenResDto = new AuthorizationResDto();

            if (AuthorizationInfo.account!="123" || AuthorizationInfo.password!="admin")
            {
                resultModel.success = false;
                resultModel.message = "用戶帳戶或密碼無效";
                return resultModel;
            }

            //检查数据库核对用户授权信息
            int UserId = Convert.ToInt32(AuthorizationInfo.account);

            //生成授权信息
            authorizationTokenResDto.Authorization = generateJwt.GenerateEncodedTokenAsync(UserId);

            //写入缓存

            //写入数据库

            resultModel.data = authorizationTokenResDto;
            return resultModel;
        }
    }
}
