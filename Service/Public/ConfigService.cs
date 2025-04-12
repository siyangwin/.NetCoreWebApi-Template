using IService;
using Model.View;
using System;
using System.Collections.Generic;
using System.Text;
using ViewModel;

namespace Service
{
    public class ConfigService : IConfigService
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly IRepository connection;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="connection">数据库操作</param>
        public ConfigService(IRepository connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// 根据名称获取配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isLike">是否模糊查询</param>
        /// <returns></returns>
        public List<ConfigResDto> Get(string name, bool isLike)
        {
            return connection.QuerySet<vw_public_Config>()
                .WhereIf(isLike, x => x.Name.Contains(name), x => x.Name == name)
                .ToList<ConfigResDto>();
        }

    }
}
