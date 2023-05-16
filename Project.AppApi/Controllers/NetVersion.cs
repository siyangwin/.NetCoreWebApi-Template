using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Table;
using ViewModel;
using ViewModel.App;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// 测试
    /// </summary>
    [ApiController]
    //[Route("[controller]")]
    public class NetVersion : BaseController
    {

        private readonly ILogger<NetVersion> _logger;

        //数据库链接
        private IRepository connection;

        private ISystemLogService systemLogService { get; set; }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="logger"></param>
       /// <param name="connection"></param>
       /// <param name="systemLogService"></param>
        public NetVersion(ILogger<NetVersion> logger, IRepository connection, ISystemLogService systemLogService)  //, IRepository connection
        {
            _logger = logger;
            this.connection = connection;
            this.systemLogService = systemLogService;
        }


        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        [Route("/api/WeatherForecast/get-log")]
        [HttpGet]
        public async Task<ResultModel<AuthorizationTokenResDto>> Log()
        {
            ResultModel<AuthorizationTokenResDto> resultModel = new ResultModel<AuthorizationTokenResDto>();
            //var connection = new Repository();
            //var SystemLog = connection.QuerySet<SystemLog>().Get();
            //_logger.LogInformation("INFO");
            //_logger.LogError("ERROR");
            //_logger.LogWarning("WARNING");
            //_logger.LogDebug("DEBUG");

            AuthorizationTokenResDto systemLog = new AuthorizationTokenResDto();
            systemLog.UserId = 1;
            systemLog.Token = Guid.NewGuid().ToString();


            //故意制造错误
            //int a = 1;int b = 0;
            //int c = a / b;

            resultModel.data = systemLog;
            return resultModel;
        }
    }
}