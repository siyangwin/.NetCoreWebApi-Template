using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Xml;
using Model.EnumModel;

namespace Project.Core
{
    /// <summary>
    /// 多语言配置
    /// </summary>
    public class ResourceManage
    {
        ResourceManager resource;

        public ResourceManage()
        {

        }
        public ResourceManage(ResourceManager resource)
        {
            this.resource = resource;
        }

        /// <summary>
        /// 根据语言获取资源内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public string Get(string name, LanguageEnum language)
        {
            string result = string.Empty;
            result = GetByXml(name, language);
            return !string.IsNullOrEmpty(result) ? result : name;
        }

        /// <summary>
        /// 根据xml获取中文
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetByXml(string name, LanguageEnum language)
        {
            string result = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Config/LanguageResource.resx");
            var nodes = xmlDoc.SelectNodes("root/data");
            foreach (XmlNode row in nodes)
            {
                if (row.Attributes["name"].InnerText == name)
                {
                    if (language == LanguageEnum.CN)
                        result = row.SelectSingleNode("comment")?.InnerText;
                    else
                        result = row.SelectSingleNode("value")?.InnerText;
                    break;
                }
            }
            return result;
        }
    }
}
