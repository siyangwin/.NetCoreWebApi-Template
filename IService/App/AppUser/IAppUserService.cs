using Model.EnumModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.App;
using ViewModel;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;

namespace IService.App
{
    /// <summary>
    /// 用户操作
    /// </summary>
    public interface IAppUserService
    {
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="language">CN:1  EN:2</param>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        ResultModel<AuthorizationResDto> Authorization(LanguageEnum language, AuthorizationReqDto AuthorizationInfo);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <returns></returns>
        ResultModel ProductInsert(LanguageEnum language, List<ProductInfoReqDto> Req);

        /// <summary>
        /// 写入产品名称数据
        /// </summary>
        /// <returns></returns>
        ResultModel ProductNameInsert(LanguageEnum language, List<ProductReqDto> Req);

        /// <summary>
        /// 获取小程序用户编号
        /// </summary>
        /// <param name="getUserOpenIdResDto">请求类</param>
        /// <param name="httpContext">请求数据</param>
        /// <returns></returns>
        ResultModel<GetUserOpenIdResDto> GetUserOpenId(GetUserOpenIdReqDto getUserOpenIdResDto, HttpContext httpContext);

        /// <summary>
        ///修改用户信息
        /// </summary>
        /// <param name="changeUserInfoResDto"></param>
        /// <returns></returns>
        ResultModel ChangeUserInfo(ChangeUserInfoReqDto changeUserInfoResDto);


        /// <summary>
        /// 查看头像信息
        /// </summary>
        /// <param name="token">用户信息</param>
        /// <returns></returns>
        ResultModels<GetAvatarListResDto> GetAvatarList(string token);
    }
}
