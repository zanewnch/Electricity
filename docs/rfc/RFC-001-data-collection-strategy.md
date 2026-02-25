# RFC-001: Data Collection Strategy

## Status

- [x] Draft
- [ ] Under Review
- [ ] Accepted
- [ ] Rejected

## Problem

The electricity analysis system needs a continuous data source to:
1. Populate the database with sensor readings
2. Test dashboard and aggregation features during development
3. Validate that time-aggregated trends reveal meaningful patterns
4. Provide a foundation for future real device integration

Raw sensor data collection is currently non-existent; we need both an immediate solution (dummy data) and a migration path to real devices (BLE, MQTT, etc.).

## Goals

- Implement a reliable data collector that writes readings every 3.5 seconds
- Support multiple device types (EnergyMeter, Modbus) with device-specific fields
- Enable seamless transition from dummy data to real device sources
- Provide stable, continuous operation with graceful shutdown
- Share a common data model with the backend via the `shared/` project

## Non-Goals

- Real device integration (BLE/MQTT/Modbus) — planned for future phases
- Data validation or sanitization at collection time (handled by backend)
- Distributed collectors or clustering — single-instance sufficient for MVP
- High-frequency data collection (>1 reading/sec) — 3.5s granularity is acceptable for trend analysis

## Proposed Solution

### Architecture: .NET Console App with Pluggable Data Source

```
┌─────────────────────────────────────┐
│       Collector (Console App)       │
│                                     │
│  ┌──────────────────────────────┐   │
│  │    IDataSource (interface)   │   │
│  │                              │   │
│  │  ┌──────────────────────┐    │   │
│  │  │ DummyDataGenerator   │    │   │
│  │  │ (current impl)       │    │   │
│  │  └──────────────────────┘    │   │
│  │                              │   │
│  │  ┌──────────────────────┐    │   │
│  │  │ MqttDataSource       │    │   │
│  │  │ (future impl)        │    │   │
│  │  └──────────────────────┘    │   │
│  └──────────────────────────────┘   │
│              ↓                       │
│       DbContext (EF Core)           │
│              ↓                       │
│         SQL Server DB               │
└─────────────────────────────────────┘
```

### Data Model

**SensorData Table** (shared via `shared/` project)

```csharp
public class SensorData
{
    public int Id { get; set; }
    public string DeviceType { get; set; }        // "EnergyMeter" / "Modbus" / other
    public string BleAddress { get; set; }        // Device identifier (MAC address format)
    public double Current { get; set; }           // Amperes
    public double Voltage { get; set; }           // Volts
    public double Watt { get; set; }              // Power (calculated as I × V)
    public double? PowerFactor { get; set; }      // 0.0-1.0 (EnergyMeter only)
    public double? Frequency { get; set; }        // Hz (EnergyMeter only)
    public DateTime Timestamp { get; set; }       // UTC capture time
}
```

**Device Types Supported:**

| Device Type | Fields | Source | Purpose |
|-------------|--------|--------|---------|
| EnergyMeter | Current, Voltage, Watt, PowerFactor, Frequency | AC residential meter | Typical home circuit analysis |
| Modbus | Current, Voltage, Watt | Industrial device | Industrial/large-scale testing |

### Current Implementation: DummyDataGenerator

The collector runs continuously in a loop, generating synthetic readings every 3.5 seconds:

```csharp
// Program.cs execution flow:
1. Configure DbContext with SQL Server
2. Create DataGenerator instance
3. Loop until Ctrl+C:
   a) Generate list of SensorData records
   b) Write to database via DbContext
   c) Print to console
   d) Wait 3.5 seconds
```

**Data Generation Logic:**
- **EnergyMeter** (stable): ~220V ±2V, 0-100A, realistic power factor (0.85-0.99), frequency ~60Hz
- **Modbus** (wide range): 0-480V, 0-30A, industrial profile, no PowerFactor/Frequency

### Collection Interval: 3.5 Seconds

Why 3.5 seconds?
- Fast enough to generate meaningful volume (24,686 records/day per device)
- Slow enough to avoid database contention
- Sufficient granularity for minute-level aggregation
- Leaves headroom for real device integration (BLE/MQTT typically 1-5 sec cycles)

### Graceful Shutdown

```csharp
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nStopping data collection...");
};
```

Ensures in-flight database writes complete before exit, preventing corruption.

## Alternatives Considered

| Option | Pros | Cons | Decision |
|--------|------|------|----------|
| **Dummy Console App** (Chosen) | No external dependencies; instant setup; pluggable source | Requires manual testing; dummy data may not match real devices | Selected for MVP speed and flexibility |
| Windows Service | Can auto-restart; runs in background | Complex setup; harder to debug; overkill for development | Defer to production phase |
| Docker container | Portable; production-ready | Adds complexity; requires Docker setup on dev machine | Defer to production phase |
| Direct SQL inserts | Very fast; minimal dependencies | Not type-safe; hard to migrate to real sources | Rejected; EF Core is superior for maintainability |
| Event-driven (Kafka/MQTT broker) | Decouples collector from database | External dependency; complex for development | Defer to future phase |

## Migration Path: Dummy → Real Devices

When transitioning to real device data (BLE, MQTT, Modbus):

1. **Create `IDataSource` interface:**
   ```csharp
   public interface IDataSource
   {
       Task<List<SensorData>> ReadAsync();
   }
   ```

2. **Implement concrete sources:**
   - `DummyDataSource` (current)
   - `MqttDataSource` (future)
   - `BleDataSource` (future)

3. **Modify Program.cs:**
   ```csharp
   IDataSource source = config["Source"] switch
   {
       "mqtt" => new MqttDataSource(config),
       "ble" => new BleDataSource(config),
       _ => new DummyDataSource()
   };
   ```

4. **Data model remains unchanged** — no migration needed, as SensorData is device-agnostic.

## Open Questions

- [ ] At what point should real device integration begin? (After MVP validation?)
- [ ] Should the collector validate readings or pass all values as-is? (Current: pass as-is)
- [ ] Is 3.5 seconds the optimal collection interval, or should it be configurable?
- [ ] Should the collector implement retry logic if database is temporarily unavailable?

## References

- [ADR-001 — Tech Stack](../adr/ADR-001-tech-stack.md)
- [ADR-002 — ORM Selection](../adr/ADR-002-orm-selection.md)
- [PRD — Functional Requirement F1](../PRD.md#f1--dashboard-real-time-overview)
- [SensorData Model](../../shared/Models/SensorData.cs)
