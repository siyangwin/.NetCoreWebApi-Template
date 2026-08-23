using Kogel.Dapper.Extension.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Core.Validation;

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
		[LocalizedRange(1, 100, "validation:pagesize_range")]
		public int pageSize { get; set; } = 20;

		/// <summary>
		/// 排序字段（白名单：仅允许字母/数字/下划线/逗号/点，防止 SQL 注入）
		/// </summary>
		[LocalizedRegularExpression(@"^[A-Za-z0-9_,.\s]+$", "validation:sort_field_invalid")]
		public string sort { get; set; } = "Id";

		/// <summary>
		/// 排序方式（白名单：asc / desc）
		/// </summary>
		[LocalizedRegularExpression(@"^(asc|desc)$", "validation:sort_order_invalid")]
		public string sortOrder { get; set; } = "asc";

		/// <summary>
		/// 動態條件
		/// </summary>
		public Dictionary<string, DynamicTree> dynamicWhere { get; set; }
	}
}
