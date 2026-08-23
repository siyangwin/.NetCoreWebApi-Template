using System;
using System.Collections.Generic;

namespace Model.EnumModel
{
    /// <summary>
    /// 语言枚举（内部代码使用，与语言代码通过 LanguageHelper 映射）
    /// </summary>
    public enum LanguageEnum
    {
        /// <summary>
        /// 中文（对应语言代码 "zh"）
        /// </summary>
        CN = 1,

        /// <summary>
        /// 英文（对应语言代码 "en"）
        /// </summary>
        EN = 2,

        /// <summary>
        /// 日文（对应语言代码 "ja"）
        /// </summary>
        JA = 3
    }
}
