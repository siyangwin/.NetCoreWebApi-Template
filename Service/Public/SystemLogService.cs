using Microsoft.AspNetCore.Http;
using Model.Table;
using Model.EnumModel;
using IService;
using Serilog;

namespace Service
{
	public class SystemLogService : ISystemLogService
	{

        public SystemLogService() 
        {
        }

        /// <summary>
        ///  写入日志
        /// </summary>
        /// <param name="systemLogType">日志级别</param>
        /// <param name="httpContext">请求内容</param>
        /// <param name="instructions">操作说明</param>
        /// <param name="reqParameter">请求参数内容</param>
        /// <param name="resParameter">返回参数内容</param>
        /// <param name="time">耗费时间</param>
        /// <param name="ex">错误级别需要</param>
        /// <returns></returns>
        public async Task LogAdd(SystemLogTypeEnum systemLogType, HttpContext httpContext, string instructions, string reqParameter, string resParameter, string? time, Exception? ex)
		{
			SystemLog systemLog = new SystemLog();
			systemLog.Guid = httpContext.Request.Headers["Guid"].ToString();
			systemLog.ClientType = httpContext.Request.Headers["ClientType"].ToString();
			systemLog.APIName = httpContext.Request.Path;
			systemLog.Request = httpContext.Request.Method;
            systemLog.UserId = httpContext.Request.Headers["UserId"].ToString() == "" ? 0 : Convert.ToInt32(httpContext.Request.Headers["UserId"]);
			systemLog.DeviceId = httpContext.Request.Headers["DeviceId"].ToString() == "" ? "0" : httpContext.Request.Headers["DeviceId"].ToString();
            systemLog.Instructions = instructions;
			systemLog.ReqParameter = reqParameter;
			systemLog.ResParameter = resParameter;
			systemLog.Time = string.IsNullOrEmpty(time)?"":time;
			systemLog.IP = string.IsNullOrEmpty(httpContext.Connection.RemoteIpAddress.ToString()) ? "": httpContext.Connection.RemoteIpAddress.ToString();
			systemLog.Server = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("USERNAME"))? "": Environment.GetEnvironmentVariable("USERNAME");


            //string MessageTemplate = "{Guid}{ClientType}{APIName}{Request}{UserId}{DeviceId}{Instructions}{ReqParameter}{ResParameter}{Time}{IP}{Server}";
            string MessageTemplate = "\"Guid\":\"{Guid}\",\"ClientType\":\"{ClientType}\",\"APIName\":\"{APIName}\",\"Request\":\"{Request}\",\"UserId\":\"{UserId}\",\"DeviceId\":\"{DeviceId}\",\"Instructions\":\"{Instructions}\",\"ReqParameter\":\"{ReqParameter}\",\"ResParameter\":\"{ResParameter}\",\"Time\":\"{Time}\",\"IP\":\"{IP}\",\"Server\":\"{Server}\"";

            //判断传入参数
            switch (systemLogType)
			{
				case SystemLogTypeEnum.Verbose:
                    Log.Verbose(MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                case SystemLogTypeEnum.Debug:
                    Log.Debug(MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                case SystemLogTypeEnum.Information:
                    Log.Information(MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                case SystemLogTypeEnum.Warning:
                    Log.Warning(MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                case SystemLogTypeEnum.Error:
                    Log.Error(ex, MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                case SystemLogTypeEnum.Fatal:
                    Log.Fatal(MessageTemplate, systemLog.Guid, systemLog.ClientType, systemLog.APIName, systemLog.Request, systemLog.UserId, systemLog.DeviceId, systemLog.Instructions, systemLog.ReqParameter, systemLog.ResParameter, systemLog.Time, systemLog.IP, systemLog.Server);
                    break;
                default:
					break;
			}
        }
	}
}
