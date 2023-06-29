using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace IService.App
{
    /// <summary>
    /// 第三方数据同步
    /// </summary>
    public interface IOtherSystemService
    {
        /// <summary>
        /// 获取植物市场价格
        /// </summary>
        /// <param name="httpContext">传入参数获取</param>
        /// <returns></returns>
        ResultModel GetPriceByPlantId(HttpContext httpContext);
    }
}
