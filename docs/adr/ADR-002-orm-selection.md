# ADR-002: 選用 Entity Framework Core 作為 ORM

## Status

Accepted

## Context

後端需要一個資料存取層來操作 SQL Server 資料庫。主要候選方案為 Entity Framework Core（全功能 ORM）和 Dapper（輕量 Micro-ORM）。系統需要 Migration 管理 Schema 變更，且 Collector 與 Backend 需共用同一個 DbContext。

## Decision

選用 **Entity Framework Core 10.0.3**。

## Consequences

**Positive:**
- `dotnet ef migrations` 管理 Schema 變更，歷史可追蹤
- LINQ GroupBy 可直接用於聚合查詢（見 RFC-001）
- `DbContext` 可在 Collector 和 Backend 間共享（透過 `shared/` 專案）
- 與 ASP.NET Core DI 原生整合，無需額外設定

**Negative:**
- EF Core 的複雜 GroupBy 有時無法完整翻譯成最優 SQL，需確認生成查詢
- 若聚合查詢效能不足，需改用 `FromSqlRaw` 補充（見 RFC-001 Option B）
- 相較 Dapper，效能略低，但對此規模無實際影響

替代方案：

| 選項 | 捨棄原因 |
|------|----------|
| Dapper | 需手寫 SQL；Migration 需額外工具（如 Fluent Migrator） |
| Raw ADO.NET | 過於繁瑣；容易引入 SQL injection 風險 |
