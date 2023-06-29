using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 根据ProductId获取市场信息返回信息
    /// </summary>
    public class GetAreaByMBIDResDto
    {
        public List<Datum> Data { get; set; }
        public object RequestWebViewData { get; set; }
        public DateTime RunnigStartTime { get; set; }
        public DateTime RunnigEndTime { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object CommandName { get; set; }
    }

    public class Datum
    {
        public string MArea { get; set; }
        public string MArea2 { get; set; }
        public string MAreaID { get; set; }
    }
}
