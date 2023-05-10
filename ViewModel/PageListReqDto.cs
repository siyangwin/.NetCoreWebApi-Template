using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel
{
	/// <summary>
	/// 翻頁列表請求基類
	/// </summary>
	public class PageListReqDto
	{
		/// <summary>
		/// 當前頁
		/// </summary>
		public int pageIndex { get; set; } = 1;

		/// <summary>
		/// 顯示數
		/// </summary>
		public int pageSize { get; set; } = 20;

		/// <summary>
		/// 排序字段
		/// </summary>
		public string sort { get; set; } = "Id";

		/// <summary>
		/// 排序方式
		/// </summary>
		public string sortOrder { get; set; } = "asc";

		/// <summary>
		/// 動態條件
		/// </summary>
		public Dictionary<string, DynamicTree> dynamicWhere { get; set; }
	}
}
