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
}
