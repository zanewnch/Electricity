# Backend - ASP.NET Core Web API

## 📑 Index

- [🟢 Backend Analysis](#-backend-analysis)
  - [Source Files](#-source-files)
  - [Intention](#-intention)
  - [API Architecture](#-api-architecture)
  - [DB Schema](#-db-schema)
  - [Data Workflow](#-data-workflow)
- [🟡 啟動配置](#-啟動配置)
  - [Connection String](#connection-string)
  - [環境變數](#環境變數)
  - [服務埠](#服務埠)
- [🟣 Requirement Phases](#-requirement-phases)
  - [Phase 1: DbContext 與 Migration 配置](#phase-1-dbcontext-與-migration-配置)
  - [Phase 2: 自動 Migration（Development）](#phase-2-自動-migrationdevelopment)
  - [Phase 3: 初始控制器](#phase-3-初始控制器)
- [🟣 Implementation Steps](#-implementation-steps)
- [📄 相關文件](#-相關文件)

---

## 🟢 Backend Analysis

### 🟢 Source Files

- Entry Point: `backend/Program.cs`
- DbContext: `shared/Data/MqttDbContext.cs`
- Model: `shared/Models/SensorData.cs`
- Migrations: `backend/Migrations/`
  - `20260214180507_InitialCreate.cs`
  - `20260214180507_InitialCreate.Designer.cs`
  - `MqttDbContextModelSnapshot.cs`
- Configuration: `backend/appsettings.json`、`backend/appsettings.Development.json`
- Project: `backend/backend.csproj`

### 🟢 Intention

.NET 10 ASP.NET Core Web API，用於提供電力感測資料的 REST API 端點。使用 shared project 的 `MqttDbContext` 和 `SensorData` Model，Migration 檔案統一存放在 backend，開發時自動升級資料庫 Schema。

### 🟢 API Architecture

**層級架構：**

```
Client (Frontend / Postman)
    ↓
ASP.NET Core Web API (backend/Program.cs)
    ↓
Controllers (待實作，backend/Controllers/)
    ↓
Entity Framework Core (shared/Data/MqttDbContext.cs)
    ↓
SQL Server LocalDB (MqttDb)
```

**中間件管道：**
- Authorization
- HttpsRedirection
- Controller routing
- OpenAPI（Swagger）

### 🟢 DB Schema

**Table: `SensorData`**（由 Migration 創建）

| Column | Type | Nullable | 說明 |
|--------|------|----------|------|
| Id | int | No | 主鍵（自動遞增） |
| DeviceType | string | No | 裝置類型（EnergyMeter / Modbus） |
| BleAddress | string | No | BLE 裝置位址 |
| Current | double | No | 電流（A） |
| Voltage | double | No | 電壓（V） |
| Watt | double | No | 功率（W）= Current × Voltage |
| PowerFactor | double? | Yes | 功率因數（僅 EnergyMeter） |
| Frequency | double? | Yes | 頻率 Hz（僅 EnergyMeter） |
| Timestamp | DateTime | No | 資料時間戳 |

### 🟢 Data Workflow

```
Frontend 請求 API 端點
    ↓
ASP.NET Core 路由至 Controller → backend/Program.cs:27
    ↓
Controller 調用 MqttDbContext → shared/Data/MqttDbContext.cs:13
    ↓
EF Core 查詢 SensorData Table
    ↓
返回 JSON 至 Frontend
    ↓
(寫入流程) Frontend POST 新資料
    ↓
Controller 調用 db.SensorData.Add() + SaveChangesAsync()
    ↓
EF Core 執行 INSERT into SensorData
    ↓
返回 200 OK
```

---

## 🟡 啟動配置

### Connection String

**appsettings.json:**
```
Server=(localdb)\MSSQLLocalDB;Database=MqttDb;Trusted_Connection=true;TrustServerCertificate=true;
```

- **Server:** Local SQL Server Express (localdb)
- **Database:** MqttDb
- **Authentication:** Windows 驗證（Trusted Connection）
- **Certificate:** 信任伺服器憑證

### 環境變數

| 環境 | 值 | 說明 |
|------|-----|------|
| ASPNETCORE_ENVIRONMENT | Development | 開發環境，啟用 OpenAPI |
| ASPNETCORE_ENVIRONMENT | Production | 生產環境，禁用 OpenAPI |

### 服務埠

| 協定 | 埠號 | URL |
|------|------|-----|
| HTTP | 5086 | `http://localhost:5086` |
| HTTPS | 7105 | `https://localhost:7105` |

---

## 🟣 Requirement Phases

### 🟣 Phase 1: DbContext 與 Migration 配置

**Requirement**: 配置 Entity Framework Core，使用 shared 的 MqttDbContext，但 Migration 檔案統一存放在 backend 專案，便於 API 層統一管理資料庫 Schema。

**Modification Context**:
- **為什麼改**：DbContext 定義在 shared class library，但 Migration 不能在 class library 中執行，必須在執行程式中管理
- **怎麼改**：在 Program.cs 中設定 `MigrationsAssembly("backend")`，指定 Migration 檔案位置
- **影響範圍**：`backend/Program.cs`、`backend/Migrations/`

**程式碼變更**：

```csharp
// backend/Program.cs:7-10
builder.Services.AddDbContext<MqttDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("backend")));  // ← Migration 檔案位置
```

### 🟣 Phase 2: 自動 Migration（Development）

**Requirement**: 開發時，應用程式啟動時自動執行 pending migration，確保資料庫 Schema 與 Model 同步，無須手動執行 `dotnet ef database update`。

**Modification Context**:
- **為什麼改**：開發階段頻繁修改 Model，自動升級可加快開發流程
- **怎麼改**：在 Program.cs 中 `app.Build()` 之後、`app.MapControllers()` 之前添加自動 migration 邏輯
- **影響範圍**：`backend/Program.cs`（第 15-20 行之間）

**程式碼變更**：

```csharp
// backend/Program.cs（在 var app = builder.Build() 之後插入）
var app = builder.Build();

// 開發時自動升級資料庫
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<MqttDbContext>();
        db.Database.Migrate();  // ← 自動套用 pending migration
    }
}

// 後續中間件配置保持不變...
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
```

### 🟣 Phase 3: 初始控制器

**Requirement**: 建立初始 API 控制器，提供 GET /api/sensordata 端點查詢所有感測資料，及 POST 端點新增資料（供測試使用）。

**Modification Context**:
- **為什麼改**：提供前端與測試工具（Postman）呼叫 API 的入口
- **怎麼改**：在 `backend/Controllers/` 建立 `SensorDataController.cs`，實作 GetAll() 和 Create() 方法
- **影響範圍**：`backend/Controllers/SensorDataController.cs`（待建立）

**程式碼變更**：

```csharp
// backend/Controllers/SensorDataController.cs（待建立）
[ApiController]
[Route("api/[controller]")]
public class SensorDataController : ControllerBase
{
    private readonly MqttDbContext _context;

    public SensorDataController(MqttDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SensorData>>> GetAll()
    {
        return await _context.SensorData.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<SensorData>> Create(SensorData data)
    {
        _context.SensorData.Add(data);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = data.Id }, data);
    }
}
```

---

## 🟣 Implementation Steps

- [x] 建立 `backend/backend.csproj` - ASP.NET Core Web API 專案，.NET 10
- [x] 建立 `backend/Program.cs:7-10` - 配置 DbContext，指定 `MigrationsAssembly("backend")`
- [x] 建立 `backend/appsettings.json` - 設定 DefaultConnection 連線字串
- [x] 建立 `backend/Migrations/20260214180507_InitialCreate.cs` - 初始 Migration（建立 SensorData table）
- [ ] 實作 `backend/Program.cs:17-20` - 自動 Migration（Development 環境）
- [ ] 實作 `backend/Controllers/SensorDataController.cs` - GET /api/sensordata、POST /api/sensordata 端點

---

## 📄 相關文件

**Backend:**
- Entry Point: `backend/Program.cs`
- Configuration: `backend/appsettings.json`、`backend/appsettings.Development.json`
- Launch Settings: `backend/Properties/launchSettings.json`
- Project: `backend/backend.csproj`
- Migrations: `backend/Migrations/`

**Shared:**
- Model: `shared/Models/SensorData.cs`
- DbContext: `shared/Data/MqttDbContext.cs`
- Project: `shared/shared.csproj`

