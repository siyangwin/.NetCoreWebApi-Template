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
    public interface IAppUserService : IBaseService
    {
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="language">CN:1  EN:2</param>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        ResultModel<AuthorizationResDto> Authorization(LanguageEnum language, AuthorizationReqDto AuthorizationInfo);
    }
}
