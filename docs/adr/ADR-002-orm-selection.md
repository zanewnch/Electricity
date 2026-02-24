# ADR-002: Adopt Entity Framework Core as the ORM

## Status

Accepted

## Context

The backend needs a data access layer to interact with the SQL Server database. The two main candidates were Entity Framework Core (full-featured ORM) and Dapper (lightweight micro-ORM). The system requires migration-based schema management, and the Collector and Backend need to share the same `DbContext`.

## Decision

Adopt **Entity Framework Core 10.0.3**.

## Consequences

**Positive:**
- `dotnet ef migrations` manages schema changes with a traceable history
- LINQ `GroupBy` can be used directly for aggregation queries (see RFC-001)
- `DbContext` can be shared between Collector and Backend via the `shared/` project
- Native integration with ASP.NET Core DI; no additional configuration needed

**Negative:**
- Complex EF Core `GroupBy` expressions sometimes cannot be fully translated to optimal SQL; generated queries should be verified
- If aggregation query performance is insufficient, `FromSqlRaw` may be needed as a fallback (see RFC-001 Option B)
- Slightly lower performance compared to Dapper, though negligible at this scale

Alternatives considered:

| Option | Reason Rejected |
|--------|----------------|
| Dapper | Requires hand-written SQL; migrations need an additional tool (e.g. Fluent Migrator) |
| Raw ADO.NET | Too verbose; easy to introduce SQL injection risks |
