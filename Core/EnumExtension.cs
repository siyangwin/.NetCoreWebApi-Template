using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Project.Core
{
    public static class EnumExtension
    {
        /// <summary>
        /// 根據枚舉成員獲取自定義屬性EnumDisplayNameAttribute的屬性DisplayName
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetEnumCustomDescription(object e)
        {
            //獲取枚舉的Type類型對象
            Type t = e.GetType();
            //獲取枚舉的所有字段
            FieldInfo[] ms = t.GetFields();

            //遍歷所有枚舉的所有字段
            foreach (FieldInfo f in ms)
            {
                if (f.Name != e.ToString())
                {
                    continue;
                }
                //第二個參數true表示查找EnumDisplayNameAttribute的繼承鏈
                if (f.IsDefined(typeof(EnumDisplayNameAttribute), true))
                {
                    return
                        (f.GetCustomAttributes(typeof(EnumDisplayNameAttribute), true)[0] as EnumDisplayNameAttribute)
                            .DisplayName;
                }
            }

            //如果沒有找到自定義屬性，直接返回屬性項的名稱
            return e.ToString();
        }
        /// <summary>
        ///  把枚舉的描述和值綁定到DropDownList
        /// </summary>
        /// <param name="isNameValue">是否使用枚舉名做為值(value)</param>
        /// <param name="isNameValue">是否使用枚舉名做為显示(Text)</param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetSelectList(Type enumType, string emptyKey, string emptyValue, bool isNameValue = false, bool isNameText = false)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(emptyKey))
            {
                result.Add(new KeyValuePair<string, string>(emptyKey, emptyValue));
            }
            foreach (object e in Enum.GetValues(enumType))
            {
                result.Add(new KeyValuePair<string, string>(isNameValue ? e.ToString() : ((int)e).ToString(), isNameText ? e.ToString() : GetDescription(e)));
            }
            return result;
        }
        /// <summary>
        ///  把枚舉的描述和值綁定到DropDownList  KEY值為int類型
        /// </summary>
        /// <param name="isNameValue">是否使用枚舉名做為值(value)</param>
        /// <returns></returns>
        public static List<KeyValuePair<int, string>> GetSelectListInt(Type enumType, bool isNameValue = false)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            foreach (object e in Enum.GetValues(enumType))
            {
                result.Add(new KeyValuePair<int, string>(isNameValue ? (int)e : ((int)e), GetDescription(e)));
            }
            return result;
        }
        /// <summary>
        /// 獲取枚舉項描述信息 
        /// </summary>
        /// <param name="en">枚舉項</param>
        /// <returns></returns>
        public static string GetEnumDesc(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
        /// <summary>
        /// 獲取枚舉項描述信息 
        /// </summary>
        /// <param name="en">枚舉項</param>
        /// <returns></returns>
        public static string GetEnumDesc(Enum en, string value)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
        /// <summary>
        /// 獲取枚舉的描述文本
        /// </summary>
        /// <param name="e">枚舉成員</param>
        /// <returns></returns>
        public static string GetDescription(object e)
        {
            //獲取字段信息
            System.Reflection.FieldInfo[] ms = e.GetType().GetFields();
            Type t = e.GetType();
            foreach (System.Reflection.FieldInfo f in ms)
            {
                //判斷名稱是否相等
                if (f.Name != e.ToString()) continue;
                //反射出自定義屬性
                foreach (Attribute attr in f.GetCustomAttributes(true))
                {
                    //類型轉換找到一個Description，用Description作為成員名稱
                    DescriptionAttribute dscript = attr as DescriptionAttribute;
                    if (dscript != null)
                        return dscript.Description;
                }
            }
            //如果沒有檢測到合適的注釋，則用默認名稱
            return e.ToString();
        }
        /// <summary>
        /// 根據值得到描述文本
        /// </summary>
        public static string GetDescription(Type enumType, int? value)
        {
            if (value.HasValue)
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if ((int)etype == value.Value) return GetDescription(etype);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 根據名字得到描述文本
        /// </summary>
        public static string GetEnumDesc(Type enumType, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (etype.ToString() == name) return GetDescription(etype);
                }
            }
            return name;
        }
        /// <summary>
        /// 根據值得到描述文本
        /// </summary>
        public static string GetEnumDesc(Type enumType, int? value)
        {
            if (value.HasValue)
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if ((int)etype == value.Value) return GetDescription(etype);
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 根據描述文本得到值
        /// </summary>
        /// <param name="enumType">typeof()</param>
        /// <param name="description">描述文本</param>
        /// <param name="isValidityCheck">是否驗證描述包含在枚舉內</param>
        /// <param name="checkEmpty">是否檢查空</param>
        /// <returns></returns>
        public static int? GetValue(Type enumType, string description, bool isValidityCheck = false, bool checkEmpty = false)
        {
            var topAttr = ((DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute)));
            string topDescription = (topAttr == null ? enumType.Name : topAttr.Description);

            if (string.IsNullOrWhiteSpace(description))
            {
                if (checkEmpty) throw new Exception(string.Format("{0}不能為空", topDescription));
            }
            else
            {
                foreach (object etype in Enum.GetValues(enumType))
                {
                    if (GetDescription(etype) == description || etype.ToString().Equals(description, StringComparison.CurrentCultureIgnoreCase) || ((int)etype).ToString() == description)
                        return (int)etype;
                }

                if (isValidityCheck) throw new Exception(string.Format("{0}錯誤[{1}]", topDescription, description));
            }
            return null;
        }

    }
    public class EnumDisplayNameAttribute : Attribute
    {
        private string _displayName;

        public EnumDisplayNameAttribute(string displayName)
        {
            this._displayName = displayName;
        }
        public string DisplayName
        {
            get { return _displayName; }
        }
    }
}
