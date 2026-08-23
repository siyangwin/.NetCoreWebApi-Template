using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    /// <summary>
    /// 本地化 StringLength 验证（支持多语言错误消息）
    /// 用法：[LocalizedStringLength(50, "validation:name_max_length")]
    /// </summary>
    public class LocalizedStringLengthAttribute : StringLengthAttribute
    {
        private readonly string _key;

        public LocalizedStringLengthAttribute(int maximumLength, string key) : base(maximumLength)
        {
            _key = key;
            ErrorMessage = null;
        }

        public override string FormatErrorMessage(string name)
        {
            return Lang.GetFormat(_key, MaximumLength);
        }
    }
}
