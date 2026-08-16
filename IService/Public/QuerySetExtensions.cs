using Kogel.Dapper.Extension.Core.Interfaces;
using Kogel.Dapper.Extension.Core.SetQ;
using Model;

namespace IService
{
    /// <summary>
    /// 软删除查询扩展：BaseModel 派生实体的查询自动过滤 IsDelete=false 的记录
    /// </summary>
    public static class QuerySetExtensions
    {
        /// <summary>
        /// 追加 IsDelete=false 过滤（软删除过滤）
        /// <para>注意：不能写成 !x.IsDelete，Kogel 表达式解析器会把取反生成为无效的 NOT (列) SQL</para>
        /// </summary>
        public static QuerySet<T> WhereNotDeleted<T>(this IQuerySet<T> querySet) where T : BaseModel
        {
            return querySet.Where(x => x.IsDelete == false);
        }
    }
}
