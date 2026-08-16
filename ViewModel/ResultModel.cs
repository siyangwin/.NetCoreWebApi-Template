using System;

namespace ViewModel
{
    /// <summary>
    /// API返回信息基類
    /// </summary>
    public abstract class ApiResult
    {
        protected ApiResult()
        {
            ApiVersion = "v1";
            Code = "200";
            Success = true;
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 版本號
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回時間（默認當前時間）
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// API返回信息
    /// </summary>
    public class ResultModel : ApiResult
    {
        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultModel SetMessage(string message)
        {
            this.Message = message;
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
            this.Success = success;
            this.Message = message;
            return this;
        }
    }

    /// <summary>
    /// API返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultModel<T> : ApiResult
    {
        /// <summary>
        /// 返回數據集合
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResultModel<T> SetMessage(string message)
        {
            this.Message = message;
            return this;
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="success"></param>
        public void SetMsg(string message, bool success)
        {
            this.Message = message;
            this.Success = success;
        }

        /// <summary>
        /// 設置信息並返回
        /// </summary>
        /// <param name="message"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public ResultModel<T> SetMessage(string message, bool success)
        {
            this.Success = success;
            this.Message = message;
            return this;
        }
    }

    /// <summary>
    /// 分頁返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPageModel<T> : ApiResult
    {
        /// <summary>
        /// 分頁數據
        /// </summary>
        public PageJson<T> Data { get; set; }
    }
}
