using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    /// <summary>
    /// 本地化 RegularExpression 验证（支持多语言错误消息）
    /// 用法：[LocalizedRegularExpression(@"^[A-Za-z0-9_]+$", "validation:sort_field_invalid")]
    /// </summary>
    public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
    {
        private readonly string _key;

        public LocalizedRegularExpressionAttribute(string pattern, string key) : base(pattern)
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
