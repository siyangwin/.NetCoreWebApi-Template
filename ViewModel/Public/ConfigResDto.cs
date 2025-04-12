using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModel
{
	/// <summary>
	/// 配置响应类
	/// </summary>
	public class ConfigResDto
	{
		/// <summary>
		/// 编号
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 名称(code)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 值
		/// </summary>
		public string Values { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		public DateTime UpdateTime { get; set; }
	}
}
