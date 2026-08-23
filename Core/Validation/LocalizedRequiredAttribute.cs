using System.ComponentModel.DataAnnotations;

namespace Core.Validation
{
    /// <summary>
    /// 本地化 Required 验证（支持多语言错误消息）
    /// 用法：[LocalizedRequired("validation:name_required")]
    /// </summary>
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        private readonly string _key;

        public LocalizedRequiredAttribute(string key)
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
