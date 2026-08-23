using System.ComponentModel.DataAnnotations;
using Core.Validation;

namespace ViewModel.Demo
{
    /// <summary>
    /// Demo 示范请求类
    /// </summary>
    public class DemoReqDto
    {
        /// <summary>
        /// 名称（必填，示范 Required 校验）
        /// </summary>
        [LocalizedRequired("validation:name_required")]
        [LocalizedStringLength(50, "validation:name_max_length")]
        public string Name { get; set; }

        /// <summary>
        /// 年龄（示范 Range 校验）
        /// </summary>
        [LocalizedRange(1, 120, "validation:age_range")]
        public int Age { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
