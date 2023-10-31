using Kogel.Dapper.Extension.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.App;

namespace IService.App
{
    /// <summary>
    /// 植物
    /// </summary>
    public interface IPlantService
    {
        /// <summary>
        /// 植物科属
        /// </summary>
        /// <param name="plantFamilyListReqDto">植物科属请求类</param>
        /// <returns></returns>
        ResultModel<PageList<PlantFamilyListResDto>> GetPlantFamily(PlantFamilyListReqDto plantFamilyListReqDto);


        /// <summary>
        /// 植物列表
        /// </summary>
        /// <param name="plantListReqDto">植物列表请求类</param>
        /// <returns></returns>
        ResultModel<PageList<PlantListResDto>> GetPlantList(PlantListReqDto plantListReqDto);

        /// <summary>
        /// 植物详情
        /// </summary>
        /// <param name="Id">植物编号</param>
        /// <returns></returns>
        ResultModel<PlantInfoResDto> GetPlantInfo(int Id);

        /// <summary>
        /// 植物搜索
        /// </summary>
        /// <param name="plantListSearchReqDto">植物搜索请求类</param>
        /// <returns></returns>
        ResultModel<PageList<PlantListResDto>> GetSearch(PlantListSearchReqDto plantListSearchReqDto);

        /// <summary>
        /// 植物市场规格关系
        /// </summary>
        /// <param name="Id">植物编号</param>
        /// <returns></returns>
        ResultModel<AreaInfoListResDto> GetAreaInfoByPlantId(int Id);

        /// <summary>
        /// 默认植物市场规格关系
        /// </summary>
        /// <returns></returns>
        ResultModels<AreaInfoListResDto> GetAreaInfoByDefaultPlantId();

        /// <summary>
        /// 获取植物市场规格的价格
        /// </summary>
        /// <param name="priceid">植物市场规格对应编号</param>
        /// <param name="httpContext">请求参数</param>
        /// <returns></returns>
        ResultModel<GetPriceResDto> GetPriceByMid(string priceid, HttpContext httpContext);
    }
}
