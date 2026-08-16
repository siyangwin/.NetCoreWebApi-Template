using Kogel.Repository;
using Core;
using IService;
using Microsoft.Data.SqlClient;
using Kogel.Dapper.Extension;
using Model;

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

		/// <summary>
		/// 新增（自动填充审计字段：CreateTime/UpdateTime/CreateUser/UpdateUser，IsDelete=false）
		/// </summary>
		public int InsertWithAudit<TEntity>(TEntity entity, string user = "") where TEntity : BaseModel
		{
			entity.CreateTime = DateTime.Now;
			entity.UpdateTime = DateTime.Now;
			entity.CreateUser = user;
			entity.UpdateUser = user;
			entity.IsDelete = false;
			return this.CommandSet<TEntity>().Insert(entity);
		}

		/// <summary>
		/// 修改（自动填充 UpdateTime/UpdateUser）
		/// </summary>
		public int UpdateWithAudit<TEntity>(TEntity entity, string user = "") where TEntity : BaseModel
		{
			entity.UpdateTime = DateTime.Now;
			entity.UpdateUser = user;
			return this.CommandSet<TEntity>()
				.Where($"[{GetIdentityName<TEntity>()}] = @Id", new { Id = GetIdentityValue(entity) })
				.Update(entity);
		}

		/// <summary>
		/// 软删除（将 IsDelete 置为 true，需实体继承 BaseModel 且有 [Identity] 主键）
		/// </summary>
		public int SoftDelete<TEntity>(object id) where TEntity : BaseModel, new()
		{
			var identity = GetIdentityName<TEntity>();
			//软删除：UPDATE SET IsDelete=true WHERE 主键=id
			return this.CommandSet<TEntity>()
				.Where($"[{identity}] = @Id", new { Id = id })
				.Update(x => new TEntity { IsDelete = true });
		}

		/// <summary>
		/// 获取实体主键字段名（[Identity] 特性，找不到抛异常）
		/// </summary>
		private static string GetIdentityName<TEntity>() where TEntity : BaseModel
		{
			var entityObject = EntityCache.QueryEntity(typeof(TEntity));
			if (string.IsNullOrEmpty(entityObject.Identitys))
				throw new DapperExtensionException("主键不存在!请前往实体类使用[Identity]特性设置主键。");
			return entityObject.Identitys;
		}

		/// <summary>
		/// 获取实体主键值
		/// </summary>
		private static object GetIdentityValue<TEntity>(TEntity entity) where TEntity : BaseModel
		{
			var entityObject = EntityCache.QueryEntity(typeof(TEntity));
			return entityObject.EntityFieldList
				.First(x => x.IsIdentity).PropertyInfo
				.GetValue(entity);
		}
	}
}
