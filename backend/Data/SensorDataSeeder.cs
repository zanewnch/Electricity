using Shared.Data;
using Shared.Models;

namespace Backend.Data;

public static class SensorDataSeeder
{
    private const int TargetCount = 100_000;
    private const int BatchSize = 1_000;

    public static async Task SeedAsync(MqttDbContext db)
    {
        var existingCount = await db.SensorData.CountAsync();

        if (existingCount >= TargetCount)
        {
            Console.WriteLine($"Dummy data already exists ({existingCount} records). Skipping seed.");
            return;
        }

        var random = new Random();
        var toInsert = TargetCount - existingCount;
        var baseTime = DateTime.Now.AddDays(-30);
        var totalSeconds = (int)TimeSpan.FromDays(30).TotalSeconds;

        Console.WriteLine($"Seeding {toInsert} dummy records...");

        for (int i = 0; i < toInsert; i += BatchSize)
        {
            var batch = new List<SensorData>();
            var currentBatchSize = Math.Min(BatchSize, toInsert - i);

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
            Console.WriteLine($"  Seeded {Math.Min(i + BatchSize, toInsert)}/{toInsert} records...");
        }

        Console.WriteLine("Seeding complete.");
    }
}
