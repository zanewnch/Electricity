# Collector Refactoring Impl

## Overview

**Feature**: Collector refactoring
**Component**: `collector/Program.cs`, `collector/Services/DataGenerator.cs`
**Date**: 2026-02-26

---

## Index

- [Collector Analysis — Current Logic](#collector-analysis--current-logic)
  - [Architecture Overview](#1-architecture-overview)
  - [File Structure](#2-file-structure)
  - [Program.cs — Entry Point & Main Loop](#3-programcs--entry-point--main-loop)
  - [DataGenerator.cs — Data Generation Service](#4-datageneratorcs--data-generation-service)
  - [Data Workflow (full trace)](#5-data-workflow-full-trace)
  - [What Each Cycle Does](#6-what-each-cycle-does)
  - [Current Limitations](#7-current-limitations)
- [Requirement Phases](#requirement-phases)
- [Implementation Steps](#implementation-steps)
- [Related Files](#related-files)

---

## Collector Analysis — Current Logic

### 1. Architecture Overview

The collector is a **.NET 10 Console App** that runs an infinite loop, generating fake sensor data and writing it to SQL Server every 3.5 seconds. It has **no dependency injection**, **no configuration file**, and **no interface abstraction** — it's a minimal script-style app.

```
┌──────────────────────────────────────────────────┐
│              collector (Console App)              │
│                                                  │
│  Program.cs                                      │
│  ┌────────────────────────────────────────────┐  │
│  │  1. Hardcoded connection string             │  │
│  │  2. Build DbContextOptions manually         │  │
│  │  3. new DataGenerator()                     │  │
│  │  4. while loop (every 3.5s):                │  │
│  │     a) generator.Generate()  → 2 readings   │  │
│  │     b) generator.PrintReadings() → console  │  │
│  │     c) new MqttDbContext() → AddRange+Save  │  │
│  │     d) Task.Delay(3500)                     │  │
│  └────────────────────────────────────────────┘  │
│                      ↓                            │
│  DataGenerator.cs                                │
│  ┌────────────────────────────────────────────┐  │
│  │  Generate() → List<SensorData>              │  │
│  │    - 1x EnergyMeter (residential AC)        │  │
│  │    - 1x Modbus (industrial)                 │  │
│  │                                             │  │
│  │  PrintReadings() → JSON to console          │  │
│  └────────────────────────────────────────────┘  │
│                      ↓                            │
│           shared/Data/MqttDbContext               │
│                      ↓                            │
│              SQL Server (MqttDb)                  │
└──────────────────────────────────────────────────┘
```

### 2. File Structure

| File | Lines | Purpose |
|------|-------|---------|
| `collector/collector.csproj` | 21 | Project config: .NET 10, references `shared.csproj` |
| `collector/Program.cs` | 44 | Entry point, DB setup, main loop |
| `collector/Services/DataGenerator.cs` | 59 | Data generation + console printing |

No other files — no `appsettings.json`, no DI container, no interfaces.

### 3. Program.cs — Entry Point & Main Loop

**File**: `collector/Program.cs` (44 lines, top-level statements)

#### Line-by-line walkthrough:

| Lines | What it does |
|-------|-------------|
| `5` | **Hardcoded connection string** — `Server=(localdb)\MSSQLLocalDB;Database=MqttDb;...` directly in code |
| `7-9` | **Build DbContextOptions** — `new DbContextOptionsBuilder<MqttDbContext>().UseSqlServer(connectionString).Options` |
| `11` | **Create generator** — `new DataGenerator()` (no DI, direct instantiation) |
| `14-20` | **Ctrl+C handler** — `CancellationTokenSource` + `Console.CancelKeyPress` for graceful shutdown |
| `22` | **Startup message** — prints "Starting dummy data generation (every 3.5 seconds, press Ctrl+C to stop)" |
| `24` | **Main loop** — `while (!cts.Token.IsCancellationRequested)` |
| `26` | **Generate** — `var readings = generator.Generate()` → returns `List<SensorData>` with 2 items |
| `27` | **Print** — `generator.PrintReadings(readings)` → JSON to console |
| `30-34` | **Write to DB** — `new MqttDbContext(options)` inside `using`, then `AddRange` + `SaveChangesAsync` |
| `38` | **Wait** — `Task.Delay(3500, cts.Token)` → 3.5 seconds between cycles |
| `40-43` | **Graceful exit** — catch `TaskCanceledException` → `break` |

#### Key observations:

- **New DbContext every cycle** (line 30) — creates and disposes a `MqttDbContext` per iteration. This is actually fine for a console app (no DI scope), but different from typical ASP.NET patterns.
- **No error handling for DB failures** — if `SaveChangesAsync` throws (e.g., DB offline), the whole app crashes.
- **Connection string hardcoded** — not in `appsettings.json` or environment variables.
- **Synchronous generation, async save** — `Generate()` is sync, `SaveChangesAsync()` is async.

### 4. DataGenerator.cs — Data Generation Service

**File**: `collector/Services/DataGenerator.cs` (59 lines)

#### Class structure:

```csharp
public class DataGenerator
{
    // Static JSON options for pretty-printing
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    // Random instance (not thread-safe, but single-threaded app so OK)
    private readonly Random _random = new();

    // Two public methods:
    public List<SensorData> Generate()        // → produce readings
    public void PrintReadings(...)            // → JSON to console
}
```

#### `Generate()` method (lines 12–47):

Produces exactly **2 readings per call** — one EnergyMeter, one Modbus:

**EnergyMeter reading** (lines 19–29):
```
DeviceType  = "EnergyMeter"           ← hardcoded string
BleAddress  = "B0:C4:11:22:33:01"     ← hardcoded single device
Current     = random * 100            ← 0–100A (residential high)
Voltage     = 218 + random * 4        ← 218–222V (stable residential AC)
Watt        = Current × Voltage       ← calculated
PowerFactor = 0.85 + random * 0.14    ← 0.85–0.99 (realistic)
Frequency   = 59.9 + random * 0.2     ← 59.9–60.1Hz
Timestamp   = DateTime.Now            ← local time, not UTC
```

**Modbus reading** (lines 34–44):
```
DeviceType  = "Modbus"                ← hardcoded string
BleAddress  = "B0:C4:11:22:33:02"    ← hardcoded single device
Current     = random * 30             ← 0–30A (industrial low)
Voltage     = random * 480            ← 0–480V (wide industrial range)
Watt        = Current × Voltage       ← calculated
PowerFactor = null                    ← not applicable
Frequency   = null                    ← not applicable
Timestamp   = DateTime.Now            ← local time, not UTC
```

#### `PrintReadings()` method (lines 49–58):

Serializes each `SensorData` to indented JSON and prints to console, followed by `"---"` separator.

### 5. Data Workflow (full trace)

```
1. App starts
   └─ collector/Program.cs:5       — Connection string defined (hardcoded)
   └─ collector/Program.cs:7-9     — DbContextOptions built
   └─ collector/Program.cs:11      — DataGenerator created
         ↓
2. Graceful shutdown registered
   └─ collector/Program.cs:14-20   — CancellationTokenSource + Ctrl+C handler
         ↓
3. Main loop begins
   └─ collector/Program.cs:24      — while (!cts.Token.IsCancellationRequested)
         ↓
4. Generate data (per cycle)
   └─ collector/Program.cs:26                    — generator.Generate()
   └─ collector/Services/DataGenerator.cs:12     — Generate() entry
   └─ collector/Services/DataGenerator.cs:19-29  — EnergyMeter SensorData created
   └─ collector/Services/DataGenerator.cs:34-44  — Modbus SensorData created
         ↓
5. Print to console (per cycle)
   └─ collector/Program.cs:27                    — generator.PrintReadings(readings)
   └─ collector/Services/DataGenerator.cs:49-58  — JSON serialize + Console.WriteLine
         ↓
6. Write to database (per cycle)
   └─ collector/Program.cs:30      — new MqttDbContext(options)
   └─ collector/Program.cs:32      — db.SensorData.AddRange(readings)
   └─ collector/Program.cs:33      — await db.SaveChangesAsync(cts.Token)
         ↓
7. Wait 3.5 seconds
   └─ collector/Program.cs:38      — await Task.Delay(3500, cts.Token)
         ↓
   (back to step 3)
```

### 6. What Each Cycle Does

Every 3.5 seconds, one cycle produces:

| Item | Count | Size |
|------|-------|------|
| `SensorData` objects created | 2 (1 EM + 1 Modbus) | ~200 bytes each |
| Database INSERTs | 2 rows per cycle | via single `SaveChangesAsync` |
| Console output | 2 JSON blocks + separator | ~500 chars |
| Daily volume | ~49,371 rows/day | (2 rows × ~24,686 cycles/day) |

### 7. Current Limitations

| # | Limitation | Detail |
|---|-----------|--------|
| 1 | **Hardcoded connection string** | `collector/Program.cs:5` — not configurable |
| 2 | **No DI container** | `DataGenerator` is instantiated with `new`, no `IServiceProvider` |
| 3 | **No error handling** | DB failure = app crash, no retry logic |
| 4 | **Fixed 2 devices** | Always 1 EM + 1 Modbus, hardcoded BleAddress |
| 5 | **No interface abstraction** | `DataGenerator` is a concrete class, not swappable |
| 6 | **`DateTime.Now` not UTC** | `DataGenerator.cs:28,43` — uses local time |
| 7 | **Fixed interval** | 3.5s hardcoded at `Program.cs:38`, not configurable |
| 8 | **Console output mixed with logic** | `PrintReadings()` lives in `DataGenerator`, coupling generation with presentation |

---

## Requirement Phases

*Awaiting your requirements. Tell me what you want to change about the collector and I'll fill in the phases.*

---

## Implementation Steps

*Will be generated after Requirement Phases are defined.*

---

## Related Files

**Collector:**
- Project: `collector/collector.csproj`
- Entry: `collector/Program.cs`
- Generator: `collector/Services/DataGenerator.cs`

**Shared:**
- Model: `shared/Models/SensorData.cs`
- DbContext: `shared/Data/MqttDbContext.cs`

**Docs:**
- RFC: `docs/rfc/RFC-001-data-collection-strategy.md`
- Related impl: `docs/impl/device-type-design/impl.md`

---

## Notes

- This impl focuses on the **collector project only** — no frontend or backend analysis needed
- The collector currently has only 2 files of actual code (`Program.cs` + `DataGenerator.cs`)
- RFC-001 already planned an `IDataSource` interface for future real device integration
- Any collector refactoring should maintain backward compatibility with the existing `SensorData` table schema
