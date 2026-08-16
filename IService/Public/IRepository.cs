using Kogel.Repository.Interfaces;
using Model;

namespace IService
{
	public interface IRepository : IBaseRepository<IRepository>,IBaseService
	{
		/// <summary>
		/// 新增（自动填充审计字段：CreateTime/UpdateTime/CreateUser/UpdateUser，IsDelete=false）
		/// </summary>
		int InsertWithAudit<TEntity>(TEntity entity, string user = "") where TEntity : BaseModel;

		/// <summary>
		/// 修改（自动填充 UpdateTime/UpdateUser）
		/// </summary>
		int UpdateWithAudit<TEntity>(TEntity entity, string user = "") where TEntity : BaseModel;

		/// <summary>
		/// 软删除（将 IsDelete 置为 true，需实体继承 BaseModel 且有 [Identity] 主键）
		/// </summary>
		int SoftDelete<TEntity>(object id) where TEntity : BaseModel, new();
	}
}
