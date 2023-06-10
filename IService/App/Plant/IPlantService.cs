using Kogel.Dapper.Extension.Model;
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
        public ResultModel<PlantInfoResDto> GetPlantInfo(int Id);
    }
}
