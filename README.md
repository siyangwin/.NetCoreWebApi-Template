# .NET Core WebAPI 快速开发模板

基于 **ASP.NET Core 8** 的通用 WebAPI 模板（**AppApi=net8.0，库项目=net6.0**）。集成了双 Token 认证、API Key 认证、角色权限、Serilog 日志、Dapper ORM、Swagger 多版本文档等常用功能，开箱即用。

---

## 功能特性

- **双 Token 认证**：AccessToken(30min) + RefreshToken(7天)，支持自动续签
- **API Key 认证**：服务间/第三方调用，与 JWT 互斥
- **角色权限控制**：Admin/User/Guest，基于 `[Authorize(Roles)]`
- **多设备登录管理**：可配置单设备踢出或多设备同时在线
- **Refresh Token 安全**：SHA256 哈希存储 + 复用检测 + FamilyId 机制
- **日志身份追踪**：自动区分 JWT/API Key 请求来源
- **多语言支持**：JSON 语言文件 + 数据库数据翻译表，支持中英文等多语言
- **Swagger 多版本**：V1/V2 文档切换，支持双认证输入
- **Dapper ORM**：轻量数据访问（Kogel.Dapper）
- **软删除与审计字段**：自动填充审计字段，软删除 + 自动过滤
- **统一返回结构**：标准化 API 响应格式（含 apiVersion/timestamp）
- **.NET 8 特性**：C# 12 主构造函数、Keyed DI、IOptionsMonitor 热更新

---

## 快速开始

```bash
# 克隆项目
git clone https://github.com/siyangwin/.NetCoreWebApi-Template.git

# 配置数据库连接（appsettings.json → ConnectionStrings:SqlServer）

# 启动项目
dotnet run --project Project.AppApi --urls "http://localhost:5207"

# 访问接口文档
https://localhost:7034/index.html
```

**首次使用**：先调用 `POST /api/v1/auth/init` 初始化认证体系数据库表。

---

## 认证体系

本模板提供两种认证方式，**互斥使用**（同一请求只能用一种）。

### 1. JWT 双 Token 认证

适用于**用户端应用**（App/H5/小程序），通过登录获取 Token 进行接口认证。

#### 登录流程

```
POST /api/user/login
Body: {"account":"123","password":"admin"}

返回:
{
  "accessToken": "eyJ...",    // 短期令牌（30分钟）
  "refreshToken": "xxx...",   // 长期令牌（7天）
  "expiresIn": 1800           // AccessToken 过期秒数
}
```

#### 使用方式

```
GET /api/appuser/checkauthorizationinfo
Header: Authorization: Bearer {accessToken}
```

#### 刷新 Token

AccessToken 过期后，使用 RefreshToken 获取新的双 Token：

```
POST /api/user/refresh
Body: {"refreshToken":"xxx...","deviceId":"device-001"}

返回: 新的 accessToken + refreshToken
```

#### 前端刷新策略

后端 `/api/user/refresh` 接口行为统一，策略选择由前端决定：

**策略一：自动续签（推荐）**
- 前端每次请求后检查 AccessToken 剩余时间
- 剩余 < 5 分钟时自动调用 `/api/user/refresh`
- 用户全程无感知，适合 App/SPA

**策略二：被动刷新**
- 前端直接用 AccessToken 发请求
- 收到 401 后调用 `/api/user/refresh` 再重试
- 实现简单，适合后台管理系统

#### 登出

```
POST /api/user/logout
Body: {"refreshToken":"xxx..."}          // 吊销单个 Token
Body: {"refreshToken":"xxx...","allDevices":true}  // 吊销该用户所有 Token
```

---

### 2. API Key 认证

适用于**服务间调用**或**第三方接入**，无需用户登录。

#### 使用方式

```
GET /api/appuser/checknoAuthorizationinfo
Header: X-Api-Key: sk_test_xxxxx
```

#### 管理 API Key

```
POST /api/v1/auth/apikey        // 创建（返回明文，仅一次）
GET  /api/v1/auth/apikeys       // 查询列表（不含明文）
DELETE /api/v1/auth/apikey/{id} // 吊销/删除
```

**注意**：API Key 明文仅在创建时返回一次，请务必保存。数据库仅存储 SHA256 哈希。

---

### 3. 角色权限控制

基于 ASP.NET Core 的 `[Authorize(Roles)]` 机制。

