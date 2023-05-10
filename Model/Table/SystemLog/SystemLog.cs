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
        /// 创建人
        /// </summary>
        public  string CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public  DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public  string UpdateUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public  DateTime UpdateTime { get; set; }
        /// <summary>
        /// 删除标志（已经删除[True]=1    未删除[False]=0）
        /// </summary>
        public  bool IsDelete { get; set; }
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 請求客戶類型 APP CMS
        /// </summary>
        public string ClientType { get; set; }

        /// <summary>
        /// 条款Url
        /// </summary>
        public string APIName { get; set; }

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
        /// 
        /// </summary>
        public string IP { get; set; }
        #endregion
    }
}
