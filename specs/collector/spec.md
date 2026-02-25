# Collector - Dummy Data Generator

## 📑 Index

- [🟢 Backend Analysis](#-backend-analysis)
  - [Source Files](#-source-files)
  - [Intention](#-intention)
  - [DB Schema](#-db-schema)
  - [Data Workflow](#-data-workflow)
- [🟡 資料結構](#-資料結構)
  - [EnergyMeter 裝置](#energymeter-裝置)
  - [Modbus 裝置](#modbus-裝置)
- [🟣 Requirement Phases](#-requirement-phases)
  - [Phase 1: 資料產生器](#phase-1-資料產生器)
  - [Phase 2: 資料庫寫入](#phase-2-資料庫寫入)
  - [Phase 3: 優雅退出機制](#phase-3-優雅退出機制)
- [🟣 Implementation Steps](#-implementation-steps)
- [📄 相關文件](#-相關文件)

---

## 🟢 Backend Analysis

### 🟢 Source Files

- Entry Point: `collector/Program.cs`
- Service: `collector/Services/DataGenerator.cs`
- Model: `shared/Models/SensorData.cs`
- DbContext: `shared/Data/MqttDbContext.cs`
- Project: `collector/collector.csproj`

### 🟢 Intention

.NET 10 Console App，用於模擬電力感測器資料收集。每 3.5 秒產生兩筆 dummy sensor data（EnergyMeter + Modbus），印出 JSON 至 Console 並寫入 SQL Server LocalDB，供前端與後端開發測試使用。

### 🟢 DB Schema

**Table: `SensorData`**

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

**Connection String:**
```
Server=(localdb)\MSSQLLocalDB;Database=MqttDb;Trusted_Connection=true;TrustServerCertificate=true;
```

### 🟢 Data Workflow

**Workflow A: Phase 1-3（已完成）- 即時寫入（臨時方案）**

```
DataGenerator.Generate()                    → collector/Services/DataGenerator.cs:12
    ↓
建立 2 筆 SensorData（EnergyMeter + Modbus）→ collector/Services/DataGenerator.cs:17-46
    ↓
DataGenerator.PrintReadings()               → collector/Services/DataGenerator.cs:49
    ↓
Console JSON 輸出                            → collector/Services/DataGenerator.cs:53-54
    ↓
MqttDbContext.SensorData.AddRange()         → collector/Program.cs:32
    ↓
await db.SaveChangesAsync()                 → collector/Program.cs:33
    ↓
Task.Delay(3500ms)                          → collector/Program.cs:38
    ↓
Loop（直到 Ctrl+C 觸發 CancellationToken）
```

**Workflow B: Phase 4（待實作）- 批次寫入（60 秒）**

```
[每 3.5 秒] DataGenerator.Generate()        → collector/Services/DataGenerator.cs:12
    ↓
建立 2 筆 SensorData
    ↓
DataGenerator.PrintReadings()               → collector/Services/DataGenerator.cs:49
    ↓
Console JSON 輸出
    ↓
DataBufferService.Enqueue(readings)         → collector/Services/DataBufferService.cs（待實作）
    ↓
存入 ConcurrentQueue（記憶體 buffer）
    ↓
Task.Delay(3500ms)
    ↓
[每 60 秒] DataBufferService.FlushToDatabase()
    ↓
批次取出 buffer 中的所有資料
    ↓
MqttDbContext.SensorData.AddRange(batch)
    ↓
await db.SaveChangesAsync()
    ↓
Console 輸出「✅ 批次寫入 N 筆資料」
    ↓
清空 buffer
    ↓
Ctrl+C 時觸發最後一次 FlushToDatabase()
```

---

## 🟡 資料結構

### EnergyMeter 裝置

| 欄位 | 範圍 | 說明 |
|------|------|------|
| DeviceType | `"EnergyMeter"` | 固定值 |
| BleAddress | `"B0:C4:11:22:33:01"` | 固定 BLE 位址 |
| Current | 0 ~ 100 A | 隨機產生，保留 2 位小數 |
| Voltage | 218 ~ 222 V | 穩定電壓範圍 |
| Watt | Current × Voltage | 計算值 |
| PowerFactor | 0.85 ~ 0.99 | 功率因數 |
| Frequency | 59.9 ~ 60.1 Hz | 頻率 |

### Modbus 裝置

| 欄位 | 範圍 | 說明 |
|------|------|------|
| DeviceType | `"Modbus"` | 固定值 |
| BleAddress | `"B0:C4:11:22:33:02"` | 固定 BLE 位址 |
| Current | 0 ~ 30 A | 工業用小電流 |
| Voltage | 0 ~ 480 V | 工業電壓範圍較廣 |
| Watt | Current × Voltage | 計算值 |
| PowerFactor | null | 無此欄位 |
| Frequency | null | 無此欄位 |

---

## 🟣 Requirement Phases

### 🟣 Phase 1: 資料產生器 ✅

**Requirement**: 建立 `DataGenerator` 服務，模擬兩種電力裝置（EnergyMeter、Modbus）的感測數據，產生符合實際範圍的 dummy data。

**Modification Context**:
- **為什麼改**：開發與測試需要持續性的模擬資料，避免依賴實體裝置
- **怎麼改**：建立 `DataGenerator` class，內含 `Generate()` 與 `PrintReadings()` 方法
- **影響範圍**：`collector/Services/DataGenerator.cs`

**程式碼變更**：

```csharp
// collector/Services/DataGenerator.cs
public List<SensorData> Generate()
{
    // EnergyMeter：穩定電壓 ~220V、大電流、有 PowerFactor 和 Frequency
    // Modbus：工業電壓範圍較廣、小電流、無 PowerFactor 和 Frequency
}

public void PrintReadings(List<SensorData> readings)
{
    // JSON 格式化輸出至 Console
}
```

**狀態**：✅ 已完成

---

### 🟣 Phase 2: 即時資料庫寫入（臨時方案） ✅

**Requirement**: 將產生的 sensor data 透過 EF Core 寫入 SQL Server LocalDB，使用 shared project 的 `MqttDbContext`。（該方案將在 Phase 4 被替換為批次寫入）

**Modification Context**:
- **為什麼改**：資料需持久化至資料庫，供後端 API 查詢與前端展示
- **怎麼改**：在主迴圈中建立 `MqttDbContext`，使用 `AddRange` + `SaveChangesAsync` 寫入
- **影響範圍**：`collector/Program.cs`、`shared/Data/MqttDbContext.cs`

**程式碼變更**：

```csharp
// collector/Program.cs:30-34（臨時實作，將被取代）
using (var db = new MqttDbContext(options))
{
    db.SensorData.AddRange(readings);
    await db.SaveChangesAsync(cts.Token);
}
```

**狀態**：✅ 已完成（臨時方案，待取代）

---

### 🟣 Phase 3: 優雅退出機制 ✅

**Requirement**: 支援 Ctrl+C 優雅退出，確保資料寫入完成後才停止程式。

**Modification Context**:
- **為什麼改**：避免資料寫入中途被強制中斷，確保資料一致性
- **怎麼改**：使用 `CancellationTokenSource` + `Console.CancelKeyPress` 事件處理
- **影響範圍**：`collector/Program.cs`

**程式碼變更**：

```csharp
// collector/Program.cs:14-20
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\n停止收值...");
};
```

**狀態**：✅ 已完成

---

### 🟣 Phase 4: 資料匯集與批次寫入 ⏳

**Requirement**: 將每 3.5 秒產生的 sensor data 先存入記憶體 buffer，每 60 秒批次寫入資料庫，降低 DB I/O 頻率，提升效能。

**Modification Context**:
- **為什麼改**：目前每 3.5 秒寫入一次 DB，頻繁 I/O 影響效能；批次寫入可大幅降低資料庫負擔
- **怎麼改**：建立 `DataBufferService` 類別管理記憶體 buffer（ConcurrentQueue），使用 Timer 每 60 秒批次寫入
- **影響範圍**：`collector/Services/DataBufferService.cs`（新增）、`collector/Program.cs`（修改）

**程式碼變更**：

```csharp
// collector/Services/DataBufferService.cs（新增）
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Shared.Data;
using Shared.Models;

namespace Collector.Services;

public class DataBufferService : IAsyncDisposable
{
    private readonly ConcurrentQueue<SensorData> _buffer = new();
    private readonly MqttDbContext _context;
    private readonly Timer _flushTimer;

    public DataBufferService(MqttDbContext context)
    {
        _context = context;
        // 每 60 秒執行一次批次寫入
        _flushTimer = new Timer(FlushCallback, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
    }

    /// <summary>
    /// 將產生的資料加入 buffer
    /// </summary>
    public void Enqueue(List<SensorData> readings)
    {
        foreach (var reading in readings)
            _buffer.Enqueue(reading);
    }

    /// <summary>
    /// 定時回調函式，觸發批次寫入
    /// </summary>
    private async void FlushCallback(object? state)
    {
        await FlushToDatabase();
    }

    /// <summary>
    /// 批次寫入 buffer 中的所有資料到資料庫
    /// </summary>
    public async Task FlushToDatabase()
    {
        var batch = new List<SensorData>();

        // 取出 buffer 中的所有資料
        while (_buffer.TryDequeue(out var data))
            batch.Add(data);

        // 如果有資料，執行批次寫入
        if (batch.Count > 0)
        {
            _context.SensorData.AddRange(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ [{DateTime.Now:HH:mm:ss}] 批次寫入 {batch.Count} 筆資料");
        }
    }

    /// <summary>
    /// 停止 Timer 並清理資源
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _flushTimer?.Dispose();
        await _context.DisposeAsync();
    }
}

// collector/Program.cs（修改）
using Microsoft.EntityFrameworkCore;
using Collector.Services;
using Shared.Data;

var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MqttDb;Trusted_Connection=true;TrustServerCertificate=true;";

var options = new DbContextOptionsBuilder<MqttDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var generator = new DataGenerator();
using var bufferService = new DataBufferService(new MqttDbContext(options));

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\n停止收值...");
};

Console.WriteLine("開始產生 dummy data（每 3.5 秒一次，按 Ctrl+C 停止）\n");

while (!cts.Token.IsCancellationRequested)
{
    var readings = generator.Generate();
    generator.PrintReadings(readings);

    bufferService.Enqueue(readings);  // ← 放入 buffer，而非立即寫入

    try
    {
        await Task.Delay(3500, cts.Token);
    }
    catch (TaskCanceledException)
    {
        break;
    }
}

// 優雅退出時，確保 buffer 中的資料也寫入資料庫
await bufferService.FlushToDatabase();
```

**狀態**：⏳ 待實作

---

### 🟣 Phase 5: DB Schema 最終設計 ⏳

**Requirement**: 設計並確認資料庫的最終 Schema，包含表格結構、欄位、索引和關聯。

**待確認問題**：
- 是否需要 Device 管理 Table？
- 是否需要歷史資料匯總 Table（如 DailyAggregation）？
- 資料保留策略（保留多久？需要歸檔？）
- 索引策略（Timestamp、DeviceType 等）

**狀態**：⏳ 待設計

---

## 🟣 Implementation Steps

### 已完成
- [x] 建立 `collector/collector.csproj` - .NET 10 Console App，引用 shared project
- [x] 建立 `shared/Models/SensorData.cs` - 定義感測資料 Entity Model（Id, DeviceType, BleAddress, Current, Voltage, Watt, PowerFactor, Frequency, Timestamp）
- [x] 建立 `shared/Data/MqttDbContext.cs:13` - 定義 EF Core DbContext，包含 `DbSet<SensorData>`
- [x] 實作 `collector/Services/DataGenerator.cs:12` - Generate() 產生 EnergyMeter + Modbus 兩筆資料
- [x] 實作 `collector/Services/DataGenerator.cs:49` - PrintReadings() JSON 格式化輸出至 Console
- [x] 實作 `collector/Program.cs:32-33` - 即時資料庫寫入（臨時方案，將被取代）
- [x] 實作 `collector/Program.cs:14-20` - Ctrl+C 優雅退出（CancellationToken 控制迴圈終止）

### 待實作
- [ ] 建立 `collector/Services/DataBufferService.cs` - 資料 buffer 與 60 秒批次寫入服務
- [ ] 修改 `collector/Program.cs` - 整合 DataBufferService，改為批次寫入
- [ ] 設計最終 DB Schema - 確認 Table、欄位、索引、關聯
- [ ] 執行 Migration - 根據最終 Schema 建立或修改 Migration

---

## 📄 相關文件

**Collector:**
- Entry Point: `collector/Program.cs`
- Service: `collector/Services/DataGenerator.cs`
- Project: `collector/collector.csproj`

**Shared:**
- Model: `shared/Models/SensorData.cs`
- DbContext: `shared/Data/MqttDbContext.cs`
- Project: `shared/shared.csproj`
