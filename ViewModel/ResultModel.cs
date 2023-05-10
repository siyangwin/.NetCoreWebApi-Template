using System.Collections.Generic;

namespace ViewModel
{
    /// <summary>
    /// API返回信息
    /// </summary>
    public class ResultModels<T>
    {
        /// <summary>
        /// 返回類型
        /// </summary>
        public ResultModels()
        {
            this.api_version = "v1";
            code = "200";
            this.success = true;
        }
        /// <summary>
        /// 版本號
        /// </summary>
        public string api_version { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 總數量
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回數據集合
        /// </summary>
        public List<T> data { get; set; }
    }
    /// <summary>
    /// API返回信息
    /// </summary>
    public class ResultModel
    {
        /// <summary>
        /// 返回類型
        /// </summary>
        public ResultModel()
        {
            this.api_version = "v1";
            code = "200";
            this.success = true;
        }
        /// <summary>
        /// 版本號
        /// </summary>
        public string api_version { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultModel SetMessage(string message)
        {
            this.message = message;
            return this;
        }

        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ResultModel SetMessage(string message, bool success)
        {
            this.success = success;
            this.message = message;
            return this;
        }
    }
    /// <summary>
    /// API返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultModel<T>
    {
        /// <summary>
        /// 返回類型
        /// </summary>
        public ResultModel()
        {
            this.api_version = "v1";
            code = "200";
            this.success = true;
        }
        /// <summary>
        /// 版本號
        /// </summary>
        public string api_version { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回數據集合
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultModel<T> SetMessage(string message)
        {
            this.message = message;
            return this;
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="success"></param>
        public void SetMsg(string message, bool success)
        {
            this.message = message;
            this.success = success;
        }

        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ResultModel<T> SetMessage(string message, bool success)
        {
            this.success = success;
            this.message = message;
            return this;
        }
    }
    /// <summary>
    /// API返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPageModel<T>
    {
        /// <summary>
        /// 返回類型
        /// </summary>
        public ResultPageModel()
        {
            this.api_version = "v1";
            code = "200";
            this.success = true;
        }
        /// <summary>
        /// 版本號
        /// </summary>
        public string api_version { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 分頁數據
        /// </summary>
        public PageJson<T> data { get; set; }
    }
    /// <summary>
    /// 積分記錄專用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPageModel_Point<T>
    {
        /// <summary>
        /// 返回類型
        /// </summary>
        public ResultPageModel_Point()
        {
            this.api_version = "v1";
            code = "200";
            this.success = true;
        }
        /// <summary>
        /// 版本號
        /// </summary>
        public string api_version { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 分頁數據
        /// </summary>
        public PageJson_Point<T> data { get; set; }
    }
}
