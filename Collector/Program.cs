// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

#pragma warning disable SA1516

using Collector.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MqttDb;Trusted_Connection=true;TrustServerCertificate=true;";

var options = new DbContextOptionsBuilder<MqttDbContext>()
    .UseSqlServer(connectionString)
    .Options;

var generator = new DataGenerator();

// Support graceful Ctrl+C exit
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
    Console.WriteLine("\nStopping data collection...");
};

Console.WriteLine("Starting dummy data generation (every 3.5 seconds, press Ctrl+C to stop)\n");

while (!cts.Token.IsCancellationRequested)
{
    var readings = generator.Generate();
    generator.PrintReadings(readings);

    // Write to database
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
