using IService.App;
using Kogel.Dapper.Extension.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.App;
using ViewModel.App;
using ViewModel;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.AppApi.Controllers.OtherSystem
{
    /// <summary>
    /// 第三方同步数据
    /// </summary>
    public class OtherSystemController : BaseController
    {
        private readonly IOtherSystemService otherSystemService;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="otherSystemService"></param>
        public OtherSystemController(IOtherSystemService otherSystemService)
        {
            this.otherSystemService = otherSystemService;
        }

        /// <summary>
        /// 获取植物市场价格
        /// </summary>
        /// <returns></returns>
        [Route("/api/othersystem/getprice")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel> GetPriceByPlantId()
        {
            return otherSystemService.GetPriceByPlantId(HttpContext);
        }
    }
}
