using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 根据Mid获取价格Table
    /// </summary>
    public class GetPriceTableResDto
    {

        public List<GetPriceTableDatum> Data { get; set; }
        public object RequestWebViewData { get; set; }
        public DateTime RunnigStartTime { get; set; }
        public DateTime RunnigEndTime { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object CommandName { get; set; }
    }

    public class GetPriceTableDatum
    {
        //年份
        public int Year { get; set; }

        //月份价格表格信息
        public List<GetPriceTableListtdmonthprice> ListTdMonthPrice { get; set; }
    }

    public class GetPriceTableListtdmonthprice
    {
        public bool IsHave { get; set; }
        public int Month { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; }
    }
}
