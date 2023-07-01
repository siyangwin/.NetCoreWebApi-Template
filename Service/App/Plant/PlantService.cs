﻿using IService;
using IService.App;
using Kogel.Dapper.Extension.Model;
using Model.EnumModel;
using Model.View;
using Model.View.Plant;
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
    public class PlantService : IPlantService
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
            .OrderByDescing(s => s.CountNum)
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
                PlantPictures = x.PlantPictures
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
            .Where(s => s.Id == Id)
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



        /// <summary>
        /// 植物搜索
        /// </summary>
        /// <param name="plantListSearchReqDto">植物搜索请求类</param>
        /// <returns></returns>
        public ResultModel<PageList<PlantListResDto>> GetSearch(PlantListSearchReqDto plantListSearchReqDto)
        {
            ResultModel<PageList<PlantListResDto>> resultModel = new ResultModel<PageList<PlantListResDto>>();

            var PlantListResDto = connection.QuerySet<vm_app_PlantList>()
            .Where(s => s.PlantName.Contains(plantListSearchReqDto.Content))
            .OrderBy(s => s.PlantName)
            .PageList(plantListSearchReqDto.PageIndex, plantListSearchReqDto.PageSize, x => new PlantListResDto()
            {
                Id = x.Id,
                PlantName = x.PlantName,
                PlantPictures = x.PlantPictures
            });

            resultModel.data = PlantListResDto;

            return resultModel;
        }

        /// <summary>
        /// 植物市场规格关系
        /// </summary>
        /// <param name="Id">植物编号</param>
        /// <returns></returns>
        public ResultModel<AreaInfoListResDto> GetAreaInfoByPlantId(int Id)
        {
            ResultModel<AreaInfoListResDto> resultModels = new ResultModel<AreaInfoListResDto>();

            var vm_app_AreaInfoByPlantRes = connection.QuerySet<vm_app_AreaInfoByPlant>()
           .Where(s => s.Id == Id)
           .ToList();

            if (vm_app_AreaInfoByPlantRes != null && vm_app_AreaInfoByPlantRes.Count > 0)
            {
                AreaInfoListResDto areaInfoListResDto = new AreaInfoListResDto();

                var PlantInfo = vm_app_AreaInfoByPlantRes.Select(s => new { s.PlantName, s.PlantPictures }).Distinct().FirstOrDefault();
                areaInfoListResDto.PlantName = PlantInfo.PlantName;
                areaInfoListResDto.PlantPictures = PlantInfo.PlantPictures;

                //创建接受药市List
                List<AreaInfoListsResDtos> areaInfoListsResDtos = new List<AreaInfoListsResDtos>();
                var AreaListInfo = vm_app_AreaInfoByPlantRes.Select(s => new { s.AreaId, s.AreaName }).Distinct().ToList();

                foreach (var item in AreaListInfo)
                {
                    AreaInfoListsResDtos areaInfoListsResDtoss = new AreaInfoListsResDtos();
                    areaInfoListsResDtoss.AreaName = item.AreaName;

                    var SpecListInfo = vm_app_AreaInfoByPlantRes.Where(s => s.AreaId == item.AreaId && s.AreaName == item.AreaName).Select(s => new { s.SpecId, s.SpecName, s.PriceId }).Distinct().ToList();

                    //创建接受规格List
                    List<AreaInfoSpecList> SpecsInfo = new List<AreaInfoSpecList>();

                    foreach (var items in SpecListInfo)
                    {
                        AreaInfoSpecList SpecInfo = new AreaInfoSpecList();
                        SpecInfo.SpecName = items.SpecName;
                        SpecInfo.PriceId = items.PriceId;
                        SpecsInfo.Add(SpecInfo);
                    }

                    areaInfoListsResDtoss.Spec = SpecsInfo;

                    areaInfoListsResDtos.Add(areaInfoListsResDtoss);

                }

                areaInfoListResDto.areaInfoListsResDtos = areaInfoListsResDtos;


                resultModels.data = areaInfoListResDto;
            }

            return resultModels;
        }



        /// <summary>
        /// 默认植物市场规格关系
        /// </summary>
        /// <returns></returns>
        public ResultModels<AreaInfoListResDto> GetAreaInfoByDefaultPlantId()
        {
            ResultModels<AreaInfoListResDto> resultModels = new ResultModels<AreaInfoListResDto>();

            var vm_app_AreaInfoByPlant = connection.QuerySet<vm_app_AreaInfoByDefaultPlant>()
          .ToList();

            if (vm_app_AreaInfoByPlant != null && vm_app_AreaInfoByPlant.Count > 0)
            {
                List<AreaInfoListResDto> areaInfoListResDtos = new List<AreaInfoListResDto>();

               var Plant= vm_app_AreaInfoByPlant.GroupBy(S => S.Id).ToList();
                foreach (var AreaInfoitem in Plant)
                {
                    AreaInfoListResDto areaInfoListResDto = new AreaInfoListResDto();

                    var vm_app_AreaInfoByPlantRes = vm_app_AreaInfoByPlant.Where(s => s.Id == AreaInfoitem.Key).ToList();

                    var PlantInfo = vm_app_AreaInfoByPlantRes.Select(s => new { s.PlantName, s.PlantPictures }).Distinct().FirstOrDefault();
                    areaInfoListResDto.PlantName = PlantInfo.PlantName;
                    areaInfoListResDto.PlantPictures = PlantInfo.PlantPictures;

                    //创建接受药市List
                    List<AreaInfoListsResDtos> areaInfoListsResDtos = new List<AreaInfoListsResDtos>();
                    var AreaListInfo = vm_app_AreaInfoByPlantRes.Select(s => new { s.AreaId, s.AreaName }).Distinct().ToList();

                    foreach (var item in AreaListInfo)
                    {
                        AreaInfoListsResDtos areaInfoListsResDtoss = new AreaInfoListsResDtos();
                        areaInfoListsResDtoss.AreaName = item.AreaName;

                        var SpecListInfo = vm_app_AreaInfoByPlantRes.Where(s => s.AreaId == item.AreaId && s.AreaName == item.AreaName).Select(s => new { s.SpecId, s.SpecName, s.PriceId }).Distinct().ToList();

                        //创建接受规格List
                        List<AreaInfoSpecList> SpecsInfo = new List<AreaInfoSpecList>();

                        foreach (var items in SpecListInfo)
                        {
                            AreaInfoSpecList SpecInfo = new AreaInfoSpecList();
                            SpecInfo.SpecName = items.SpecName;
                            SpecInfo.PriceId = items.PriceId;
                            SpecsInfo.Add(SpecInfo);
                        }

                        areaInfoListsResDtoss.Spec = SpecsInfo;

                        areaInfoListsResDtos.Add(areaInfoListsResDtoss);

                    }

                    areaInfoListResDto.areaInfoListsResDtos = areaInfoListsResDtos;

                    areaInfoListResDtos.Add(areaInfoListResDto);
                }
                resultModels.data = areaInfoListResDtos;
            }

            return resultModels;;
        }
    }
}
