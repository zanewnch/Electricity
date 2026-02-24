# C4 — Container Diagram

Expands the system boundary to show each container (independently deployable unit) and their interactions.

```mermaid
C4Container
    title Electricity Data Analysis System — Container Diagram

    Person(user, "User", "Operates via browser")

    System_Boundary(electricity, "Electricity Data Analysis System") {
        Container(frontend, "Frontend", "Vue 3 / TypeScript / Vite", "Provides Web UI for Dashboard, historical queries, and trend analysis\nport: 5173")
        Container(backend, "Backend API", "ASP.NET Core / .NET 10", "Provides RESTful API, handles queries and aggregation logic\nport: 5086 / 7105")
        Container(collector, "Collector", ".NET 10 Console App", "Periodically generates simulated sensor data and writes to database\nevery 3.5 seconds")
        ContainerDb(db, "Database", "SQL Server LocalDB", "Stores all raw sensor data\nDatabase: MqttDb\nTable: SensorData")
    }

    Rel(user, frontend, "Operates", "HTTPS / Browser")
    Rel(frontend, backend, "Calls REST API", "HTTP/JSON")
    Rel(backend, db, "Reads data", "EF Core / SQL")
    Rel(collector, db, "Writes data", "EF Core / SQL")
```

---

## Container Descriptions

| Container | Technology | Responsibility |
|-----------|------------|----------------|
| Frontend | Vue 3 + Vite | Web UI, calls Backend API to display data |
| Backend API | ASP.NET Core | Provides query, filtering, and time aggregation APIs |
| Collector | .NET Console | Simulates sensor, periodically writes data |
| Database | SQL Server LocalDB | Persists all SensorData |

## Key Data Flow

```
Collector ──(writes every 3.5s)──► Database
                                        │
                               Backend API ◄──(HTTP)──► Frontend ◄──(HTTPS)──► User
```

## Deployment Notes

- Collector and Backend share the `MqttDbContext` and `SensorData` model from the `shared/` project
- In the development environment, Collector and Backend can run simultaneously on the local machine, connecting to the same LocalDB
