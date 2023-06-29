using Core;
using IService;
using IService.App;
using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using Model.Table;
using Model.View;
using MvcCore.Extension.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using ViewModel.App;

namespace Service.App
{
    /// <summary>
    /// 第三方数据同步
    /// </summary>
    public class OtherSystemService: IOtherSystemService
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly IRepository connection;

        private readonly HttpHelper httpHelper; //API请求
        public OtherSystemService(IRepository connection, HttpHelper httpHelper)
        {
            this.connection = connection;
            this.httpHelper = httpHelper;
        }


        /// <summary>
        /// 获取植物市场价格
        /// </summary>
        /// <param name="httpContext">传入参数获取</param>
        /// <returns></returns>
        public ResultModel GetPriceByPlantId(HttpContext httpContext)
        {
            ResultModel resultModel = new ResultModel();

            //开启事务
            connection.UnitOfWork.BeginTransaction(() =>
            {
                //查询本地产品信息
                var Productids = connection.QuerySet<vm_app_ProductList>()
                .OrderBy(s => s.Productid)
                //.Top(10)
                .ToList(s=>s.Productid);

                foreach (var item in Productids)
                {
                    //根据ProductId获取市场信息
                    string Url = "https://www.zyctd.com/Breeds/GetAreaByMBID";
                    var req = new
                    {
                        MBID = item,
                        MAreaTypeID=1
                    };

                    //查询
                    var httpResult = httpHelper.GetHtml(new HttpItem()
                    {
                        URL = Url,
                        Method = "Post",
                        PostDataType = PostDataType.Byte,
                        ContentType = "application/json",
                        PostdataByte = httpHelper.GetPostDate(req),
                        Timeout = 1800000
                    }, httpContext);

                    if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        if (!string.IsNullOrEmpty(httpResult.Html))
                        {
                            GetAreaByMBIDResDto getAreaByMBIDResDto = JsonConvert.DeserializeObject<GetAreaByMBIDResDto>(httpResult.Html);

                            foreach (var Areaitem in getAreaByMBIDResDto.Data)
                            {

                                //根据ProductId获取市场信息
                                string AreaitemUrl = "https://www.zyctd.com/Breeds/GetSpecByAreaID";
                                var Areaitemreq = new
                                {
                                    MBID = item,
                                    MAreaID = Areaitem.MAreaID
                                };

                                //查询
                                var AreaitemhttpResult = httpHelper.GetHtml(new HttpItem()
                                {
                                    URL = AreaitemUrl,
                                    Method = "Post",
                                    PostDataType = PostDataType.Byte,
                                    ContentType = "application/json",
                                    PostdataByte = httpHelper.GetPostDate(Areaitemreq),
                                    Timeout = 1800000
                                }, httpContext);

                                if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    if (!string.IsNullOrEmpty(AreaitemhttpResult.Html))
                                    {
                                        GetSpecByAreaIDResDto getSpecByAreaIDResDto = JsonConvert.DeserializeObject<GetSpecByAreaIDResDto>(AreaitemhttpResult.Html);

                                        foreach (var getSpecByAreaIDResDtoitem in getSpecByAreaIDResDto.Data)
                                        {
                                            PlantPriceTemp plantPriceTemp = new PlantPriceTemp();
                                            plantPriceTemp.MBID = item.ToString();
                                            plantPriceTemp.MAreaID = Areaitem.MAreaID;
                                            plantPriceTemp.MArea = Areaitem.MArea;
                                            plantPriceTemp.MBSID = getSpecByAreaIDResDtoitem.MBSID;
                                            plantPriceTemp.MSpec = getSpecByAreaIDResDtoitem.MSpec;
                                            plantPriceTemp.mid = getSpecByAreaIDResDtoitem.mid;
                                            plantPriceTemp.CreateTime = DateTime.Now;
                                            plantPriceTemp.CreateUser = "System";
                                            //写入数据库
                                            bool productNum = connection.CommandSet<PlantPriceTemp>().Insert(plantPriceTemp) > 0;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    resultModel.success = false;
                                    resultModel.message = "执行失败";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        resultModel.success = false;
                        resultModel.message = "执行失败";
                        return;
                    }
                }
            }, IsolationLevel.ReadCommitted); 
            if (resultModel.success)
            {
                //提交
                resultModel.message = "获取数据成功";
                connection.UnitOfWork.Commit();
            }
            else
            {
                //回滚
                connection.UnitOfWork.Rollback();
            }
            return resultModel;
        }
    }
}