```csharp
// 仅管理员可访问
[Authorize(Roles = "Admin")]
public class AdminController : BaseController { ... }

// 已登录用户即可
[Authorize(Roles = "User,Admin")]
public class ProfileController : BaseController { ... }

// 允许匿名
[AllowAnonymous]
public class PublicController : BaseController { ... }
```

**角色枚举**：
| 值 | 名称 | 说明 |
|----|------|------|
| 1 | Admin | 管理员 |
| 2 | User | 普通用户（默认） |
| 3 | Guest | 访客 |

---

### 4. 多设备登录管理

通过 `appsettings.json → AuthSettings` 配置：

```json
{
  "AuthSettings": {
    "AllowMultiLogin": true,        // true=多设备同时在线，false=新登录踢旧会话
    "MaxDevicesPerUser": 5,         // 多设备模式下每用户最大设备数
    "RefreshTokenFamilyExpiryDays": 90  // Token Family 有效期（天）
  }
}
```

**单设备模式**（`AllowMultiLogin: false`）：
- 新登录时自动吊销旧会话的所有 RefreshToken
- 旧设备下次请求时收到 401，需重新登录

**多设备模式**（`AllowMultiLogin: true`）：
- 各设备独立登录，互不影响
- 超过 `MaxDevicesPerUser` 时吊销最早的 Token

---

### 5. 安全机制

| 机制 | 说明 |
|------|------|
| **Token 哈希存储** | RefreshToken 和 API Key 仅存 SHA256 哈希，数据库泄露不影响明文 |
| **复用检测** | 已吊销的 RefreshToken 被再次使用 → 整个 Family 吊销（全家族失效） |
| **FamilyId** | 同一次登录的所有 Token 共享 FamilyId，用于复用检测和批量吊销 |
| **API Key 前缀** | `sk_test_`（开发）/ `sk_live_`（生产），便于识别和日志 |
| **密码校验** | 模板项目不校验密码（展示流程），实际项目请自行替换为 DB+BCrypt |

---

### 6. 日志身份追踪

所有请求日志自动记录认证类型和身份标识：

| 请求类型 | AuthType | AuthIdentity | UserId |
|----------|----------|--------------|--------|
| JWT 请求 | JWT | 123 | 123 |
| API Key 请求 | ApiKey | 订单服务 | 0 |
| 未认证请求 | （空） | （空） | 0 |

- `AuthType`：认证类型（JWT / ApiKey / 空）
- `AuthIdentity`：身份标识（万能字段，统一记录）
- `UserId`：保留字段（向后兼容），未来可用 `AuthIdentity` 替代

---

## 数据库初始化

首次使用需调用初始化接口创建认证体系表：

```bash
# 初始化认证体系表（RefreshToken + ApiKey + UserInfo.Role + SystemLog.AuthType/AuthIdentity）
POST /api/v1/auth/init

# 初始化用户表（软删除演示用）
POST /api/v2/demo/init
```

**表结构**：

| 表名 | 说明 |
|------|------|
| RefreshToken | 刷新令牌（SHA256 哈希存储） |
| ApiKey | API Key（SHA256 哈希存储） |
| UserInfo | 用户表（新增 Role 字段） |
| SystemLog | 系统日志（新增 AuthType/AuthIdentity 字段） |

---

## 多语言支持

本模板提供**两层独立的多语言支持**，分别解决不同场景的问题：

| 层级 | 解决什么问题 | 实现方式 | 存储位置 |
|------|-------------|----------|----------|
| **系统语言包** | 错误提示、成功消息、界面文案等**静态文字** | JSON 语言文件 + `Lang` 工具类 | `Resources/zh.json`、`en.json` |
| **DB 数据语言包** | 商品名称、文章标题等**业务内容** | 翻译表（Translation Table） | `I18nProductTranslation`、`I18nArticleTranslation` |

**核心区别**：
- **系统语言包**：代码中的字符串，编译后固定，通过 `Lang.Get("module:key")` 获取
- **DB 数据语言包**：数据库中的业务数据，运行时可增删改，通过 SQL LEFT JOIN 查询

### 1. 语言传递方式

两层多语言共用同一个语言参数，通过 `?language=zh` 或 `language` 请求头传递：

```
GET /api/demo?language=zh     # 中文
GET /api/demo?language=en     # 英文
GET /api/demo?language=ja     # 日文
Header: language: zh
```

**支持的传入格式**：

| 传入值 | 解析结果 | 说明 |
|--------|----------|------|
| `"zh"` | CN | 推荐 |
| `"zh-CN"` | CN | BCP 47 格式 |
| `"en"` | EN | 推荐（默认） |
| `"ja"` | JA | 推荐 |
| `"1"` | CN | 兼容旧格式 |
| 空/null | EN | 默认英文 |

