using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.App
{

    /// <summary>
    /// 插入商品数据请求类
    /// </summary>
    public class ProductInfoReqDto
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public int productId { get; set; }

        /// <summary>
        /// 书名
        /// </summary>
        public string bookName { get; set; }


        /// <summary>
        /// 详情
        /// </summary>
        public string[][] detail { get; set; }

    }
}
