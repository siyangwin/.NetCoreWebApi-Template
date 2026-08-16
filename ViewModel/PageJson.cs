using System.Collections.Generic;

namespace ViewModel
{
	/// <summary>
	/// 翻頁返回基類
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class PageJson<T>
    {
        /// <summary>
        /// 總條數
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 數據集合
        /// </summary>
        public List<T> Items { get; set; }
    }
}
