using System;
using System.Collections.Generic;
using System.Text;
using ViewModel;

namespace IService
{
    public interface IConfigService : IBaseService
    {
        /// <summary>
        /// 根据名称获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLike">是否模糊查询</param>
        /// <returns></returns>
        List<ConfigResDto> Get(string name, bool isLike);

    }
}
