using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// GetPriceChart 获取价格走势图和进入价格
    /// </summary>
    public class GetPriceChartResDto
    {
        public GetPriceChartData Data { get; set; }
        public object RequestWebViewData { get; set; }
        public DateTime RunnigStartTime { get; set; }
        public DateTime RunnigEndTime { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object CommandName { get; set; }
    }

    public class GetPriceChartData
    {
        public string PriceChartData { get; set; }
        public decimal TodayPrice { get; set; }
    }
}
