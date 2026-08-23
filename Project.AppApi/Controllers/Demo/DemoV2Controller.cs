using Dapper;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Table;
using ViewModel;
using Core;

namespace Project.AppApi.Controllers
{
    /// <summary>
    /// Demo 示范控制器 V2（模板示例）
    /// <para>说明：V2 版本示例：演示 BaseModel 审计字段自动填充 + 软删除（InsertWithAudit / SoftDelete / WhereNotDeleted）。</para>
    /// <para>前置：需要数据库中有 UserInfo 表（继承 BaseModel 的实体）。</para>
    /// </summary>
    [ApiExplorerSettings(GroupName = "V2")]
    [Route("api/v2/demo")]
    public class DemoV2Controller : BaseController
    {
        private readonly IRepository connection;

        public DemoV2Controller(IRepository connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// V2-示范0：初始化 UserInfo 表（演示软删除所需表结构，幂等）
        /// </summary>
        /// <remarks>
        /// 演示：使用 Dapper 直接执行建表 SQL。
        /// 表结构对应 UserInfo : BaseModel（含审计字段 CreateTime/UpdateTime/CreateUser/UpdateUser/IsDelete）。
        /// </remarks>
        [HttpPost("init")]
        [AllowAnonymous]
        public ResultModel InitUserInfoTable()
        {
            string sql = @"
IF OBJECT_ID('UserInfo') IS NULL
BEGIN
    CREATE TABLE UserInfo (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Avatar NVARCHAR(200) NULL,
        [Name] NVARCHAR(50) NOT NULL,
        WechatOpenid NVARCHAR(100) NULL,
        TikTokOpenid NVARCHAR(100) NULL,
        CreateUser NVARCHAR(50) NULL,
        CreateTime DATETIME NOT NULL DEFAULT GETDATE(),
        UpdateUser NVARCHAR(50) NULL,
        UpdateTime DATETIME NOT NULL DEFAULT GETDATE(),
        IsDelete BIT NOT NULL DEFAULT 0
    );
END";
            connection.Orm.Execute(sql);
            return new ResultModel().SetMessage(Lang.Get("demo:init_success"));
        }

        /// <summary>
        /// V2-示范1：新增用户（自动填充审计字段 CreateTime/UpdateTime/CreateUser/UpdateUser，IsDelete=false）
        /// </summary>
        /// <param name="name">姓名</param>
        /// <remarks>
        /// 演示 InsertWithAudit：不需要手动设置 BaseModel 的审计字段，方法内部自动填充。
        /// CreateUser/UpdateUser 取当前登录用户（UserId），未登录为 0。
        /// </remarks>
        [HttpPost("user")]
        [AllowAnonymous]
        public ResultModel<int> InsertUser(string name)
        {
            ResultModel<int> resultModel = new ResultModel<int>();
            if (string.IsNullOrWhiteSpace(name))
            {
                return resultModel.SetMessage(Lang.Get("demo:name_empty"), false);
            }

            UserInfo user = new UserInfo { Name = name };
            //审计字段由 InsertWithAudit 自动填充
            int id = connection.InsertWithAudit(user, UserId.ToString());
            resultModel.Data = id;
            resultModel.SetMessage(Lang.GetFormat("demo:create_success", id));
            return resultModel;
        }

        /// <summary>
        /// V2-示范2：查询全部用户（WhereNotDeleted 软删除过滤：只返回 IsDelete=false 的记录）
        /// </summary>
        /// <remarks>
        /// 演示 WhereNotDeleted 扩展：普通查询会自动过滤已删除记录，
        /// 避免每个查询都手写 x =&gt; !x.IsDelete。
        /// </remarks>
        [HttpGet("users")]
        [AllowAnonymous]
        public ResultModel<List<UserInfo>> GetUsers()
        {
            ResultModel<List<UserInfo>> resultModel = new ResultModel<List<UserInfo>>();
            resultModel.Data = connection.QuerySet<UserInfo>()
                .WhereNotDeleted() //软删除过滤
                .ToList();
            return resultModel;
        }

        /// <summary>
        /// V2-示范3：软删除（SoftDelete：将 IsDelete 置为 true，不物理删除）
        /// </summary>
        /// <param name="id">用户主键</param>
        /// <remarks>
        /// 演示 SoftDelete：执行 UPDATE SET IsDelete=true WHERE Id=id。
        /// 软删除后 WhereNotDeleted 查询将不再返回该记录，但数据仍在表中。
        /// </remarks>
        [HttpDelete("user/{id}")]
        [AllowAnonymous]
        public ResultModel SoftDeleteUser(int id)
        {
            int rows = connection.SoftDelete<UserInfo>(id);
            return new ResultModel().SetMessage(rows > 0 ? Lang.GetFormat("demo:delete_success", id) : Lang.Get("demo:user_not_found"), rows > 0);
        }

        /// <summary>
        /// V2-示范4：修改用户（UpdateWithAudit：自动填充 UpdateTime/UpdateUser）
        /// </summary>
        /// <param name="id">用户主键</param>
        /// <param name="name">新姓名</param>
        [HttpPut("user/{id}")]
        [AllowAnonymous]
        public ResultModel UpdateUser(int id, string name)
        {
            UserInfo user = connection.QuerySet<UserInfo>()
                .Where(x => x.Id == id)
                .Get();
            if (user == null)
            {
                return new ResultModel().SetMessage(Lang.Get("demo:user_not_found"), false);
            }
            user.Name = name;
            //审计字段 UpdateTime/UpdateUser 由 UpdateWithAudit 自动填充
            connection.UpdateWithAudit(user, UserId.ToString());
            return new ResultModel().SetMessage(Lang.Get("demo:update_success"));
        }

        /// <summary>
        /// V2-示范5：查看软删除后的全部数据（含已删除，演示对比）
        /// </summary>
        /// <remarks>
        /// 不加 WhereNotDeleted 过滤，可看到软删除记录仍在表中（IsDelete=true）
        /// </remarks>
        [HttpGet("users/all")]
        [AllowAnonymous]
        public ResultModel<List<UserInfo>> GetAllUsers()
        {
            ResultModel<List<UserInfo>> resultModel = new ResultModel<List<UserInfo>>();
            resultModel.Data = connection.QuerySet<UserInfo>().ToList();
            return resultModel;
        }
    }
}
