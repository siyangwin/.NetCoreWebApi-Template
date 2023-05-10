using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Table;
using Newtonsoft.Json;
using Service;
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
        public AuthorizationTokenResDto Log()
        {
            //var connection = new Repository();
            //var SystemLog = connection.QuerySet<SystemLog>().Get();
            _logger.LogInformation("INFO");
            _logger.LogError("ERROR");
            _logger.LogWarning("WARNING");

            AuthorizationTokenResDto systemLog = new AuthorizationTokenResDto();
            systemLog.UserId = 1;
            systemLog.Token = Guid.NewGuid().ToString();

            systemLogService.LocalAndSqlLogAdd(new SystemLog { Guid = HttpContext.Request.Headers["Guid"].ToString(), ClientType = HttpContext.Request.Headers["ClientType"].ToString(), APIName = HttpContext.Request.Path, UserId = HttpContext.Request.Headers["UserId"].ToString() == "" ? 0 : Convert.ToInt32(HttpContext.Request.Headers["UserId"]), DeviceId = HttpContext.Request.Headers["DeviceId"].ToString() == "" ? "0" : HttpContext.Request.Headers["DeviceId"].ToString(), Instructions = "请求-返回", ReqParameter = "", ResParameter = "", Time = "", IP = "" });

            return systemLog;
        }
    }
}