using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model.View
{
    /// <summary>
    /// 植物详情
    /// </summary>
    [Display(Rename = "vm_app_PlantInfo")]
    public class vm_app_PlantInfo
    {
        //植物编号
        public int Id { get; set; }

        // 中文名
        public string NameCN { get; set; }

        // 出处
        public string Provenance { get; set; }

        // 拼音名
        public string NamePY { get; set; }

        // 英文名
        public string NameEN { get; set; }

        // 别名
        public string Alias { get; set; }

        // 来源
        public string Source { get; set; }

        // 原形态
        public string OriginalForm { get; set; }

        // 生境分布
        public string HabitatDistribution { get; set; }

        // 栽培
        public string Cultivation { get; set; }

        // 性状
        public string Character { get; set; }

        // 化学成分
        public string ChemicalComposition { get; set; }

        // 鉴别
        public string Identify { get; set; }

        // 作用
        public string Role { get; set; }

        // 炮制
        public string Preparation { get; set; }

        // 性味
        public string Taste { get; set; }

        // 归经
        public string Attribution { get; set; }

        // 功效
        public string Efficacy { get; set; }

        // 用法用量
        public string Dosage { get; set; }

        // 注意
        public string Notice { get; set; }

        // 附方
        public string SupplementaryFormula { get; set; }

        // 名家论述
        public string Discussions { get; set; }

        // 临床应用
        public string ClinicalApplication { get; set; }
    }
}
