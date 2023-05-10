using Kogel.Repository;
using Core;
using IService;
using System.Data.SqlClient;
using Kogel.Dapper.Extension;

namespace Service
{
	/// <summary>
	/// 不用每个实体继承仓储，直接使用此仓储当连接即可
	/// </summary>
	public class Repository : BaseRepository<IRepository>, IRepository
	{
		/// <summary>
		/// 配置数据库连接方式
		/// </summary>
		/// <param name="builder"></param>
		public override void OnConfiguring(RepositoryOptionsBuilder builder)
		{
			builder
				.BuildConnection(new SqlConnection(GlobalConfig.ConnectionString))
				.BuildProvider(new MsSqlProvider())
				.BuildAutoSyncStructure(false);
		}
	}
}
