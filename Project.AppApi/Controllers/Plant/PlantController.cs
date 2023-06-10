using IService.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.App;
using ViewModel.App;
using ViewModel;
using Kogel.Dapper.Extension.Model;

namespace Project.AppApi.Controllers.Plant
{
    /// <summary>
    /// 植物
    /// </summary>
    public class PlantController : BaseController
    {
        private readonly IPlantService plantService;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="plantService">植物类</param>
        public PlantController(IPlantService plantService)
        {
            this.plantService = plantService;
        }

        /// <summary>
        /// 植物科属
        /// </summary>
        /// <param name="plantFamilyListReqDto">植物科属请求类</param>
        /// <returns></returns>
        [Route("/api/plant/family")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel<PageList<PlantFamilyListResDto>>> GetPlantFamilyList(PlantFamilyListReqDto plantFamilyListReqDto)
        {
            return plantService.GetPlantFamily(plantFamilyListReqDto);
        }


        /// <summary>
        /// 植物列表
        /// </summary>
        /// <param name="plantListReqDto">植物列表请求类</param>
        /// <returns></returns>
        [Route("/api/plant/gets")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ResultModel<PageList<PlantListResDto>>> GetPlantList(PlantListReqDto plantListReqDto)
        {
            return plantService.GetPlantList(plantListReqDto);
        }


        /// <summary>
        /// 植物详情
        /// </summary>
        /// <param name="Id">植物编号</param>
        /// <returns></returns>
        [Route("/api/plant/get")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResultModel<PlantInfoResDto>> GetPlantInfo(int Id)
        {
            return plantService.GetPlantInfo(Id);
        }
    }
}
