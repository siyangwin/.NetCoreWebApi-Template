using Core;
using IService;
using IService.App;
using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using Model.Table;
using Model.View;
using MvcCore.Extension.Auth;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using ViewModel;
using ViewModel.App;

namespace Service.App
{
    /// <summary>
    /// 用户操作
    /// </summary>
    public class AppUserService : IAppUserService
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly IRepository connection;

        /// <summary>
        /// Jwt工具类
        /// </summary>
        private readonly GenerateJwt generateJwt;

        private readonly HttpHelper httpHelper; //API请求

        public AppUserService(IRepository connection, GenerateJwt generateJwt, HttpHelper httpHelper)
        {
            this.connection = connection;
            this.generateJwt = generateJwt;
            this.httpHelper = httpHelper;
        }


        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="language">CN:1  EN:2</param>
        /// <param name="AuthorizationInfo">授权信息</param>
        /// <returns></returns>
        public ResultModel<AuthorizationResDto> Authorization(LanguageEnum language, AuthorizationReqDto AuthorizationInfo)
        {
            //定义返回类
            ResultModel<AuthorizationResDto> resultModel = new ResultModel<AuthorizationResDto>();

            //定义返回数据类
            AuthorizationResDto authorizationTokenResDto = new AuthorizationResDto();

            if (AuthorizationInfo.account!="123" || AuthorizationInfo.password!="admin")
            {
                resultModel.success = false;
                resultModel.message = "用戶帳戶或密碼無效";
                return resultModel;
            }

            //检查数据库核对用户授权信息
            int UserId = Convert.ToInt32(AuthorizationInfo.account);

            //生成授权信息
            authorizationTokenResDto.Authorization = generateJwt.GenerateEncodedTokenAsync(UserId);

            //写入缓存

            //写入数据库

            resultModel.data = authorizationTokenResDto;
            return resultModel;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <returns></returns>
        public ResultModel ProductInsert(LanguageEnum language, List<ProductInfoReqDto> Req)
        {
            ResultModel resultModel = new ResultModel();
            int success = 0;
            string message = "";
            foreach (var item in Req)
            {
                //Console.WriteLine(item.productId + ":" + item.bookName);
                switch (item.bookName) {
                    case "中国药典":
                        //创建数据库类,写入数据
                        ChinesePharmacopoeia chinesePharmacopoeia = new ChinesePharmacopoeia();
                        chinesePharmacopoeia.Pid = item.productId;
                        foreach (var items in item.detail)
                        {
                            switch (items[0]){
                                case "名称":
                                    chinesePharmacopoeia.NameCN = items[1];
                                    break;
                                case "拼音名":
                                    chinesePharmacopoeia.NamePY = items[1];
                                    break;
                                case "英文名":
                                    chinesePharmacopoeia.NameEN = items[1];
                                    break;
                                case "别名":
                                    chinesePharmacopoeia.Alias = items[1];
                                    break;
                                case "来源":
                                    chinesePharmacopoeia.Source = items[1];
                                    break;
                                case "性状":
                                    chinesePharmacopoeia.Character = items[1];
                                    break;
                                case "鉴别":
                                    chinesePharmacopoeia.Identify = items[1];
                                    break;
                                case "含量测定":
                                    chinesePharmacopoeia.ContentDetermination = items[1];
                                    break;
                                case "炮制":
                                    chinesePharmacopoeia.Preparation = items[1];
                                    break;
                                case "制法":
                                    chinesePharmacopoeia.PreparationMethod= items[1];
                                    break;
                                case "性味":
                                    chinesePharmacopoeia.Taste = items[1];
                                    break;
                                case "归经":
                                    chinesePharmacopoeia.Attribution = items[1];
                                    break;
                                case "功效":
                                    chinesePharmacopoeia.Efficacy = items[1];
                                    break;
                                case "用法用量":
                                    chinesePharmacopoeia.Dosage = items[1];
                                    break;
                                case "储藏":
                                    chinesePharmacopoeia.Storage = items[1];
                                    break;
                                case "备注":
                                    chinesePharmacopoeia.Remark = items[1];
                                    break;
                                case "摘录出处":
                                    chinesePharmacopoeia.ExcerptSource = items[1];
                                    break;
                                default:
                                    //可以记录缺少的字段
                                    message += "《中国药典》,缺少:" + items[0] + ";";
                                    break;
                            }
                            //按items[0]的名称分配数据库字段
                            //Console.WriteLine(items[0] + ":" + items[1]);
                        }
                        chinesePharmacopoeia.CreateUser = "SiYang";
                        //写入数据库
                        bool chinesePharmacopoeiares = connection.CommandSet<ChinesePharmacopoeia>().InsertIdentity(chinesePharmacopoeia)>0;
                        if (chinesePharmacopoeiares)
                        {
                            success++;
                        }
                        break;
                    case "辞典":
                        //创建数据库类,写入数据
                        Dictionary dictionary = new Dictionary();
                        dictionary.Pid = item.productId;
                        foreach (var items in item.detail)
                        {
                            switch (items[0])
                            {
                                case "名称":
                                    dictionary.NameCN = items[1];
                                    break;
                                case "拼音名":
                                    dictionary.NamePY = items[1];
                                    break;
                                case "英文名":
                                    dictionary.NameEN = items[1];
                                    break;
                                case "出处":
                                    dictionary.Provenance = items[1];
                                    break;
                                case "别名":
                                    dictionary.Alias = items[1];
                                    break;
                                case "来源":
                                    dictionary.Source = items[1];
                                    break;
                                case "原形态":
                                    dictionary.OriginalForm = items[1];
                                    break;
                                case "生境分布":
                                    dictionary.HabitatDistribution = items[1];
                                    break;
                                case "栽培":
                                    dictionary.Cultivation = items[1];
                                    break;
                                case "性状":
                                    dictionary.Character = items[1];
                                    break;
                                case "化学成分":
                                    dictionary.ChemicalComposition = items[1];
                                    break;
                                case "作用":
                                    dictionary.Role = items[1];
                                    break;
                                case "毒性":
                                    dictionary.Toxicity = items[1];
                                    break;
                                case "炮制":
                                    dictionary.Preparation = items[1];
                                    break;
                                case "制法":
                                    dictionary.PreparationMethod = items[1];
                                    break;
                                case "性味":
                                    dictionary.Taste = items[1];
                                    break;
                                case "归经":
                                    dictionary.Attribution = items[1];
                                    break;
                                case "功效":
                                    dictionary.Efficacy = items[1];
                                    break;
                                case "用法用量":
                                    dictionary.Dosage = items[1];
                                    break;
                                case "注意":
                                    dictionary.Notice = items[1];
                                    break;
                                case "附方":
                                    dictionary.SupplementaryFormula = items[1];
                                    break;
                                case "各家论述":
                                    dictionary.Discussions = items[1];
                                    break;
                                case "临床应用":
                                    dictionary.ClinicalApplication = items[1];
                                    break;
                                case "备注":
                                    dictionary.Remark = items[1];
                                    break;
                                case "摘录出处":
                                    dictionary.ExcerptSource = items[1];
                                    break;
                                default:
                                    //可以记录缺少的字段
                                    message += "《辞典》,缺少:" + items[0] + ";";
                                    break;
                             }
                            //按items[0]的名称分配数据库字段
                             //Console.WriteLine(items[0] + ":" + items[1]);
                         }
                
                        dictionary.CreateUser = "SiYang";
                        //写入数据库
                        bool dictionaryres = connection.CommandSet<Dictionary>().InsertIdentity(dictionary) > 0;
                        if (dictionaryres)
                        {
                            success++;
                        }
                        break;
                    case "中华本草":
                        //创建数据库类,写入数据
                        ChineseMateriaMedica chineseMateriaMedica = new ChineseMateriaMedica();
                        chineseMateriaMedica.Pid = item.productId;
                        foreach (var items in item.detail)
                        {
                            switch (items[0])
                            {
                                case "名称":
                                    chineseMateriaMedica.NameCN = items[1];
                                    break;
                                case "拼音名":
                                    chineseMateriaMedica.NamePY = items[1];
                                    break;
                                case "英文名":
                                    chineseMateriaMedica.NameEN = items[1];
                                    break;
                                case "出处":
                                    chineseMateriaMedica.Provenance = items[1];
                                    break;
                                case "别名":
                                    chineseMateriaMedica.Alias = items[1];
                                    break;
                                case "来源":
                                    chineseMateriaMedica.Source = items[1];
                                    break;
                                case "原形态":
                                    chineseMateriaMedica.OriginalForm = items[1];
                                    break;
                                case "生境分布":
                                    chineseMateriaMedica.HabitatDistribution = items[1];
                                    break;
                                case "栽培":
                                    chineseMateriaMedica.Cultivation = items[1];
                                    break;
                                case "性状":
                                    chineseMateriaMedica.Character = items[1];
                                    break;
                                case "化学成分":
                                    chineseMateriaMedica.ChemicalComposition = items[1];
                                    break;
                                case "作用":
                                    chineseMateriaMedica.Role = items[1];
                                    break;
                                case "毒性":
                                    chineseMateriaMedica.Toxicity = items[1];
                                    break;
                                case "鉴别":
                                    chineseMateriaMedica.Identify = items[1];
                                    break;
                                case "炮制":
                                    chineseMateriaMedica.Preparation = items[1];
                                    break;
                                case "制法":
                                    chineseMateriaMedica.PreparationMethod = items[1];
                                    break;
                                case "性味":
                                    chineseMateriaMedica.Taste = items[1];
                                    break;
                                case "归经":
                                    chineseMateriaMedica.Attribution = items[1];
                                    break;
                                case "功效":
                                    chineseMateriaMedica.Efficacy = items[1];
                                    break;
                                case "用法用量":
                                    chineseMateriaMedica.Dosage = items[1];
                                    break;
                                case "注意":
                                    chineseMateriaMedica.Notice = items[1];
                                    break;
                                case "附方":
                                    chineseMateriaMedica.SupplementaryFormula = items[1];
                                    break;
                                case "各家论述":
                                    chineseMateriaMedica.Discussions = items[1];
                                    break;
                                case "临床应用":
                                    chineseMateriaMedica.ClinicalApplication = items[1];
                                    break;
                                case "备注":
                                    chineseMateriaMedica.Remark = items[1];
                                    break;
                                case "摘录出处":
                                    chineseMateriaMedica.ExcerptSource = items[1];
                                    break;
                                default:
                                    //可以记录缺少的字段
                                    message += "《中华本草》,缺少:" + items[0]+";";
                                    break;
                            }
                            //按items[0]的名称分配数据库字段
                            //Console.WriteLine(items[0] + ":" + items[1]);
                        }
                        chineseMateriaMedica.CreateUser = "SiYang";
                        //写入数据库
                        bool chineseMateriaMedicares = connection.CommandSet<ChineseMateriaMedica>().InsertIdentity(chineseMateriaMedica) > 0;
                        if (chineseMateriaMedicares)
                        {
                            success++;
                        }
                        break; 
                    default:
                        break;
                }

                //换下一个
                //Console.WriteLine("-----------------------------------------换下一个------------------------------------------");
            }

            if (Req.Count()==0)
            {
                resultModel.success = true;
            }
            else
            {
                resultModel.success = success > 0;
            }

            resultModel.message = "传入数量：" + Req.Count() + ",写入成功数据：" + success+",备注:"+message;
            return resultModel;
        }


        /// <summary>
        /// 写入产品名称数据
        /// </summary>
        /// <returns></returns>
        public ResultModel ProductNameInsert(LanguageEnum language, List<ProductReqDto> Req)
        {
            ResultModel resultModel = new ResultModel();
            int success = 0;
            string message = "";
            foreach (var item in Req)
            {
                Product product = new Product();
                product.ProductId = item.Id;
                product.ProductName = item.Name;
                product.CreateUser = "SiYang";

                //写入数据库
                bool productNum = connection.CommandSet<Product>().InsertIdentity(product) > 0;
                if (productNum)
                {
                    success++;
                }
            }

            if (Req.Count() == 0)
            {
                resultModel.success = true;
            }
            else
            {
                resultModel.success = success > 0;
            }

            resultModel.message = "传入数量：" + Req.Count() + ",写入成功数据：" + success;
            return resultModel;
        }


        /// <summary>
        /// 获取小程序用户编号
        /// </summary>
        /// <param name="getUserOpenIdResDto">请求类</param>
        /// <param name="httpContext">请求数据</param>
        /// <returns></returns>
        public ResultModel<GetUserOpenIdResDto> GetUserOpenId(GetUserOpenIdReqDto getUserOpenIdResDto, HttpContext httpContext)
        {
            ResultModel<GetUserOpenIdResDto> resultModel = new ResultModel<GetUserOpenIdResDto>();
            //OpenId
            string openid = "";
            //Wechat
            if (getUserOpenIdResDto.Type == 1)
            {
                string Url = "https://api.weixin.qq.com/sns/jscode2session";
                string appid = "wxc80fb977dc7e9e0e";
                string secret = "d123a8aaffdba053f07f732272239260";

                Url += "?appid=" + appid + "&secret=" + secret + "&js_code="+ getUserOpenIdResDto.Code + "&grant_type=authorization_code";

                //調用接口註冊
                var httpResult = httpHelper.GetHtml(new HttpItem()
                {
                    URL = Url,
                    Method = "get",
                    PostDataType = PostDataType.String,
                    ContentType = "application/json",
                    Timeout = 1800000
                }, httpContext);

                if (httpResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(httpResult.Html))
                    {
                        var UserInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpResult.Html);

                        if (!UserInfo.ContainsKey("openid"))
                        {
                            resultModel.success = false;
                            return resultModel;
                        }
                        openid = UserInfo["openid"];
                    }
                    else
                    {
                        resultModel.success = false;
                        return resultModel;
                    }
                }
                else
                {
                    resultModel.success = false;
                    return resultModel;
                }
            }
            else
            {
                //TikTok
                string Url = "https://open-sandbox.douyin.com/api/apps/v2/jscode2session";
                string appid = "tt991eefa30622bebb01";
                string secret = "0cad3606b9ee7d0fc4a4bff041a80783451503de";

                var req = new
                {
                    appid = appid,
                    secret = secret,
                    code= getUserOpenIdResDto.Code
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
                        TikTokCode2SessionResDto tikTokCode2SessionResDto= JsonConvert.DeserializeObject<TikTokCode2SessionResDto>(httpResult.Html);

                        if (string.IsNullOrEmpty(tikTokCode2SessionResDto.TikTokCode2SessionData.openid))
                        {
                            resultModel.success = false;
                            return resultModel;
                        }

                        openid = tikTokCode2SessionResDto.TikTokCode2SessionData.openid;
                    }
                    else
                    {
                        resultModel.success = false;
                        return resultModel;
                    }
                }
                else
                {
                    resultModel.success = false;
                    return resultModel;
                }
            }

            var vm_app_UserInfo = connection.QuerySet<vm_app_UserInfo>()
                .WhereIf(getUserOpenIdResDto.Type == 1,s=>s.WechatOpenid==openid,s=>s.TikTokOpenid== openid)
                .Get();

            GetUserOpenIdResDto getUserOpenIdReq = new GetUserOpenIdResDto();
            //查询用户是否存在
            if (vm_app_UserInfo!=null)
            {
                getUserOpenIdReq.Token = openid;
                getUserOpenIdReq.Avatar = vm_app_UserInfo.Avatar;
                getUserOpenIdReq.Name = vm_app_UserInfo.Name;
                getUserOpenIdReq.IsNewUser = false;

                resultModel.data = getUserOpenIdReq;
                return resultModel;
            }

            //查询数据库存在就Return


            //随机生成头像
            Random AvatarRandom = new Random();
            int randomNumber = AvatarRandom.Next(1, 21); // 生成 1-20 之间的随机整数

            //随机生成姓名
            Random NameRandom = new Random();
            StringBuilder nameBuilder = new StringBuilder();

            string letters = "abcdefghijklmnopqrstuvwxyz"; // 字母表

            for (int i = 0; i < 10; i++)
            {
                int index = NameRandom.Next(letters.Length);
                char randomChar = letters[index];
                nameBuilder.Append(randomChar);
            }

            //写入数据库
            UserInfo userInfo = new UserInfo();
            userInfo.Avatar = randomNumber.ToString() + ".png";
            userInfo.Name = nameBuilder.ToString();
            if (getUserOpenIdResDto.Type == 1)
            {
                userInfo.WechatOpenid = openid;
                userInfo.TikTokOpenid = "";
            }
            else
            {
                userInfo.WechatOpenid = "";
                userInfo.TikTokOpenid = openid;
            }

            userInfo.CreateTime = DateTime.Now;
            userInfo.CreateUser = "System";
            //不存在就新增
            //写入数据库
            bool userInfoNum = connection.CommandSet<UserInfo>().Insert(userInfo) > 0;

            //返回信息
            if (userInfoNum)
            {
                getUserOpenIdReq.Token = openid;
                getUserOpenIdReq.Avatar = "https://wechat.xunfan.ltd/other/pic/"+ randomNumber.ToString() + ".png";
                getUserOpenIdReq.Name = nameBuilder.ToString();
                getUserOpenIdReq.IsNewUser = true;
                resultModel.data = getUserOpenIdReq;
            }
            else
            {
                resultModel.success = false;
                resultModel.message = "获取授权失败";
            }

            return resultModel;
        }



        /// <summary>
        ///修改用户信息
        /// </summary>
        /// <param name="changeUserInfoResDto"></param>
        /// <returns></returns>
        public ResultModel ChangeUserInfo(ChangeUserInfoReqDto changeUserInfoResDto)
        {
            ResultModel resultModel = new ResultModel();

            var vm_app_UserInfo = connection.QuerySet<vm_app_UserInfo>()
            .Where(s => s.WechatOpenid == changeUserInfoResDto.Token || s.TikTokOpenid == changeUserInfoResDto.Token)
            .Get();

            //查询用户是否存在
            if (vm_app_UserInfo == null)
            {
                resultModel.success = false;
                resultModel.message = "用户不存在";
                return resultModel;
            }


            bool userInfoNum = connection.CommandSet<UserInfo>()
                  .Where(x => x.IsDelete == false && x.Id == vm_app_UserInfo.Id)
                  .Update(x => new UserInfo
                  {
                     Avatar = changeUserInfoResDto.Avatar+".png",
                     Name= changeUserInfoResDto.Name,
                     UpdateTime = DateTime.Now,
                     UpdateUser = vm_app_UserInfo.Id.ToString()
                  })> 0;

            if (!userInfoNum)
            {
                resultModel.success = false;
                resultModel.message = "编辑失败";
            }
            else
            {
                resultModel.message = "编辑成功";
            }

            return resultModel;
        }





        /// <summary>
        /// 查看头像信息
        /// </summary>
        /// <param name="token">用户信息</param>
        /// <returns></returns>
        public ResultModels<GetAvatarListResDto> GetAvatarList(string token)
        {
            ResultModels<GetAvatarListResDto> resultModel = new ResultModels<GetAvatarListResDto>();

           var vm_app_Avatar = connection.QuerySet<vm_app_Avatar>()
           .ToList(x => new GetAvatarListResDto
           {
               Id = x.Id,
               Url = x.Url,
           });

            var vm_app_UserInfo = connection.QuerySet<vm_app_UserInfo>()
            .Where(s => s.WechatOpenid == token || s.TikTokOpenid == token)
            .Get()?.Avatar??"";


            //查询用户是否存在
            if (!string.IsNullOrEmpty(vm_app_UserInfo))
            {
                GetAvatarListResDto getAvatarListResDto =vm_app_Avatar.FirstOrDefault(x => x.Url == vm_app_UserInfo);

                if (getAvatarListResDto != null)
                {
                    // 将找到的元素从列表中移除
                    vm_app_Avatar.Remove(getAvatarListResDto);
                    // 将找到的元素插入到列表的第一个位置
                    vm_app_Avatar.Insert(0, getAvatarListResDto);
                }
            }

            resultModel.data = vm_app_Avatar;
            return resultModel;
        }
    }
}
