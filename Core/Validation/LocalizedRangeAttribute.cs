using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    /// <summary>
    /// 本地化 Range 验证（支持多语言错误消息）
    /// 用法：[LocalizedRange(1, 120, "validation:age_range")]
    /// </summary>
    public class LocalizedRangeAttribute : RangeAttribute
    {
        private readonly string _key;

        public LocalizedRangeAttribute(int minimum, int maximum, string key) : base(minimum, maximum)
        {
            _key = key;
            ErrorMessage = null;
        }

        public override string FormatErrorMessage(string name)
        {
            return Lang.Get(_key);
        }
    }
}
