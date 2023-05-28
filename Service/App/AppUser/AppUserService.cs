using IService;
using IService.App;
using Model.EnumModel;
using Model.Table;
using MvcCore.Extension.Auth;
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

        public AppUserService(IRepository connection, GenerateJwt generateJwt)
        {
            this.connection = connection;
            this.generateJwt = generateJwt;
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
                Console.WriteLine(item.productId + ":" + item.bookName);
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
                                    message += "中国药典,缺少:" + items[0];
                                    break;
                            }
                            //按items[0]的名称分配数据库字段
                            //Console.WriteLine(items[0] + ":" + items[1]);
                        }
                        chinesePharmacopoeia.CreateUser = "SiYang";
                        //写入数据库
                        bool chinesePharmacopoeiares = connection.CommandSet<ChinesePharmacopoeia>().InsertIdentity(chinesePharmacopoeia)>1;
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
                                case "性状":
                                    dictionary.Character = items[1];
                                    break;
                                case "化学成分":
                                    dictionary.ChemicalComposition = items[1];
                                    break;
                                case "作用":
                                    dictionary.Role = items[1];
                                    break;
                                case "炮制":
                                    dictionary.Preparation = items[1];
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
                                    message += "辞典,缺少:" + items[0];
                                    break;
                             }
                            //按items[0]的名称分配数据库字段
                             //Console.WriteLine(items[0] + ":" + items[1]);
                         }
                        dictionary.CreateUser = "SiYang";
                        //写入数据库
                        bool dictionaryres = connection.CommandSet<Dictionary>().InsertIdentity(dictionary) > 1;
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
                                    message += "中华本草,缺少:" + items[0];
                                    break;
                            }
                            //按items[0]的名称分配数据库字段
                            //Console.WriteLine(items[0] + ":" + items[1]);
                        }
                        chineseMateriaMedica.CreateUser = "SiYang";
                        //写入数据库
                        bool chineseMateriaMedicares = connection.CommandSet<ChineseMateriaMedica>().InsertIdentity(chineseMateriaMedica) > 1;
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

            resultModel.success = success > 0;
            resultModel.message = "传入数量：" + Req.Count() + ",写入成功数据：" + success+",备注:"+message;
            return resultModel;
        }
    }
}
