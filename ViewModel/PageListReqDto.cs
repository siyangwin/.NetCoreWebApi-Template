using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
		[Range(1, 100, ErrorMessage = "pageSize 必须在 1-100 之间")]
		public int pageSize { get; set; } = 20;

		/// <summary>
		/// 排序字段（白名单：仅允许字母/数字/下划线/逗号/点，防止 SQL 注入）
		/// </summary>
		[RegularExpression(@"^[A-Za-z0-9_,.\s]+$", ErrorMessage = "排序字段不合法")]
		public string sort { get; set; } = "Id";

		/// <summary>
		/// 排序方式（白名单：asc / desc）
		/// </summary>
		[RegularExpression(@"^(asc|desc)$", ErrorMessage = "排序方式不合法")]
		public string sortOrder { get; set; } = "asc";

		/// <summary>
		/// 動態條件
		/// </summary>
		public Dictionary<string, DynamicTree> dynamicWhere { get; set; }
	}
}
