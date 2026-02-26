# Dummy Data Seeder Impl

| | |
|---|---|
| **Feature** | Dummy Data Seeder |
| **Component** | `backend/Program.cs` |
| **Date** | 2026-02-26 |

---

## Index

1. [Analysis](#analysis)
2. [Requirement](#requirement)
3. [Implementation Steps](#implementation-steps)
4. [Related Files](#related-files)
5. [Notes](#notes)

---

## Analysis

### Context

This project is purely for testing and learning purposes — all data is dummy data. The `collector` app currently generates data slowly (2 records every 3.5 seconds). For development and frontend testing, we need a large dataset (100,000 records) available immediately at backend startup.

### Existing Generation Logic (DataGenerator.cs)

Reference: [collector/Services/DataGenerator.cs](../../../collector/Services/DataGenerator.cs)

The existing generator produces two device types per call:

```
EnergyMeter  →  BleAddress: B0:C4:11:22:33:01
                Current:     0 – 100 A
                Voltage:     218 – 222 V
                Watt:        Current × Voltage
                PowerFactor: 0.85 – 0.99
                Frequency:   59.9 – 60.1 Hz

Modbus       →  BleAddress: B0:C4:11:22:33:02
                Current:     0 – 30 A
                Voltage:     0 – 480 V
                Watt:        Current × Voltage
                PowerFactor: null
                Frequency:   null
```

### Current Program.cs Structure

Reference: [backend/Program.cs](../../../backend/Program.cs)

```
builder setup
  └── AddDbContext<MqttDbContext>
  └── AddControllers
  └── AddOpenApi

var app = builder.Build();          ← inject seeder HERE

app pipeline setup
app.Run();
```

### Existence Check Strategy

Use `CountAsync()` to check existing record count:

| Condition | Action |
|-----------|--------|
| `count >= 100_000` | Skip — log "already exists (N records)" |
| `count < 100_000` | Insert `100_000 - count` additional records |

This makes the seeder **idempotent** and **resumable** (safe to run after partial failures).

---

## Requirement

### Phase 1 — Add seeding block to backend/Program.cs

Add the following block between `var app = builder.Build();` and the pipeline setup, converting `Program.cs` to use top-level async (`await`).

**Required usings to add:**

```csharp
using Shared.Models;
```

**Seeding block:**

```csharp
// Seed dummy data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MqttDbContext>();
    const int targetCount = 100_000;
    var existingCount = await db.SensorData.CountAsync();

    if (existingCount >= targetCount)
    {
        Console.WriteLine($"Dummy data already exists ({existingCount} records). Skipping seed.");
    }
    else
    {
        var random = new Random();
        var toInsert = targetCount - existingCount;
        const int batchSize = 1_000;
        var baseTime = DateTime.Now.AddDays(-30);
        var totalSeconds = (int)TimeSpan.FromDays(30).TotalSeconds;

        Console.WriteLine($"Seeding {toInsert} dummy records...");

        for (int i = 0; i < toInsert; i += batchSize)
        {
            var batch = new List<SensorData>();
            var currentBatchSize = Math.Min(batchSize, toInsert - i);

            for (int j = 0; j < currentBatchSize; j++)
            {
                // Alternate between EnergyMeter and Modbus (50/50 split)
                if ((i + j) % 2 == 0)
                {
                    var c = Math.Round(random.NextDouble() * 100, 2);
                    var v = Math.Round(218 + random.NextDouble() * 4, 2);
                    batch.Add(new SensorData
                    {
                        DeviceType = "EnergyMeter",
                        BleAddress = "B0:C4:11:22:33:01",
                        Current = c,
                        Voltage = v,
                        Watt = Math.Round(c * v, 2),
                        PowerFactor = Math.Round(0.85 + random.NextDouble() * 0.14, 2),
                        Frequency = Math.Round(59.9 + random.NextDouble() * 0.2, 2),
                        Timestamp = baseTime.AddSeconds(random.Next(totalSeconds))
                    });
                }
                else
                {
                    var c = Math.Round(random.NextDouble() * 30, 2);
                    var v = Math.Round(random.NextDouble() * 480, 2);
                    batch.Add(new SensorData
                    {
                        DeviceType = "Modbus",
                        BleAddress = "B0:C4:11:22:33:02",
                        Current = c,
                        Voltage = v,
                        Watt = Math.Round(c * v, 2),
                        PowerFactor = null,
                        Frequency = null,
                        Timestamp = baseTime.AddSeconds(random.Next(totalSeconds))
                    });
                }
            }

            db.SensorData.AddRange(batch);
            await db.SaveChangesAsync();
            Console.WriteLine($"  Seeded {Math.Min(i + batchSize, toInsert)}/{toInsert} records...");
        }

        Console.WriteLine("Seeding complete.");
    }
}
```

**Implementation checklist:**

- [x] Add `using Shared.Models;` to the top of Program.cs
- [x] Insert seeding block after `var app = builder.Build();`
- [x] Verify `await` works (top-level statements in C# support `async` natively)

---

## Implementation Steps

- [x] Add `using Shared.Models;` import to [backend/Program.cs](../../../backend/Program.cs)
- [x] Insert seeding block after `var app = builder.Build();` line 15
- [ ] Run backend: `cd backend && dotnet run`
- [ ] Verify console output shows batch progress logs ending in "Seeding complete."
- [ ] Run backend a second time → verify "Dummy data already exists" message appears
- [ ] Query DB: `SELECT COUNT(*) FROM SensorData` → should return 100000

---

## Related Files

- [backend/Program.cs](../../../backend/Program.cs) — only file modified
- [collector/Services/DataGenerator.cs](../../../collector/Services/DataGenerator.cs) — reference for generation logic
- [shared/Models/SensorData.cs](../../../shared/Models/SensorData.cs) — data model
- [shared/Data/MqttDbContext.cs](../../../shared/Data/MqttDbContext.cs) — DbContext

---

## Notes

- **Batch size 1,000**: Keeps each `SaveChangesAsync` small to avoid memory pressure. 100 batches total.
- **Timestamp spread**: Records are spread randomly across the last 30 days, making the data useful for time-series queries and chart testing.
- **Idempotent**: Running the backend multiple times is safe — the check uses `>=` so partial seeds from interrupted runs are also topped up correctly.
- **No migration needed**: Uses the existing `SensorData` table from `InitialCreate` migration.
- **Top-level async**: .NET top-level statements support `await` natively — no need to wrap in `async Task Main()`.
