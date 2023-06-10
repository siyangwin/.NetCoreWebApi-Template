using IService;
using IService.App;
using Kogel.Dapper.Extension.Model;
using Model.EnumModel;
using Model.View;
using MvcCore.Extension.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.App;

namespace Service.App
{
    /// <summary>
    /// 植物
    /// </summary>
    public class PlantService: IPlantService
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly IRepository connection;


        public PlantService(IRepository connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 植物科属
        /// </summary>
        /// <param name="plantFamilyListReqDto">植物科属请求类</param>
        /// <returns></returns>
        public ResultModel<PageList<PlantFamilyListResDto>> GetPlantFamily(PlantFamilyListReqDto plantFamilyListReqDto)
        {
            ResultModel<PageList<PlantFamilyListResDto>> resultModel = new ResultModel<PageList<PlantFamilyListResDto>>();

            var PlantFamilyListResDto = connection.QuerySet<vm_app_PlantFamily>()
            .OrderBy(s => s.NameCN)
            .PageList(plantFamilyListReqDto.PageIndex, plantFamilyListReqDto.PageSize, x => new PlantFamilyListResDto()
            {
                Id = x.Id,
                Name = x.NameCN
            });


            resultModel.data = PlantFamilyListResDto;

            return resultModel;

        }


        /// <summary>
        /// 植物列表
        /// </summary>
        /// <param name="plantListReqDto">植物列表请求类</param>
        /// <returns></returns>
        public ResultModel<PageList<PlantListResDto>> GetPlantList(PlantListReqDto plantListReqDto)
        {
            ResultModel<PageList<PlantListResDto>> resultModel = new ResultModel<PageList<PlantListResDto>>();

            var PlantListResDto = connection.QuerySet<vm_app_PlantList>()
            .Where(s => s.PlantFamilyId == plantListReqDto.PlantFamilyId)
            .OrderBy(s => s.PlantName)
            .PageList(plantListReqDto.PageIndex, plantListReqDto.PageSize, x => new PlantListResDto()
            {
                Id = x.Id,
                PlantName = x.PlantName,
                PlantPictures=x.PlantPictures
            });

            resultModel.data = PlantListResDto;

            return resultModel;
        }

        /// <summary>
        /// 植物详情
        /// </summary>
        /// <param name="Id">植物编号</param>
        /// <returns></returns>
        public ResultModel<PlantInfoResDto> GetPlantInfo(int Id)
        {
            ResultModel<PlantInfoResDto> resultModel = new ResultModel<PlantInfoResDto>();

            var PlantInfoResDto = connection.QuerySet<vm_app_PlantInfo>()
            .Where(s=>s.Id==Id)
            .Get(x => new PlantInfoResDto()
            {
                NameCN = x.NameCN,
                Provenance = x.Provenance,
                NamePY = x.NamePY,
                NameEN = x.NameEN,
                Alias = x.Alias,
                Source = x.Source,
                OriginalForm = x.OriginalForm,
                HabitatDistribution = x.HabitatDistribution,
                Cultivation = x.Cultivation,
                Character = x.Character,
                ChemicalComposition = x.ChemicalComposition,
                Identify = x.Identify,
                Role = x.Role,
                Preparation = x.Preparation,
                Taste = x.Taste,
                Attribution = x.Attribution,
                Efficacy = x.Efficacy,
                Dosage = x.Dosage,
                Notice = x.Notice,
                SupplementaryFormula = x.SupplementaryFormula,
                Discussions = x.Discussions,
                ClinicalApplication = x.ClinicalApplication
            });

            resultModel.data = PlantInfoResDto;

            return resultModel;
        }
    }
}
