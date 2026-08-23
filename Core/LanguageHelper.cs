using Model.EnumModel;
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 语言代码映射工具：ISO 639-1 标准语言代码 ↔ LanguageEnum
    /// 支持 "zh"/"zh-CN"/"1" 等多种格式传入
    /// </summary>
    public static class LanguageHelper
    {
        // ============================================================
        // 默认语言配置 —— 修改此处即可切换系统默认语言
        // 当请求的语言缺少对应翻译时，会降级到此语言
        // ============================================================
        public static readonly LanguageEnum DefaultLanguage = LanguageEnum.EN;  // 默认英文，可改为 LanguageEnum.CN
        // ============================================================

        /// <summary>
        /// 语言代码 → 枚举 映射
        /// </summary>
        private static readonly Dictionary<string, LanguageEnum> _codeToEnum = new(StringComparer.OrdinalIgnoreCase)
        {
            { "zh", LanguageEnum.CN },
            { "zh-CN", LanguageEnum.CN },
            { "zh-TW", LanguageEnum.CN },
            { "en", LanguageEnum.EN },
            { "en-US", LanguageEnum.EN },
            { "en-GB", LanguageEnum.EN },
            { "ja", LanguageEnum.JA },
            { "ja-JP", LanguageEnum.JA }
        };

        /// <summary>
        /// 枚举 → 语言代码 映射
        /// </summary>
        private static readonly Dictionary<LanguageEnum, string> _enumToCode = new()
        {
            { LanguageEnum.CN, "zh" },
            { LanguageEnum.EN, "en" },
            { LanguageEnum.JA, "ja" }
        };

        /// <summary>
        /// 枚举 → 显示名称
        /// </summary>
        private static readonly Dictionary<LanguageEnum, string> _enumToDisplayName = new()
        {
            { LanguageEnum.CN, "中文" },
            { LanguageEnum.EN, "English" },
            { LanguageEnum.JA, "日本語" }
        };

        /// <summary>
        /// 字符串 → LanguageEnum（支持 "zh"/"1"/"CN" 等多种格式）
        /// </summary>
        /// <param name="input">语言字符串</param>
        /// <returns>解析后的 LanguageEnum，无法解析时返回默认语言</returns>
        public static LanguageEnum Parse(string input)
        {
            if (string.IsNullOrEmpty(input))
                return DefaultLanguage;

            // 尝试数字解析（兼容旧格式 "1"/"2"/"3"）
            if (int.TryParse(input, out int num) && Enum.IsDefined(typeof(LanguageEnum), num))
                return (LanguageEnum)num;

            // 尝试枚举名称解析（"CN"/"EN"/"JA"）
            if (Enum.TryParse<LanguageEnum>(input, true, out var result))
                return result;

            // 尝试 ISO 语言代码解析（"zh"/"en"/"ja"/"zh-CN" 等）
            if (_codeToEnum.TryGetValue(input, out var lang))
                return lang;

            return DefaultLanguage;
        }

        /// <summary>
        /// LanguageEnum → 语言代码字符串（如 "zh"/"en"/"ja"）
        /// </summary>
        public static string ToCode(LanguageEnum lang)
        {
            return _enumToCode.TryGetValue(lang, out var code) ? code : "en";
        }

        /// <summary>
        /// LanguageEnum → 显示名称（如 "中文"/"English"/"日本語"）
        /// </summary>
        public static string ToDisplayName(LanguageEnum lang)
        {
            return _enumToDisplayName.TryGetValue(lang, out var name) ? name : lang.ToString();
        }

        /// <summary>
        /// 获取所有支持的语言代码列表
        /// </summary>
        public static IReadOnlyList<string> GetSupportedCodes()
        {
            return new List<string>(_codeToEnum.Keys);
        }
    }
}
