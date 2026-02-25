# ADR-001: Adopt Vue 3 + .NET 10 + SQL Server Tech Stack

## Status

Accepted

## Context

We need to build an electricity data collection and visualization system consisting of: a continuously running data collector, a RESTful API backend, a charting web frontend, and a relational database.

The developer (personal project) has a basic familiarity with the .NET and JavaScript ecosystems and prefers mature, LTS technologies.

## Decision

Adopt the following tech stack:

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Vue 3 + TypeScript + Vite | Vue 3.5, TS 5.9, Vite 7 |
| Backend | ASP.NET Core Web API | .NET 10 (LTS) |
| Data Collector | .NET Console App | .NET 10 |
| ORM | Entity Framework Core | 10.0.3 |
| Database | SQL Server | LocalDB (development) |

Alternatives considered:

| Layer | Option | Reason Rejected |
|-------|--------|----------------|
| Frontend | React | More boilerplate; too heavy for this scale |
| Frontend | Svelte | Smaller ecosystem; fewer resources when issues arise |
| Backend | Node.js / Express | C# type system is stricter for data processing |
| Backend | Python / FastAPI | Mixing with .NET adds complexity |
| Database | PostgreSQL | More involved setup on Windows dev environment |
| Database | SQLite | Not suitable for continuous high-frequency writes |

## Consequences

**Positive:**
- .NET + EF Core provides strongly-typed models, reducing runtime errors
- Vue 3 Composition API has a clear structure and is easy to split into components
- LocalDB requires zero configuration in development, enabling fast startup
- Collector and Backend can share models via the `shared/` project

**Negative:**
- SQL Server LocalDB is not suitable for production; migration will be needed later
- .NET 10 is relatively new; some package ecosystems are still catching up
- Frontend/backend separation requires handling CORS configuration
