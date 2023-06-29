using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{
    /// <summary>
    /// 根据ProductId和市场id获取规格返回信息
    /// </summary>
    public class GetSpecByAreaIDResDto
    {

        public List<GetSpecByAreaIDResDtoDatum> Data { get; set; }
        public object RequestWebViewData { get; set; }
        public DateTime RunnigStartTime { get; set; }
        public DateTime RunnigEndTime { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object CommandName { get; set; }
    }

    public class GetSpecByAreaIDResDtoDatum
    {
        public string mid { get; set; }
        public string MSpec { get; set; }
        public string MBSID { get; set; }
    }
}
