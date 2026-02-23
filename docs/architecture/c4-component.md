# C4 — Component Diagram

展開各容器的內部元件結構。

---

## Backend API 元件

```mermaid
C4Component
    title Backend API — Component Diagram

    Container_Ext(frontend, "Frontend", "Vue 3", "呼叫 API")
    ContainerDb_Ext(db, "Database", "SQL Server", "SensorData 資料表")

    Container_Boundary(backend, "Backend API") {
        Component(controller, "SensorDataController", "ASP.NET Core Controller", "處理 HTTP 請求，路由至對應端點\nGET /api/sensor-data/*")
        Component(service, "SensorDataService", "C# Service", "封裝查詢邏輯、聚合計算、分頁處理")
        Component(dbContext, "MqttDbContext", "EF Core DbContext", "資料庫存取層\n定義於 shared/Data/")
        Component(model, "SensorData", "C# Model", "感測器資料實體\n定義於 shared/Models/")
    }

    Rel(frontend, controller, "HTTP GET", "JSON")
    Rel(controller, service, "呼叫")
    Rel(service, dbContext, "LINQ 查詢")
    Rel(dbContext, db, "SQL", "EF Core")
    Rel(dbContext, model, "對應")
```

> `SensorDataService` 目前尚未建立，需實作。

---

## Collector 元件

```mermaid
C4Component
    title Collector — Component Diagram

    ContainerDb_Ext(db, "Database", "SQL Server", "SensorData 資料表")

    Container_Boundary(collector, "Collector") {
        Component(program, "Program.cs", "Entry Point", "主迴圈：每 3.5 秒觸發一次\n處理 Ctrl+C 優雅退出")
        Component(generator, "DataGenerator", "C# Service", "產生 EnergyMeter 與 Modbus 的模擬數據")
        Component(dbContext, "MqttDbContext", "EF Core DbContext", "與 Backend 共用\n定義於 shared/Data/")
    }

    Rel(program, generator, "呼叫 Generate()")
    Rel(program, dbContext, "呼叫 SaveChangesAsync()")
    Rel(dbContext, db, "INSERT SensorData", "EF Core / SQL")
```

---

## Frontend 元件

```mermaid
C4Component
    title Frontend — Component Diagram

    Container_Ext(backend, "Backend API", "ASP.NET Core", "REST API")

    Container_Boundary(frontend, "Frontend") {
        Component(router, "Vue Router", "Routing", "管理頁面路由：/ / /history / /analysis")
        Component(dashboard, "DashboardView", "Vue Component", "即時數據總覽\n呼叫 /api/sensor-data/latest")
        Component(history, "HistoryView", "Vue Component", "歷史資料查詢與分頁表格\n呼叫 /api/sensor-data")
        Component(analysis, "AnalysisView", "Vue Component", "時間聚合趨勢圖（核心）\n呼叫 /api/sensor-data/aggregate")
        Component(apiService, "sensorDataService.ts", "TypeScript Service", "封裝所有 API 呼叫，統一管理 Base URL")
    }

    Rel(router, dashboard, "路由 /")
    Rel(router, history, "路由 /history")
    Rel(router, analysis, "路由 /analysis")
    Rel(dashboard, apiService, "呼叫")
    Rel(history, apiService, "呼叫")
    Rel(analysis, apiService, "呼叫")
    Rel(apiService, backend, "HTTP GET", "JSON")
```

> Frontend 元件目前尚未實作（App.vue 為空），需安裝 vue-router 後實作。

---

## 資料模型

### SensorData（`shared/Models/SensorData.cs`）

| 欄位 | C# 型別 | SQL 型別 | Nullable | 說明 |
|------|---------|----------|----------|------|
| `Id` | `int` | `int IDENTITY(1,1)` | 否 | 主鍵 |
| `DeviceType` | `string` | `nvarchar(max)` | 否 | `EnergyMeter` / `Modbus` |
| `BleAddress` | `string` | `nvarchar(max)` | 否 | MAC 位址 |
| `Current` | `double` | `float` | 否 | 電流 (A) |
| `Voltage` | `double` | `float` | 否 | 電壓 (V) |
| `Watt` | `double` | `float` | 否 | 功率 (W) |
| `PowerFactor` | `double?` | `float` | 是 | 功率因數 |
| `Frequency` | `double?` | `float` | 是 | 頻率 (Hz) |
| `Timestamp` | `DateTime` | `datetime2` | 否 | 量測時間 |
