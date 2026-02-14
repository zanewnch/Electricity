using Microsoft.EntityFrameworkCore;
using Collector.Services;
using Shared.Data;

var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MqttDb;Trusted_Connection=true;TrustServerCertificate=true;";

var options = new DbContextOptionsBuilder<MqttDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var generator = new DataGenerator();

// 支援 Ctrl+C 優雅退出
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

    // 寫入資料庫
    using (var db = new MqttDbContext(options))
    {
        db.SensorData.AddRange(readings);
        await db.SaveChangesAsync(cts.Token);
    }

    try
    {
        await Task.Delay(3500, cts.Token);
    }
    catch (TaskCanceledException)
    {
        break;
    }
}
