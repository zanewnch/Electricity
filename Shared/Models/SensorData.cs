// <copyright file="SensorData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Shared.Models;

/// <summary>
/// Represents a sensor data reading from an electricity monitoring device.
/// </summary>
public class SensorData
{
    /// <summary>Gets or sets the primary key.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the device type (e.g. EnergyMeter, Modbus).</summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>Gets or sets the BLE MAC address of the device.</summary>
    public string BleAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the current reading in amperes.</summary>
    public double Current { get; set; }

    /// <summary>Gets or sets the voltage reading in volts.</summary>
    public double Voltage { get; set; }

    /// <summary>Gets or sets the power reading in watts.</summary>
    public double Watt { get; set; }

    /// <summary>Gets or sets the power factor (null if not applicable).</summary>
    public double? PowerFactor { get; set; }

    /// <summary>Gets or sets the frequency in hertz (null if not applicable).</summary>
    public double? Frequency { get; set; }

    /// <summary>Gets or sets the timestamp of the reading.</summary>
    public DateTime Timestamp { get; set; }
}
