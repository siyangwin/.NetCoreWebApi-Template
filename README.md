# 🌐 .NET Core WebAPI 快速开发模板

这是一个基于 **.NET Core WebAPI** 的通用开发模板（**.NET 6.0 / 8.0**）。项目集成了 JWT 鉴权、Serilog 日志、Dapper ORM、Swagger 文档、Excel 导入导出、封装的 Http 请求/缓存操作/邮件发送等常用功能，统一注入 Services，涵盖大多数企业开发场景，开箱即用。

---

## ✨ 功能特性

- 🔐 **JWT 身份验证**：权限控制与用户认证（校验签名/有效期/颁发者/受众）
- 📄 **Swagger UI**：集成接口文档，调试更方便
- 📦 **Dapper ORM**：轻量数据访问组件（Kogel.Dapper）
- 📊 **Excel 导入导出**：支持批量数据处理
- 🧾 **Serilog 日志系统**：自动记录所有请求日志（控制台 + 文件 + SQL Server）
- 🌐 **Http 请求封装**：基于 HttpClientFactory，简化请求逻辑
- 🧠 **缓存操作封装**：支持本地缓存（系统缓存 / 程序缓存分离）
- 🔒 **内存锁机制**：处理并发一致性问题
- ✉️ **邮件发送支持**：SMTP 邮件推送功能
- 🧰 **常用工具类封装**：简化开发流程
- ⚠️ **统一异常处理**：集中式异常捕获与日志记录
- 📦 **统一返回结构**：标准化 API 响应格式（含时间戳）
- 🧑‍💻 **Demo 示范控制器**：`api/v1/demo/*` 提供常见请求写法的示例代码

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
- `Kogel.*` — 内嵌第三方 ORM（Dapper 扩展）

**常用接口**：
- `POST api/user/login` — 登录获取 JWT（测试账号：任意，示例不校验密码）
- `GET api/v1/demo/*` — 示范接口（分页/上传/缓存/认证/异常等）
- `GET api/cache/*` — 缓存管理（需鉴权）

---

## 🔮 后续计划

- 多语言支持（基于语言文件的国际化方案）
- JWT 高级机制（刷新 Token、黑名单吊销、权限粒度控制）
- 支持 SignalR 实时通信
- 集成 Redis 缓存/分布式锁/延迟队列
- 集成 消息队列（RabbitMQ / Kafka）
- 引入 任务调度（Quartz.NET）
- 支持 模块化插件架构
