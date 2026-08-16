using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Name 不能为空")]
        [StringLength(50, ErrorMessage = "Name 长度不能超过 50")]
        public string Name { get; set; }

        /// <summary>
        /// 年龄（示范 Range 校验）
        /// </summary>
        [Range(1, 120, ErrorMessage = "Age 必须在 1-120 之间")]
        public int Age { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
