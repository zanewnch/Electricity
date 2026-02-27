// <copyright file="MqttDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Shared.Data;

/// <summary>
/// Entity Framework Core DbContext for MQTT sensor data.
///
/// 用途：
/// - 管理 SensorData table 與資料庫的連接
/// - 被 collector（資料寫入）與 backend（API 查詢）共享使用
/// - Migration 檔案統一存放在 backend 專案中.
/// </summary>
public class MqttDbContext : DbContext
{
    public MqttDbContext(DbContextOptions<MqttDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets sensor data readings from EnergyMeter and Modbus devices.
    /// </summary>
    public DbSet<SensorData> SensorData { get; set; }
}
