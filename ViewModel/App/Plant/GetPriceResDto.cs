using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 获取价格表
    /// </summary>
    public class GetPriceResDto
    {
        /// <summary>
        /// 返回PriceTable
        /// </summary>
        public List<GetPriceTable> PriceTable { get; set; }

        /// <summary>
        /// 今日价格
        /// </summary>
        public decimal TodayPrice { get; set; }
    }

    /// <summary>
    /// PriceTable
    /// </summary>
    public class GetPriceTable
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月份价格
        /// </summary>
        public List<MonthPrices>  MonthPriceList { get; set; }
    }

    /// <summary>
    /// MonthPriceList
    /// </summary>
    public class MonthPrices {

        /// <summary>
        /// 是否有值
        /// </summary>
        public bool IsHave { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}
