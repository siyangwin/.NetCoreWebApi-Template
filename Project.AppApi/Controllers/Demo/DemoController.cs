using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.EnumModel;
using Project.AppApi.Controllers.Demo;
using Project.Core;
using ViewModel;
using ViewModel.Demo;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// Demo 示范控制器 V1（模板示例，供下载者参考常见写法）
    /// <para>说明：本控制器全部使用模拟数据，不访问数据库；路由统一带 /api/v1 体现 API 版本。</para>
    /// <para>用法：登录获取 token 后，在 Swagger 右上角 Authorize 填入 Bearer {token}，即可访问需要鉴权的接口。</para>
    /// </summary>
    [ApiExplorerSettings(GroupName = "V1")]
    [Route("api/v1/demo")]
    public class DemoController : BaseController
    {
        /// <summary>
        /// 示范1：最简返回（统一返回结构 ResultModel，自带 apiVersion + timestamp）
        /// </summary>
        /// <remarks>演示：ResultModel 统一结构 + SetMessage 链式调用</remarks>
        [HttpGet("hello")]
        [AllowAnonymous]
        public ResultModel Hello()
        {
            return new ResultModel().SetMessage("Hello World！模板演示接口");
        }

        /// <summary>
        /// 示范2：路径参数（从 URL 中取 {id}）
        /// </summary>
        /// <param name="id">编号</param>
        [HttpGet("item/{id}")]
        [AllowAnonymous]
        public ResultModel<DemoResDto> GetItem(int id)
        {
            ResultModel<DemoResDto> resultModel = new ResultModel<DemoResDto>();
            resultModel.Data = new DemoResDto
            {
                Id = id,
                Name = $"模拟数据-{id}",
                CreateTime = DateTime.Now
            };
            return resultModel;
        }

        /// <summary>
        /// 示范3：分页查询（PageListReqDto 分页参数 + 返回 ResultPageModel 分页结构）
        /// </summary>
        /// <remarks>
        /// 演示：分页参数（pageIndex/pageSize/sort/sortOrder）自动校验：
        /// - pageSize 限制 1-100（[Range] 校验）
        /// - sort/sortOrder 白名单校验（防 SQL 注入）
        /// </remarks>
        [HttpGet("list")]
        [AllowAnonymous]
        public ResultPageModel<DemoResDto> GetList([FromQuery] PageListReqDto page)
        {
            //模拟 35 条数据
            List<DemoResDto> allData = new List<DemoResDto>();
            for (int i = 1; i <= 35; i++)
            {
                allData.Add(new DemoResDto { Id = i, Name = $"数据-{i}", CreateTime = DateTime.Now.AddDays(-i) });
            }

            var paged = allData
                .Skip((page.pageIndex - 1) * page.pageSize)
                .Take(page.pageSize)
                .ToList();

            return new ResultPageModel<DemoResDto>
            {
                Data = new PageJson<DemoResDto>
                {
                    TotalCount = allData.Count,
                    Items = paged
                }
            };
        }

        /// <summary>
        /// 示范4：QueryString 查询参数（URL 问号后的参数）
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="count">数量（可选，默认 5）</param>
        [HttpGet("query")]
        [AllowAnonymous]
        public ResultModel<string> GetQuery(string keyword, int count = 5)
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = $"关键字：{keyword ?? "(空)"}，数量：{count}";
            return resultModel;
        }

        /// <summary>
        /// 示范5：POST 提交 JSON Body（[FromBody] + DataAnnotations 校验）
        /// </summary>
        /// <remarks>
        /// 演示：DTO 校验（[Required]/[Range]）由全局 ApiFilterAttribute 自动执行，校验失败返回 400
        /// </remarks>
        [HttpPost("create")]
        [AllowAnonymous]
        public ResultModel<DemoResDto> Create([FromBody] DemoReqDto req)
        {
            ResultModel<DemoResDto> resultModel = new ResultModel<DemoResDto>();
            resultModel.Data = new DemoResDto
            {
                Id = new Random().Next(1000, 9999),
                Name = req.Name,
                CreateTime = DateTime.Now
            };
            resultModel.SetMessage("创建成功");
            return resultModel;
        }

        /// <summary>
        /// 示范6：PUT 更新（路径参数 + Body）
        /// </summary>
        /// <param name="id">编号</param>
        /// <param name="req">更新内容</param>
        [HttpPut("update/{id}")]
        [AllowAnonymous]
        public ResultModel Update(int id, [FromBody] DemoReqDto req)
        {
            return new ResultModel().SetMessage($"已更新编号 {id} 的数据，新名称：{req.Name}");
        }

        /// <summary>
        /// 示范7：DELETE 删除（路径参数）
        /// </summary>
        /// <param name="id">编号</param>
        [HttpDelete("delete/{id}")]
        [AllowAnonymous]
        public ResultModel Delete(int id)
        {
            return new ResultModel().SetMessage($"已删除编号 {id} 的数据");
        }

        /// <summary>
        /// 示范8：文件上传（IFormFile → Common.Upload 保存到 other 目录，通过 /other 静态路径访问）
        /// </summary>
        [HttpPost("upload")]
        [AllowAnonymous]
        public ResultModel<string> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new ResultModel<string>().SetMessage("请选择要上传的文件", false);
            }

            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "other");
            string fileName = Common.Upload(file, savePath);

            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = $"/other/{fileName}";
            resultModel.SetMessage("上传成功");
            return resultModel;
        }

        /// <summary>
        /// 示范9：缓存使用（MvcCore.Extension.MemoryCacheHelper 系统缓存 Set/Get）
        /// </summary>
        /// <remarks>
        /// 演示：两次调用同一接口，第一次未命中写入缓存，第二次命中直接返回
        /// </remarks>
        [HttpGet("cache")]
        [AllowAnonymous]
        public ResultModel<string> CacheDemo()
        {
            string cacheKey = "Demo_Cache_Time";
            string cached = MemoryCacheHelper.Get<string>(cacheKey);
            if (cached == null)
            {
                string value = $"首次生成时间：{DateTime.Now:HH:mm:ss.fff}";
                MemoryCacheHelper.Set(cacheKey, value, TimeSpan.FromSeconds(30));
                ResultModel<string> resultModel = new ResultModel<string>();
                resultModel.Data = value;
                return resultModel.SetMessage("缓存未命中，已写入");
            }
            return new ResultModel<string>().SetMessage($"缓存命中：{cached}");
        }

        /// <summary>
        /// 示范10：当前语言（BaseController.Language，从请求头 Language 读取，默认 CN）
        /// </summary>
        [HttpGet("language")]
        [AllowAnonymous]
        public ResultModel<string> GetLanguage()
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = Language == LanguageEnum.CN ? "当前语言：中文" : "Current Language: English";
            return resultModel;
        }

        /// <summary>
        /// 示范11：JWT 认证（需登录后带 token，展示从 claims 读取 UserId）
        /// </summary>
        /// <remarks>
        /// 演示：本接口无 [AllowAnonymous]，未带 token 返回 401；带上 token 后从 JWT claims 解析出用户编号（客户端无法伪造）
        /// </remarks>
        [HttpGet("authorize")]
        public ResultModel<string> GetAuthorizeInfo()
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = $"当前登录用户编号：{UserId}";
            return resultModel;
        }

        /// <summary>
        /// 示范12：异常处理（抛出异常 → 全局异常中间件捕获 → 返回 500 统一 JSON）
        /// </summary>
        [HttpGet("error")]
        [AllowAnonymous]
        public ResultModel GetError()
        {
            throw new Exception("这是模拟的业务异常，用于演示全局异常处理中间件");
        }

        /// <summary>
        /// 示范13：版本信息（体现 API 版本 + 配置读取）
        /// </summary>
        [HttpGet("version")]
        [AllowAnonymous]
        public ResultModel<string> GetVersion()
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = $"接口版本：{resultModel.ApiVersion}，环境：{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}";
            return resultModel;
        }

        /// <summary>
        /// 示范14：Keyed DI（.NET 8 特性：同一接口按 key 注入不同实现）
        /// </summary>
        /// <remarks>
        /// 演示：Program.cs 中 AddKeyedScoped 注册了 "a"/"b" 两个 key，
        /// 通过 [FromKeyedServices] 按 key 解析对应实现
        /// </remarks>
        [HttpGet("keyed-di")]
        [AllowAnonymous]
        public ResultModel<string> GetKeyedDi([FromKeyedServices("a")] IKeyedDemoService serviceA, [FromKeyedServices("b")] IKeyedDemoService serviceB)
        {
            ResultModel<string> resultModel = new ResultModel<string>();
            resultModel.Data = $"key 'a' → {serviceA.GetName()}，key 'b' → {serviceB.GetName()}";
            return resultModel;
        }
    }
}
