# C4 — Component Diagram

Expands the internal component structure of each container.

---

## Backend API Components

```mermaid
C4Component
    title Backend API — Component Diagram

    Container_Ext(frontend, "Frontend", "Vue 3", "Calls API")
    ContainerDb_Ext(db, "Database", "SQL Server", "SensorData table")

    Container_Boundary(backend, "Backend API") {
        Component(controller, "SensorDataController", "ASP.NET Core Controller", "Handles HTTP requests, routes to corresponding endpoints\nGET /api/sensor-data/*")
        Component(service, "SensorDataService", "C# Service", "Encapsulates query logic, aggregation calculation, and pagination handling")
        Component(dbContext, "MqttDbContext", "EF Core DbContext", "Database access layer\nDefined in shared/Data/")
        Component(model, "SensorData", "C# Model", "Sensor data entity\nDefined in shared/Models/")
    }

    Rel(frontend, controller, "HTTP GET", "JSON")
    Rel(controller, service, "Calls")
    Rel(service, dbContext, "LINQ query")
    Rel(dbContext, db, "SQL", "EF Core")
    Rel(dbContext, model, "Maps to")
```

> `SensorDataService` is not yet created and needs to be implemented.

---

## Collector Components

```mermaid
C4Component
    title Collector — Component Diagram

    ContainerDb_Ext(db, "Database", "SQL Server", "SensorData table")

    Container_Boundary(collector, "Collector") {
        Component(program, "Program.cs", "Entry Point", "Main loop: triggers every 3.5 seconds\nHandles graceful Ctrl+C exit")
        Component(generator, "DataGenerator", "C# Service", "Generates simulated data for EnergyMeter and Modbus")
        Component(dbContext, "MqttDbContext", "EF Core DbContext", "Shared with Backend\nDefined in shared/Data/")
    }

    Rel(program, generator, "Calls Generate()")
    Rel(program, dbContext, "Calls SaveChangesAsync()")
    Rel(dbContext, db, "INSERT SensorData", "EF Core / SQL")
```

---

## Frontend Components

```mermaid
C4Component
    title Frontend — Component Diagram

    Container_Ext(backend, "Backend API", "ASP.NET Core", "REST API")

    Container_Boundary(frontend, "Frontend") {
        Component(router, "Vue Router", "Routing", "Manages page routing: / / /history / /analysis")
        Component(dashboard, "DashboardView", "Vue Component", "Real-time data overview\nCalls /api/sensor-data/latest")
        Component(history, "HistoryView", "Vue Component", "Historical data query and paginated table\nCalls /api/sensor-data")
        Component(analysis, "AnalysisView", "Vue Component", "Time aggregation trend chart (core)\nCalls /api/sensor-data/aggregate")
        Component(apiService, "sensorDataService.ts", "TypeScript Service", "Encapsulates all API calls, centrally manages Base URL")
    }

    Rel(router, dashboard, "Route /")
    Rel(router, history, "Route /history")
    Rel(router, analysis, "Route /analysis")
    Rel(dashboard, apiService, "Calls")
    Rel(history, apiService, "Calls")
    Rel(analysis, apiService, "Calls")
    Rel(apiService, backend, "HTTP GET", "JSON")
```

> Frontend components are not yet implemented (App.vue is empty), vue-router needs to be installed first.

---

## Data Model

### SensorData (`shared/Models/SensorData.cs`)

| Field | C# Type | SQL Type | Nullable | Description |
|-------|---------|----------|----------|-------------|
| `Id` | `int` | `int IDENTITY(1,1)` | No | Primary key |
| `DeviceType` | `string` | `nvarchar(max)` | No | `EnergyMeter` / `Modbus` |
| `BleAddress` | `string` | `nvarchar(max)` | No | MAC address |
| `Current` | `double` | `float` | No | Current (A) |
| `Voltage` | `double` | `float` | No | Voltage (V) |
| `Watt` | `double` | `float` | No | Power (W) |
| `PowerFactor` | `double?` | `float` | Yes | Power factor |
| `Frequency` | `double?` | `float` | Yes | Frequency (Hz) |
| `Timestamp` | `DateTime` | `datetime2` | No | Measurement time |
