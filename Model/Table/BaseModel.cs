using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// 公共的类
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 主键 ID，自增列
        /// </summary>
        [Identity]
        public virtual int Id { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public virtual string UpdateUser { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime UpdateTime { get; set; }
        /// <summary>
        /// 删除标志（已经删除[True]=1    未删除[False]=0）
        /// </summary>
        public virtual bool IsDelete { get; set; }
    }
}
