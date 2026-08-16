# 🌐 .NET Core WebAPI 快速开发模板

这是一个基于 **.NET Core WebAPI** 的通用开发模板（**.NET 6.0 / 8.0**）。项目集成了 JWT 鉴权、Serilog 日志、Dapper ORM、Swagger 多版本文档、Excel 导入导出、封装的 Http 请求/缓存操作/邮件发送等常用功能，统一注入 Services，涵盖大多数企业开发场景，开箱即用。

---

## ✨ 功能特性

- 🔐 **JWT 身份验证**：权限控制与用户认证（校验签名/有效期/颁发者/受众，令牌无法伪造）
- 📄 **Swagger 多版本**：V1/V2 文档下拉切换，接口按版本分组
- 📦 **Dapper ORM**：轻量数据访问组件（Kogel.Dapper）
- 🗑️ **软删除与审计字段**：BaseModel 实体新增/修改自动填充审计字段，软删除 + 自动过滤已删除记录
- 📊 **Excel 导入导出**：支持批量数据处理
- 🧾 **Serilog 日志系统**：自动记录所有请求日志（控制台 + 文件 + SQL Server，异步批量写库）
- 🌐 **Http 请求封装**：基于 HttpClientFactory，线程安全，简化请求逻辑
- 🧠 **缓存操作封装**：支持本地缓存（系统缓存 / 程序缓存分离）
- 🔒 **内存锁机制**：处理并发一致性问题
- ✉️ **邮件发送支持**：SMTP 邮件推送功能
- 🧰 **常用工具类封装**：简化开发流程
- ⚠️ **统一异常处理**：集中式异常捕获与日志记录
- 📦 **统一返回结构**：标准化 API 响应格式（含时间戳、apiVersion）
- 🧩 **.NET 8 特性**：C# 12 主构造函数、Keyed DI（按 key 注入不同实现）、IOptionsMonitor 配置热更新
- 🧑‍💻 **Demo 示范控制器**：`api/v1/demo/*` 与 `api/v2/demo/*` 提供常见请求写法与软删除示例

---

## 🧑‍💻 适合人群

- 中小型项目启动开发者  
- 想要快速构建企业后端服务的团队  
- 需要参考规范化代码结构的初中级开发者  
- 希望复用通用模块、减少重复造轮子的工程师  

---

## 🚀 快速开始

```bash
# 克隆项目
git clone https://github.com/siyangwin/.NetCoreWebApi-Template.git

# 配置数据库连接、JWT 密钥等参数（appsettings.json）

# 启动项目，访问接口文档
https://localhost:7034/index.html
```

**项目结构**：
- `Project.AppApi` — 启动项目（net8.0），Controller、中间件、配置
- `Core` / `IService` / `Service` / `Model` / `ViewModel` — 工具类、服务接口与实现、实体、DTO（net6.0）
- `MvcCore.Extension` — JWT、过滤器、Swagger、缓存等扩展
- `Kogel.*` — 内嵌第三方 ORM（Dapper 扩展，内部不修改）

---

## 🔌 常用接口

**认证**：
- `POST api/user/login` — 登录获取 JWT（测试账号：任意，示例不校验密码）
- `GET api/appuser/checkauthorizationinfo` — 带 token 查看当前用户（需鉴权）

**V1 Demo 示范（模拟数据，无需数据库）**：
- `GET api/v1/demo/hello` — 最简统一返回结构
- `GET api/v1/demo/item/{id}` — 路径参数
- `GET api/v1/demo/list?pageIndex=1&pageSize=10` — 分页（pageSize 限 1-100，sort 白名单防注入）
- `GET api/v1/demo/query?keyword=xx` — QueryString 参数
- `POST api/v1/demo/create` — JSON Body + DTO 校验（Required/Range）
- `PUT api/v1/demo/update/{id}`、`DELETE api/v1/demo/delete/{id}` — 更新/删除
- `POST api/v1/demo/upload` — 文件上传（保存到 other/ 目录，通过 /other 访问）
- `GET api/v1/demo/cache` — 缓存 Set/Get 示范
- `GET api/v1/demo/language` — 语言读取（请求头 Language，CN/EN）
- `GET api/v1/demo/authorize` — JWT 认证 + 从 claims 读 UserId（需鉴权）
- `GET api/v1/demo/error` — 异常 → 全局异常中间件 500
- `GET api/v1/demo/keyed-di` — .NET 8 Keyed DI 示范
- `GET api/v1/demo/version` — 版本信息

**V2 Demo 示范（软删除 + 审计字段，需数据库有 UserInfo 表）**：
- `POST api/v2/demo/init` — 初始化 UserInfo 表（幂等）
- `POST api/v2/demo/user?name=xx` — 新增（InsertWithAudit 自动填审计字段）
- `GET api/v2/demo/users` — 查询（WhereNotDeleted 自动过滤已删除）
- `DELETE api/v2/demo/user/{id}` — 软删除（IsDelete=true，不物理删除）
- `PUT api/v2/demo/user/{id}?name=xx` — 修改（UpdateWithAudit 自动填 UpdateTime）
- `GET api/v2/demo/users/all` — 全量查询（含已删除，对比用）

**其他**：
- `GET api/cache/*` — 缓存管理（需鉴权）

---

## 🧩 .NET 8 新特性示范

- **Keyed DI**：`AddKeyedScoped<IKeyedDemoService, KeyedServiceA>("a")` 注册，接口用 `[FromKeyedServices("a")]` 按 key 注入（见 `api/v1/demo/keyed-di`）
- **主构造函数**：`AppUserController(IAppUserService svc, GenerateJwt jwt) : BaseController`（C# 12）
- **IOptionsMonitor**：`GenerateJwt` 注入 `IOptionsMonitor<JwtConfig>`，修改 appsettings 后无需重启即生效

---

## 🗂️ 统一返回结构与 apiVersion

所有接口返回 `ResultModel` / `ResultModel<T>` / `ResultPageModel<T>`（基类 `ApiResult`）：

```json
{
  "data": "...",
  "apiVersion": "v1",
  "success": true,
  "code": "200",
  "message": null,
  "timestamp": "2026-08-16T18:22:48+08:00"
}
```

- `apiVersion` 由全局过滤器自动设置：请求路径含 `/api/vN/` 返回对应版本，否则默认 `v1`
- `timestamp` 默认当前时间

---

## 🔮 后续计划

- 多语言支持（基于语言文件的国际化方案）
- JWT 高级机制（刷新 Token、黑名单吊销、权限粒度控制）
- 支持 SignalR 实时通信
- 集成 Redis 缓存/分布式锁/延迟队列
- 集成 消息队列（RabbitMQ / Kafka）
- 引入 任务调度（Quartz.NET）
- 支持 模块化插件架构
