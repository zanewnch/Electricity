// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Data;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<MqttDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("backend")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed dummy data on startup
using IServiceScope scope = app.Services.CreateScope();
MqttDbContext db = scope.ServiceProvider.GetRequiredService<MqttDbContext>();
await SensorDataSeeder.SeedAsync(db);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