### 2. 系统语言包（JSON 文件）

用于代码中的静态文字（错误提示、成功消息、界面文案等）：

**资源文件结构**：
```
Project.AppApi/Resources/
├── zh.json    # 中文
└── en.json    # 英文（默认）
```

**JSON 文件分模块组织**：
```json
{
  "common": { "success": "操作成功", "server_error": "服务器内部错误" },
  "auth": { "login_success": "登录成功", "logout_success": "登出成功" },
  "demo": { "hello": "Hello World！模板演示接口" },
  "validation": { "name_required": "Name 不能为空" },
  "db_demo": { "product_created": "商品创建成功" }
}
```

**使用方式**：
```csharp
// 简单获取
Lang.Get("common:success")                    // 当前语言
Lang.Get("auth:login_failed", LanguageEnum.CN) // 显式指定语言

// 带占位符
Lang.GetFormat("demo:deleted", 123)           // "已删除编号 123 的数据"
```

**三级降级**：当前语言 → 默认语言(en) → 返回 key 本身

**防呆机制**：语言文件缺失或 key 不存在时不会报错，自动降级到默认语言

**适用场景**：
- Controller 中的错误提示/成功消息
- Service 中的业务提示
- Filter/中间件中的错误消息
- ViewModel 验证消息

### 3. DB 数据语言包（翻译表模式）

用于数据库中的业务数据（商品名称、文章标题等）：

**表结构**：
```sql
-- 主表：存不可翻译的字段（价格、库存等）
I18nProduct (Id, Price, Stock, ImageUrl, IsDelete, CreatedAt)

-- 翻译表：存可翻译的字段（名称、描述等）
I18nProductTranslation (Id, ProductId, Language, Name, Description)
UNIQUE(ProductId, Language)  -- 同一商品同一语言只能有一条翻译
```

**查询方式**（LEFT JOIN）：
```sql
SELECT p.Id, p.Price, t.Name, t.Description
FROM I18nProduct p
LEFT JOIN I18nProductTranslation t ON p.Id = t.ProductId AND t.Language = 'zh'
WHERE p.IsDelete = 0
```

**加新语言**：无需改表结构，只需插入新数据

**适用场景**：
- 商品名称/描述
- 文章标题/内容
- 分类名称
- 任何需要多语言的业务数据

**API 接口**：

| 接口 | 方法 | 说明 |
|------|------|------|
| `POST /api/v2/i18n/init` | POST | 初始化建表（幂等） |
| `POST /api/v2/i18n/product` | POST | 创建商品（含翻译） |
| `GET /api/v2/i18n/product/{id}` | GET | 查询商品（返回当前语言翻译） |
| `GET /api/v2/i18n/products` | GET | 查询商品列表 |
| `POST /api/v2/i18n/product/{id}/translation` | POST | 添加/更新翻译 |
| `GET /api/v2/i18n/product/{id}/translations` | GET | 获取所有翻译 |
| `DELETE /api/v2/i18n/product/{id}` | DELETE | 软删除商品 |

文章接口同理（`/api/v2/i18n/article`）。

### 4. ViewModel 验证本地化

使用本地化验证 Attribute：
```csharp
[LocalizedRequired("validation:name_required")]
[LocalizedStringLength(50, "validation:name_max_length")]
[LocalizedRange(1, 120, "validation:age_range")]
[LocalizedRegularExpression(@"^[A-Za-z0-9_]+$", "validation:sort_field_invalid")]
public string Name { get; set; }
```

### 5. 添加新语言

1. 在 `Resources/` 下新建 `xx.json`（如 `ja.json`）
2. 在 `LanguageEnum` 中新增枚举值（如 `JA = 3`）
3. 在 `LanguageHelper._codeToEnum` 中添加映射
4. 完成，客户端用 `?language=ja` 即可

### 6. 添加新翻译 key

1. 在 `zh.json` 和 `en.json` 对应模块下同时添加
2. 在代码中使用 `Lang.Get("module:key")`

---

## 配置说明

```json
{
  "JwtConfig": {
    "SecretKey": "your_secret_key",  // JWT 签名密钥
    "Issuer": "your_issuer",          // 颁发者
    "Audience": "your_issuer",        // 接收者
    "AccessExpired": 30,              // AccessToken 过期（分钟）
    "RefreshExpired": 10080           // RefreshToken 过期（分钟，7天）
  },
  "AuthSettings": {
    "AllowMultiLogin": true,          // 多设备登录开关
    "MaxDevicesPerUser": 5,           // 最大设备数
    "RefreshTokenFamilyExpiryDays": 90 // Family 有效期（天）
  },
  "ApiKeySettings": {
    "HeaderName": "X-Api-Key",        // API Key 请求头名称
    "HashAlgorithm": "SHA256"         // 哈希算法
  }
}
```

