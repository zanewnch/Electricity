namespace Shared.Models;

public class SensorData
{
    public int Id { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string BleAddress { get; set; } = string.Empty;
    public double Current { get; set; }
    public double Voltage { get; set; }
    public double Watt { get; set; }
    public double? PowerFactor { get; set; }
    public double? Frequency { get; set; }
    public DateTime Timestamp { get; set; }
}
