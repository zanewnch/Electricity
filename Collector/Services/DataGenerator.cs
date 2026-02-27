// <copyright file="DataGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text.Json;
using Shared.Models;

namespace Collector.Services;

public class DataGenerator
{
    private static readonly JsonSerializerOptions JsonOptions = new () { WriteIndented = true };

    private readonly Random random = new ();

    public List<SensorData> Generate()
    {
        var readings = new List<SensorData>();

        // Energy Meter: stable voltage ~220V, high current, with PowerFactor and Frequency
        var emCurrent = Math.Round(this.random.NextDouble() * 100, 2);
        var emVoltage = Math.Round(218 + (this.random.NextDouble() * 4), 2);
        readings.Add(new SensorData
        {
            DeviceType = "EnergyMeter",
            BleAddress = "B0:C4:11:22:33:01",
            Current = emCurrent,
            Voltage = emVoltage,
            Watt = Math.Round(emCurrent * emVoltage, 2),
            PowerFactor = Math.Round(0.85 + (this.random.NextDouble() * 0.14), 2),
            Frequency = Math.Round(59.9 + (this.random.NextDouble() * 0.2), 2),
            Timestamp = DateTime.Now,
        });

        // Modbus: wider industrial voltage range, low current, no PowerFactor or Frequency
        var mbCurrent = Math.Round(this.random.NextDouble() * 30, 2);
        var mbVoltage = Math.Round(this.random.NextDouble() * 480, 2);
        readings.Add(new SensorData
        {
            DeviceType = "Modbus",
            BleAddress = "B0:C4:11:22:33:02",
            Current = mbCurrent,
            Voltage = mbVoltage,
            Watt = Math.Round(mbCurrent * mbVoltage, 2),
            PowerFactor = null,
            Frequency = null,
            Timestamp = DateTime.Now,
        });

        return readings;
    }

    public void PrintReadings(List<SensorData> readings)
    {
        foreach (var reading in readings)
        {
            var json = JsonSerializer.Serialize(reading, JsonOptions);
            Console.WriteLine(json);
        }

        Console.WriteLine("---");
    }
}