---

## 接口总览

### 认证接口

| 接口 | 方法 | 说明 | 认证 |
|------|------|------|------|
| `/api/v1/auth/init` | POST | 初始化数据库表 | 无 |
| `/api/user/login` | POST | 登录（返回双 Token） | 无 |
| `/api/user/refresh` | POST | 刷新 Token | 无 |
| `/api/user/logout` | POST | 登出 | 无 |
| `/api/v1/auth/apikey` | POST | 创建 API Key | 无（模板） |
| `/api/v1/auth/apikeys` | GET | 查询 API Key 列表 | 无（模板） |
| `/api/v1/auth/apikey/{id}` | DELETE | 吊销 API Key | 无（模板） |

### V1 Demo 接口（26 个）

- `GET api/v1/demo/hello` — 最简返回
- `GET api/v1/demo/item/{id}` — 路径参数
- `GET api/v1/demo/list` — 分页
- `POST api/v1/demo/create` — 创建
- `GET api/v1/demo/cache` — 缓存示范
- `GET api/v1/demo/keyed-di` — .NET 8 Keyed DI
- 更多见 Swagger 文档

### V2 Demo 接口（5 个）

- `POST api/v2/demo/init` — 初始化表
- `POST api/v2/demo/user` — 新增（审计字段）
- `GET api/v2/demo/users` — 查询（软删除过滤）
- `DELETE api/v2/demo/user/{id}` — 软删除
- `GET api/v2/demo/users/all` — 全量查询

### V2 I18n 多语言接口（13 个）

- `POST api/v2/i18n/init` — 初始化多语言表
- `POST api/v2/i18n/product` — 创建商品（含翻译）
- `GET api/v2/i18n/product/{id}` — 查询商品（返回当前语言）
- `GET api/v2/i18n/products` — 商品列表
- `POST api/v2/i18n/product/{id}/translation` — 添加/更新翻译
- `GET api/v2/i18n/product/{id}/translations` — 获取所有翻译
- `DELETE api/v2/i18n/product/{id}` — 软删除商品
- `POST api/v2/i18n/article` — 创建文章（含翻译）
- `GET api/v2/i18n/article/{id}` — 查询文章
- `GET api/v2/i18n/articles` — 文章列表
- `POST api/v2/i18n/article/{id}/translation` — 添加/更新翻译
- `GET api/v2/i18n/article/{id}/translations` — 获取所有翻译
- `DELETE api/v2/i18n/article/{id}` — 软删除文章

---

## 项目结构

```
Project.AppApi/          启动项目（net8.0）
├── Controllers/         控制器
│   ├── AppUser/         认证相关（Login/Refresh/Logout）
│   ├── Auth/            API Key 管理 + 初始化
│   ├── Demo/            V1 Demo 示范
│   ├── Demo/            V2 Demo（软删除）
│   ├── Public/          BaseController
│   └── Cache/           缓存管理
├── Program.cs           配置/中间件/Serilog
└── appsettings.json     配置文件

Core/                    工具类（HttpHelper/MemoryCache/加密/Lang多语言/LanguageHelper）
IService/                服务接口（IRepository/IAppUserService 等）
Service/                 服务实现
Model/                   实体（BaseModel/UserInfo/SystemLog/RefreshToken/ApiKey/I18n）
ViewModel/               DTO（ResultModel/LoginResDto/RefreshReqDto/I18n 等）
MvcCore.Extension/       扩展模块
├── Auth/                认证（GenerateJwt/JwtConfig/RefreshTokenService/ApiKeyService/ApiKeyMiddleware）
├── Filter/              过滤器（ApiFilterAttribute — apiVersion 统一设置 + 日志）
├── Swagger/             Swagger 多版本配置
└── MemoryCacheHelper    缓存工具
Kogel.Dapper.Extension/  内嵌第三方 ORM（不修改）
Kogel.Repository/        内嵌第三方 ORM（不修改）
```

---

## 后续计划

- 支持 SignalR 实时通信
- 集成 Redis 缓存/分布式锁/延迟队列
- 集成消息队列（RabbitMQ / Kafka）
- 引入任务调度（Quartz.NET）
