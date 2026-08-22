using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Table
{
    /// <summary>
    /// 系統日志
    /// </summary>
    [Display(Rename = "SystemLog")]
    public class SystemLog
    {
        #region Model
        /// <summary>
        /// 自增列
        /// </summary>
        [Identity]
        public  int Id { get; set; }

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 請求客戶類型 APP CMS
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// API名称（context.Request.Path）
        /// </summary>
        public string APIName { get; set; }

        /// <summary>
        /// 请求方式  Get Post   context.Request.Method;
        /// </summary>
        public string Request { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 操作说明
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// 请求参数内容
        /// </summary>
        public string ReqParameter { get; set; }

        /// <summary>
        /// 返回参数内容
        /// </summary>
        public string ResParameter { get; set; }

        /// <summary>
        /// 请求耗费时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 请求用户的IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 服务器名称(负载均衡记录) Environment.GetEnvironmentVariable("USERNAME")
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 认证类型（JWT / ApiKey / 空=未认证）
        /// </summary>
        public string AuthType { get; set; }

        /// <summary>
        /// 身份标识（JWT 时为 UserId，API Key 时为 KeyName）
        /// </summary>
        public string AuthIdentity { get; set; }
        #endregion
    }
}
