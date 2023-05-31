using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.Table
{
    /// <summary>
    /// 中国药典
    /// </summary>
    [Display(Rename = "ChinesePharmacopoeia")]
    public class ChinesePharmacopoeia : BaseModel
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string NameCN { get; set; }

        /// <summary>
        /// 拼音名
        /// </summary>
        public string NamePY { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        public string NameEN { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 性状
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// 鉴别
        /// </summary>
        public string Identify { get; set; }

        /// <summary>
        /// 含量测定
        /// </summary>
        public string ContentDetermination { get; set; }

        /// <summary>
        /// 炮制
        /// </summary>
        public string Preparation { get; set; }

        /// <summary>
        /// 制法
        /// </summary>
        public string PreparationMethod { get; set; }

        /// <summary>
        /// 性味
        /// </summary>
        public string Taste { get; set; }

        /// <summary>
        /// 归经
        /// </summary>
        public string Attribution { get; set; }

        /// <summary>
        /// 功效
        /// </summary>
        public string Efficacy { get; set; }

        /// <summary>
        /// 用法用量
        /// </summary>
        public string Dosage { get; set; }

        /// <summary>
        /// 储藏
        /// </summary>
        public string Storage { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 摘录出处
        /// </summary>
        public string ExcerptSource { get; set; }
    }
}
