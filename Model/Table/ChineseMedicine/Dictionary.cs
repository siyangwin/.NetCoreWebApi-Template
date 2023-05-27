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
    /// 辞典
    /// </summary>
    [Display(Rename = "Dictionary")]
    public class Dictionary : BaseModel
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
        /// 出处
        /// </summary>
        public string Provenance { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 原形态
        /// </summary>
        public string OriginalForm { get; set; }

        /// <summary>
        /// 性状
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// 化学成分
        /// </summary>
        public string ChemicalComposition { get; set; }

        /// <summary>
        /// 作用
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 炮制
        /// </summary>
        public string Preparation { get; set; }

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
        /// 注意
        /// </summary>
        public string Notice { get; set; }

        /// <summary>
        /// 附方
        /// </summary>
        public string SupplementaryFormula { get; set; }

        /// <summary>
        /// 各家论述
        /// </summary>
        public string Discussions { get; set; }

        /// <summary>
        /// 临床应用
        /// </summary>
        public string ClinicalApplication { get; set; }

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
