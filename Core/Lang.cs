using Microsoft.AspNetCore.Http;
using Model.EnumModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core
{
    /// <summary>
    /// 静态多语言工具类
    /// 使用方式：
    ///   Lang.Get("common:success")                        → 当前语言
    ///   Lang.Get("auth:login_failed", LanguageEnum.CN)    → 显式指定语言
    ///   Lang.GetFormat("demo:deleted", 123)               → 带占位符
    ///
    /// 支持 "module:key" 格式，如 "common:success"、"auth:login_failed"
    /// 三级降级：当前语言 → 默认语言 → 返回 key 本身
    /// </summary>
    public static class Lang
    {
        // ============================================================
        // 默认语言配置（与 LanguageHelper.DefaultLanguage 保持一致）
        // ============================================================
        private static string _defaultLanguage = "en";

        private static Dictionary<string, Dictionary<string, string>> _resources = new(StringComparer.OrdinalIgnoreCase);
        private static bool _loaded = false;

        /// <summary>
        /// 启动时加载所有语言资源文件
        /// </summary>
        /// <param name="resourceDir">资源文件目录（如 AppContext.BaseDirectory/Resources）</param>
        /// <param name="defaultLanguage">默认语言代码（null 时使用 "en"）</param>
        public static void Load(string resourceDir, string defaultLanguage = null)
        {
            _defaultLanguage = defaultLanguage ?? "en";
            _resources.Clear();

            if (!Directory.Exists(resourceDir))
                return;

            var jsonFiles = Directory.GetFiles(resourceDir, "*.json");
            foreach (var file in jsonFiles)
            {
                string langCode = Path.GetFileNameWithoutExtension(file); // "zh", "en", "ja"
                try
                {
                    string json = File.ReadAllText(file);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (dict == null) continue;

                    var langDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kvp in dict)
                    {
                        // 支持嵌套对象格式（"common": {"success": "..."}）和扁平格式（"success": "..."）
                        if (kvp.Value is JObject jObj)
                        {
                            foreach (var prop in jObj.Properties())
                            {
                                string fullKey = $"{kvp.Key}:{prop.Name}";
                                langDict[fullKey] = prop.Value?.ToString() ?? prop.Name;
                            }
                        }
                        else
                        {
                            langDict[kvp.Key] = kvp.Value?.ToString() ?? kvp.Key;
                        }
                    }
                    _resources[langCode] = langDict;
                }
                catch
                {
                    // 加载失败跳过，不影响启动
                }
            }

            _loaded = true;
        }

        /// <summary>
        /// 获取翻译（使用 HttpContext 自动识别语言）
        /// </summary>
        /// <param name="key">翻译 key（支持 "module:key" 格式）</param>
        /// <param name="context">HttpContext</param>
        /// <returns>翻译文本，找不到时返回 key 本身</returns>
        public static string GetWithContext(string key, HttpContext context)
        {
            string lang = ResolveLanguage(context);
            return Get(key, lang);
        }

        /// <summary>
        /// 获取翻译（显式指定 LanguageEnum）
        /// </summary>
        public static string Get(string key, LanguageEnum language)
        {
            return Get(key, LanguageHelper.ToCode(language));
        }

        /// <summary>
        /// 获取翻译（显式指定语言代码字符串）
        /// </summary>
        /// <param name="key">翻译 key（支持 "module:key" 格式）</param>
        /// <param name="language">语言代码（如 "zh"/"en"），null 时使用默认语言</param>
        /// <returns>翻译文本，找不到时返回 key 本身</returns>
        public static string Get(string key, string language = null)
        {
            language ??= _defaultLanguage;

            // 1. 查当前语言
            if (_resources.TryGetValue(language, out var dict) && dict.TryGetValue(key, out var value))
                return value;

            // 2. 降级到默认语言
            if (language != _defaultLanguage && _resources.TryGetValue(_defaultLanguage, out var fallback) && fallback.TryGetValue(key, out var fbValue))
                return fbValue;

            // 3. 返回 key 本身（兜底）
            return key;
        }

        /// <summary>
        /// 获取带占位符的翻译（如 {0}, {1}...）
        /// </summary>
        /// <param name="key">翻译 key</param>
        /// <param name="args">占位符参数</param>
        /// <returns>格式化后的翻译文本</returns>
        public static string GetFormat(string key, params object[] args)
        {
            string value = Get(key, (string)null);
            return args != null && args.Length > 0 ? string.Format(value, args) : value;
        }

        /// <summary>
        /// 获取带占位符的翻译（显式指定语言）
        /// </summary>
        public static string GetFormat(string key, LanguageEnum language, params object[] args)
        {
            string value = Get(key, language);
            return args != null && args.Length > 0 ? string.Format(value, args) : value;
        }

        /// <summary>
        /// 获取带占位符的翻译（显式指定语言代码）
        /// </summary>
        public static string GetFormat(string key, string language, params object[] args)
        {
            string value = Get(key, language);
            return args != null && args.Length > 0 ? string.Format(value, args) : value;
        }

        /// <summary>
        /// 解析当前请求的语言
        /// 优先级：请求 Header/Query → 默认语言
        /// </summary>
        private static string ResolveLanguage(HttpContext context)
        {
            // 从 Request Headers 读取（与现有中间件一致，中间件已写入 "Language" header）
            string lang = context?.Request?.Headers["Language"].ToString();
            if (!string.IsNullOrEmpty(lang))
            {
                // header 中存的是枚举名称（如 "CN"/"EN"），转换为语言代码
                if (Enum.TryParse<LanguageEnum>(lang, true, out var enumVal))
                    return LanguageHelper.ToCode(enumVal);
                return lang;
            }

            return _defaultLanguage;
        }
    }
}
