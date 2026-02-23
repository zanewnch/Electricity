# Test Plan

> 版本：0.1
> 日期：2026-02-24
> 狀態：草稿（尚未實作任何測試）

---

## 測試策略

採用分層測試策略，依重要性由下而上：

```
         E2E Tests (少量，端對端驗證)
        ──────────────────────────────
       Integration Tests (API 層驗證)
      ────────────────────────────────
     Unit Tests (邏輯單元，快速、大量)
```

---

## 各層測試範圍

### 1. 單元測試（Unit Tests）

**目標：** 驗證獨立的業務邏輯，不依賴資料庫或外部服務。

| 測試對象 | 測試項目 |
|----------|----------|
| `DataGenerator` | EnergyMeter 數據的值域範圍正確（電壓 218-222V、功率因數 0.85-0.99） |
| `DataGenerator` | Modbus 數據的 PowerFactor / Frequency 為 null |
| `DataGenerator` | Watt = Current × Voltage（精度到小數點 2 位） |
| `SensorDataService`（待建） | 分頁邏輯正確（page / pageSize / total 計算） |
| `SensorDataService` | `granularity` 參數驗證（非法值應拋出例外） |

**工具：** xUnit（.NET 測試框架，`tests/` 目錄）

---

### 2. 整合測試（Integration Tests）

**目標：** 驗證 API 端點行為，使用 In-Memory 或 SQLite 測試資料庫，不依賴真實 SQL Server。

| 端點 | 測試項目 |
|------|----------|
| `GET /api/sensor-data` | 無篩選條件，回傳分頁結果 |
| `GET /api/sensor-data` | 依 `deviceType` 篩選，只回傳對應裝置 |
| `GET /api/sensor-data` | 依 `from`/`to` 篩選，只回傳時間範圍內的資料 |
| `GET /api/sensor-data/{id}` | 存在的 ID 回傳 200 + 正確資料 |
| `GET /api/sensor-data/{id}` | 不存在的 ID 回傳 404 |
| `GET /api/sensor-data/latest` | 回傳每台裝置各一筆最新資料 |
| `GET /api/sensor-data/aggregate` | `granularity=hour` 回傳正確聚合結構 |
| `GET /api/sensor-data/aggregate` | 非法 `granularity` 值回傳 400 |
| `GET /api/sensor-data/aggregate` | 各粒度（minute/hour/day/month）的 `period` 格式正確 |

**工具：** `WebApplicationFactory<Program>` + `Microsoft.EntityFrameworkCore.InMemory`

---

### 3. 前端元件測試（Component Tests）

**目標：** 驗證 Vue 元件的渲染行為與互動邏輯。

| 元件 | 測試項目 |
|------|----------|
| `DeviceCard` | 正確顯示電流、電壓、功率數值 |
| `DeviceCard` | EnergyMeter 顯示 PowerFactor；Modbus 不顯示 |
| `FilterBar` | 選擇 deviceType 後觸發正確 API 呼叫 |
| `AggregateChart` | 切換 granularity 後重新呼叫 API |

**工具：** Vitest + Vue Test Utils

---

### 4. E2E 測試（End-to-End）

**目標：** 驗證完整使用者流程，需後端服務運行中。

| 流程 | 測試項目 |
|------|----------|
| Dashboard 載入 | 頁面顯示各裝置最新數據 |
| 趨勢分析 | 切換粒度後圖表更新 |

**工具：** Playwright（待定）

---

## 覆蓋率目標

| 層 | 目標覆蓋率 |
|----|-----------|
| Backend 單元測試 | ≥ 80% |
| Backend 整合測試 | 所有 API 端點至少 1 個 happy path + 1 個 error case |
| Frontend 元件測試 | 核心元件（DeviceCard、AggregateChart）≥ 70% |
| E2E | 主要使用流程覆蓋即可 |

---

## 測試目錄結構（待建立）

```
tests/
├── backend/
│   ├── UnitTests/
│   │   ├── DataGeneratorTests.cs
│   │   └── SensorDataServiceTests.cs
│   └── IntegrationTests/
│       └── SensorDataControllerTests.cs
└── frontend/
    └── components/
        ├── DeviceCard.test.ts
        └── AggregateChart.test.ts
```
