# Device Type Design (EnergyMeter & Modbus) Impl

## Overview

**Feature**: Two-device-type data model with TPH inheritance
**Component**: `shared/Models/SensorData.cs`, `collector/Services/DataGenerator.cs`
**Date**: 2026-02-26

---

## Index

- [Current Design Analysis](#current-design-analysis)
- [Backend Analysis](#backend-analysis-net-10--c)
- [Collector Analysis](#collector-analysis-net-console-app)
- [Design Validation](#design-validation)
- [Requirement Phases](#requirement-phases)
  - [Phase 1: TPH Inheritance — SensorData Base + EnergyMeterData / ModbusData](#phase-1-tph-inheritance--sensordata-base--energymeterdata--modbusdata)
  - [Phase 2: Database Indexing for Query Performance](#phase-2-database-indexing-for-query-performance)
- [Implementation Steps](#implementation-steps-consolidated)
- [Related Files](#related-files)
- [Notes](#notes)

---

## Current Design Analysis

### 1. Do You Have Two Device Types? — Yes

The codebase implements **two distinct device types** through a single-table design with nullable columns:

| Device Type | String Value | BleAddress | Defined At |
|-------------|-------------|------------|------------|
| Energy Meter | `"EnergyMeter"` | `B0:C4:11:22:33:01` | `collector/Services/DataGenerator.cs:21` |
| Modbus | `"Modbus"` | `B0:C4:11:22:33:02` | `collector/Services/DataGenerator.cs:36` |

### 2. How They Differ — Field-Level Comparison

| Field | EnergyMeter | Modbus | Shared? |
|-------|------------|--------|---------|
| `DeviceType` | `"EnergyMeter"` | `"Modbus"` | Yes (discriminator) |
| `BleAddress` | `B0:C4:11:22:33:01` | `B0:C4:11:22:33:02` | Yes (device ID) |
| `Current` | 0–100A | 0–30A | Yes |
| `Voltage` | 218–222V (residential AC) | 0–480V (industrial) | Yes |
| `Watt` | Current × Voltage | Current × Voltage | Yes |
| `PowerFactor` | 0.85–0.99 | **null** | No — EM only |
| `Frequency` | ~60Hz (59.9–60.1) | **null** | No — EM only |
| `Timestamp` | `DateTime.Now` | `DateTime.Now` | Yes |

### 3. Design Pattern: Current vs Proposed

**Current**: Single flat `SensorData` class with nullable `PowerFactor` / `Frequency` columns and a `DeviceType` string literal as discriminator. No compile-time type safety.

**Proposed (Phase 1)**: TPH (Table Per Hierarchy) inheritance — abstract `SensorData` base class with two concrete subclasses. EF Core auto-manages the discriminator column. Same single table in SQL.

```
Current:
  SensorData (flat class, nullable fields, string discriminator)

Proposed:
  SensorData (abstract base)             ← shared fields
  ├── EnergyMeterData : SensorData       ← owns PowerFactor, Frequency
  └── ModbusData : SensorData            ← no extra fields, but type-safe
```

---

## Backend Analysis (.NET 10 / C#)

### 1. DB Schema (EF Core) — Current

#### Entity: `SensorData`

| Property | Type | SQL Type | Nullable | Description |
|----------|------|----------|----------|-------------|
| Id | `int` | `int IDENTITY(1,1)` | No | Primary Key |
| DeviceType | `string` | `nvarchar(max)` | No | `"EnergyMeter"` or `"Modbus"` |
| BleAddress | `string` | `nvarchar(max)` | No | Device MAC address |
| Current | `double` | `float` | No | Amperes |
| Voltage | `double` | `float` | No | Volts |
| Watt | `double` | `float` | No | Power (I × V) |
| PowerFactor | `double?` | `float` | Yes | 0.0–1.0, EM only |
| Frequency | `double?` | `float` | Yes | Hz, EM only |
| Timestamp | `DateTime` | `datetime2` | No | Reading time |

**Model**: `shared/Models/SensorData.cs:3`
**DbContext**: `shared/Data/MqttDbContext.cs:24`
**Migration**: `backend/Migrations/20260214180507_InitialCreate.cs:14`

### 2. DB Schema (EF Core) — After Phase 1

The SQL table **remains identical** (TPH uses the same single table). EF Core automatically:
- Uses the existing `Discriminator` column (renamed from `DeviceType` by convention, or configured to keep `DeviceType`)
- Maps `PowerFactor` and `Frequency` only to `EnergyMeterData` rows
- Returns the correct C# subclass type on query

### 3. Data Workflow (full trace)

```
1. Data generation
   └─ collector/Services/DataGenerator.cs:12 — Generate() creates 2 readings (1 EM + 1 Modbus)
         ↓
2. Console output
   └─ collector/Services/DataGenerator.cs:49 — PrintReadings() serializes to JSON
         ↓
3. Database write
   └─ collector/Program.cs:32 — db.SensorData.AddRange(readings)
   └─ collector/Program.cs:33 — db.SaveChangesAsync()
         ↓
4. Database storage
   └─ shared/Data/MqttDbContext.cs:24 — DbSet<SensorData>
         ↓
5. API query (NOT YET IMPLEMENTED)
   └─ backend/Controllers/ — No controllers exist yet
         ↓
6. Frontend consumption (NOT YET IMPLEMENTED)
   └─ frontend/src/ — Only App.vue and main.ts exist
```

---

## Collector Analysis (.NET Console App)

### 1. Data Collection Flow

| File | Line | Purpose |
|------|------|---------|
| `collector/Program.cs` | 11 | Create `DataGenerator` instance |
| `collector/Program.cs` | 24 | Main loop (every 3.5s) |
| `collector/Program.cs` | 26 | Call `generator.Generate()` |
| `collector/Program.cs` | 32 | Write readings to database |
| `collector/Services/DataGenerator.cs` | 12 | `Generate()` — produces 1 EM + 1 Modbus reading |
| `collector/Services/DataGenerator.cs` | 19 | EnergyMeter reading construction |
| `collector/Services/DataGenerator.cs` | 34 | Modbus reading construction |

### 2. EnergyMeter Data Profile

```csharp
// collector/Services/DataGenerator.cs:19-29
DeviceType = "EnergyMeter"
BleAddress = "B0:C4:11:22:33:01"
Voltage = 218 + random * 4          // ~220V residential AC
Current = random * 100              // 0–100A
Watt = Current × Voltage            // calculated
PowerFactor = 0.85 + random * 0.14  // realistic range
Frequency = 59.9 + random * 0.2     // ~60Hz
```

### 3. Modbus Data Profile

```csharp
// collector/Services/DataGenerator.cs:34-44
DeviceType = "Modbus"
BleAddress = "B0:C4:11:22:33:02"
Voltage = random * 480              // 0–480V industrial
Current = random * 30               // 0–30A
Watt = Current × Voltage            // calculated
PowerFactor = null                  // not applicable
Frequency = null                    // not applicable
```

---

## Design Validation

### What's Correct

1. **Two device types exist and are clearly differentiated** — EnergyMeter has richer data (PowerFactor, Frequency); Modbus is simpler
2. **Nullable columns are appropriate** — `double?` for device-specific fields is the right pattern
3. **Single table is efficient for queries** — No JOINs needed; filtering by DeviceType is straightforward
4. **Data ranges are realistic** — EM uses residential AC (~220V), Modbus uses industrial range (0–480V)
5. **Shared model in `shared/`** — Both collector and backend reference the same entity

### Design Concerns to Address

| # | Concern | Current State | Addressed In |
|---|---------|---------------|--------------|
| 1 | No type safety — `DeviceType` is a free-form `string`, device-specific fields not enforced | Typos possible, Modbus can accidentally have PowerFactor set | **Phase 1** (TPH inheritance) |
| 2 | No index on `DeviceType` or `Timestamp` | Full table scan for filtered/time-range queries | **Phase 2** (indexing) |
| 3 | `BleAddress` is hardcoded per device | Only 1 EM + 1 Modbus device | Fine for MVP |
| 4 | `nvarchar(max)` for `DeviceType` and `BleAddress` | Wastes storage, poor index support | **Phase 2** (MaxLength) |

---

## Requirement Phases

### Phase 1: TPH Inheritance — SensorData Base + EnergyMeterData / ModbusData

**Requirement**: Replace the flat `SensorData` class with a TPH class hierarchy so that each device type is a distinct C# class with compile-time type safety.

**Modification Context**:
- **Why**: Currently `DeviceType` is a free-form string and `PowerFactor`/`Frequency` are nullable on all readings — nothing prevents a Modbus reading from having `PowerFactor = 0.95` or an EnergyMeter reading from missing it. Two classes make the data contract explicit.
- **How**: Convert `SensorData` to an abstract base class with shared fields. Create `EnergyMeterData` subclass owning `PowerFactor` + `Frequency`. Create `ModbusData` subclass with no extra fields. Configure EF Core TPH discriminator in `MqttDbContext.OnModelCreating`.
- **Impact scope**: `shared/Models/SensorData.cs`, new files `shared/Models/EnergyMeterData.cs` + `shared/Models/ModbusData.cs`, `shared/Data/MqttDbContext.cs`, `collector/Services/DataGenerator.cs`, new EF Core migration

**Code Changes**:

#### 1. Refactor `shared/Models/SensorData.cs` to abstract base

```csharp
// MODIFY: shared/Models/SensorData.cs
// Convert from concrete class to abstract base class
// Remove PowerFactor and Frequency (moved to EnergyMeterData)

namespace Shared.Models;

public abstract class SensorData
{
    public int Id { get; set; }
    public string BleAddress { get; set; } = string.Empty;
    public double Current { get; set; }
    public double Voltage { get; set; }
    public double Watt { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**Key changes from current `shared/Models/SensorData.cs:3`:**
- `class` → `abstract class`
- Removed `DeviceType` property (EF Core manages discriminator automatically)
- Removed `PowerFactor` (moved to `EnergyMeterData`)
- Removed `Frequency` (moved to `EnergyMeterData`)

#### 2. Create `shared/Models/EnergyMeterData.cs`

```csharp
// NEW FILE: shared/Models/EnergyMeterData.cs
namespace Shared.Models;

public class EnergyMeterData : SensorData
{
    public double PowerFactor { get; set; }
    public double Frequency { get; set; }
}
```

**Note**: `PowerFactor` and `Frequency` are now **non-nullable `double`** on `EnergyMeterData` — every EnergyMeter reading **must** have these values. They remain nullable columns in SQL (TPH requirement), but the C# type enforces they are always populated for this device type.

#### 3. Create `shared/Models/ModbusData.cs`

```csharp
// NEW FILE: shared/Models/ModbusData.cs
namespace Shared.Models;

public class ModbusData : SensorData
{
    // No additional fields — Modbus only provides Current, Voltage, Watt
}
```

#### 4. Configure TPH in `shared/Data/MqttDbContext.cs`

```csharp
// MODIFY: shared/Data/MqttDbContext.cs
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data;

public class MqttDbContext : DbContext
{
    public MqttDbContext(DbContextOptions<MqttDbContext> options)
        : base(options)
    {
    }

    public DbSet<SensorData> SensorData { get; set; }
    public DbSet<EnergyMeterData> EnergyMeterData { get; set; }
    public DbSet<ModbusData> ModbusData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SensorData>(entity =>
        {
            // TPH discriminator — reuse existing DeviceType column
            entity.HasDiscriminator<string>("DeviceType")
                  .HasValue<EnergyMeterData>("EnergyMeter")
                  .HasValue<ModbusData>("Modbus");

            entity.Property("DeviceType").HasMaxLength(50);
            entity.Property(e => e.BleAddress).HasMaxLength(50);
        });
    }
}
```

**Key points**:
- `DbSet<SensorData>` queries **all** device types (polymorphic)
- `DbSet<EnergyMeterData>` queries only EnergyMeter rows
- `DbSet<ModbusData>` queries only Modbus rows
- Discriminator column name stays `"DeviceType"` to match existing data
- Discriminator values stay `"EnergyMeter"` / `"Modbus"` — **existing data remains compatible**

#### 5. Update `collector/Services/DataGenerator.cs`

```csharp
// MODIFY: collector/Services/DataGenerator.cs
// Change Generate() return type and construct subclass instances

public List<SensorData> Generate()
{
    var readings = new List<SensorData>();

    // Energy Meter: stable voltage ~220V, high current, with PowerFactor and Frequency
    var emCurrent = Math.Round(_random.NextDouble() * 100, 2);
    var emVoltage = Math.Round(218 + _random.NextDouble() * 4, 2);
    readings.Add(new EnergyMeterData                     // was: new SensorData
    {
        // DeviceType removed — EF Core sets it automatically
        BleAddress = "B0:C4:11:22:33:01",
        Current = emCurrent,
        Voltage = emVoltage,
        Watt = Math.Round(emCurrent * emVoltage, 2),
        PowerFactor = Math.Round(0.85 + _random.NextDouble() * 0.14, 2),
        Frequency = Math.Round(59.9 + _random.NextDouble() * 0.2, 2),
        Timestamp = DateTime.Now
    });

    // Modbus: wider industrial voltage range, low current
    var mbCurrent = Math.Round(_random.NextDouble() * 30, 2);
    var mbVoltage = Math.Round(_random.NextDouble() * 480, 2);
    readings.Add(new ModbusData                          // was: new SensorData
    {
        // DeviceType removed — EF Core sets it automatically
        // PowerFactor/Frequency removed — not available on ModbusData
        BleAddress = "B0:C4:11:22:33:02",
        Current = mbCurrent,
        Voltage = mbVoltage,
        Watt = Math.Round(mbCurrent * mbVoltage, 2),
        Timestamp = DateTime.Now
    });

    return readings;
}
```

**What changes in DataGenerator**:
- `new SensorData { DeviceType = "EnergyMeter", ... }` → `new EnergyMeterData { ... }`
- `new SensorData { DeviceType = "Modbus", PowerFactor = null, ... }` → `new ModbusData { ... }`
- `DeviceType` assignment removed (EF Core handles it)
- `PowerFactor = null` / `Frequency = null` removed (not on `ModbusData`)
- Return type `List<SensorData>` stays the same (polymorphic)

#### 6. Migration

```bash
# In backend/ directory:
dotnet ef migrations add TphInheritance
dotnet ef database update
```

The migration will:
- Keep the same `SensorData` table (TPH = single table)
- Rename/reconfigure the discriminator if needed (likely no schema change since column `DeviceType` already exists with matching values)

**Implementation Steps**:
- [ ] Modify `shared/Models/SensorData.cs:3` — convert to `abstract class`, remove `DeviceType`, `PowerFactor`, `Frequency`
- [ ] Create `shared/Models/EnergyMeterData.cs` — subclass with `PowerFactor` + `Frequency`
- [ ] Create `shared/Models/ModbusData.cs` — subclass with no extra fields
- [ ] Modify `shared/Data/MqttDbContext.cs:14` — add `DbSet<EnergyMeterData>`, `DbSet<ModbusData>`, add `OnModelCreating` with TPH discriminator config
- [ ] Modify `collector/Services/DataGenerator.cs:19` — use `new EnergyMeterData` instead of `new SensorData`
- [ ] Modify `collector/Services/DataGenerator.cs:34` — use `new ModbusData` instead of `new SensorData`
- [ ] Run `dotnet ef migrations add TphInheritance` in backend/
- [ ] Run `dotnet ef database update` in backend/

---

### Phase 2: Database Indexing for Query Performance

**Requirement**: Add indexes for the query patterns defined in PRD (F2: filter by DeviceType + time range).

**Modification Context**:
- **Why**: `DeviceType` and `Timestamp` are the primary filter columns (PRD F2, RFC-002), but no indexes exist
- **How**: Add composite indexes in the `OnModelCreating` added in Phase 1
- **Impact scope**: `shared/Data/MqttDbContext.cs`, new migration

**Code Changes**:

```csharp
// MODIFY: shared/Data/MqttDbContext.cs — extend OnModelCreating from Phase 1
// Add inside the existing entity configuration block:

entity.HasIndex(e => new { e.Timestamp })
      .HasDatabaseName("IX_SensorData_Timestamp");

entity.HasIndex("DeviceType", "Timestamp")
      .HasDatabaseName("IX_SensorData_DeviceType_Timestamp");

entity.HasIndex(e => new { e.BleAddress, e.Timestamp })
      .HasDatabaseName("IX_SensorData_BleAddress_Timestamp");
```

**Note**: Since `DeviceType` is now a shadow property (managed by EF Core discriminator, not a C# property), index it via string name `"DeviceType"` rather than lambda.

**Implementation Steps**:
- [ ] Modify `shared/Data/MqttDbContext.cs` — add composite indexes in `OnModelCreating`
- [ ] Run `dotnet ef migrations add AddSensorDataIndexes` in backend/
- [ ] Run `dotnet ef database update` in backend/

---

## Implementation Steps (Consolidated)

**Phase 1 — TPH Inheritance:**
- [ ] Modify `shared/Models/SensorData.cs:3` — convert to `abstract class`, remove `DeviceType`, `PowerFactor`, `Frequency`
- [ ] Create `shared/Models/EnergyMeterData.cs` — subclass with `PowerFactor` + `Frequency`
- [ ] Create `shared/Models/ModbusData.cs` — subclass with no extra fields
- [ ] Modify `shared/Data/MqttDbContext.cs:14` — add `DbSet<EnergyMeterData>`, `DbSet<ModbusData>`, add `OnModelCreating` with TPH discriminator
- [ ] Modify `collector/Services/DataGenerator.cs:19` — `new SensorData` → `new EnergyMeterData`
- [ ] Modify `collector/Services/DataGenerator.cs:34` — `new SensorData` → `new ModbusData`
- [ ] Run `dotnet ef migrations add TphInheritance` in backend/
- [ ] Run `dotnet ef database update` in backend/

**Phase 2 — Indexing:**
- [ ] Modify `shared/Data/MqttDbContext.cs` — add composite indexes in `OnModelCreating`
- [ ] Run `dotnet ef migrations add AddSensorDataIndexes` in backend/
- [ ] Run `dotnet ef database update` in backend/

---

## Related Files

**Shared:**
- Model (base): `shared/Models/SensorData.cs`
- Model (EM): `shared/Models/EnergyMeterData.cs` *(new in Phase 1)*
- Model (Modbus): `shared/Models/ModbusData.cs` *(new in Phase 1)*
- DbContext: `shared/Data/MqttDbContext.cs`

**Collector:**
- Entry: `collector/Program.cs`
- Generator: `collector/Services/DataGenerator.cs`

**Backend:**
- Migration: `backend/Migrations/20260214180507_InitialCreate.cs`
- Controllers: *(not yet implemented)*

**Docs:**
- RFC: `docs/rfc/RFC-001-data-collection-strategy.md`
- PRD: `docs/PRD.md`

---

## Notes

- **TPH keeps the same SQL table** — no data migration needed, existing rows with `DeviceType = "EnergyMeter"` / `"Modbus"` are automatically recognized by EF Core
- `PowerFactor` and `Frequency` columns remain nullable in SQL (TPH requirement) but are **non-nullable in C#** on `EnergyMeterData` — EF Core handles the mapping
- `DeviceType` becomes a **shadow property** — it no longer exists as a C# property on `SensorData`, but EF Core still reads/writes it in the database discriminator column
- Querying `db.SensorData.ToListAsync()` returns a mix of `EnergyMeterData` and `ModbusData` instances — use pattern matching (`is EnergyMeterData em`) or query specific `DbSet` (`db.EnergyMeterData`)
- If a third device type is added, just create `NewDeviceData : SensorData` and add `.HasValue<NewDeviceData>("NewDevice")` — no schema change needed
- The `BleAddress` field name assumes BLE connectivity — consider renaming to `DeviceAddress` if Modbus devices use RS-485 or TCP
