# ADR-001: 選用 Vue 3 + .NET 10 + SQL Server 技術棧

## Status

Accepted

## Context

需要建立一套電力數據收集與視覺化系統，包含：持續運行的數據收集程序、RESTful API 後端、圖表 Web 前端、關聯式資料庫。

開發者（個人專案）對 .NET 和 JavaScript 生態有基本熟悉度，偏好 LTS 版本的成熟技術。

## Decision

選用以下技術棧：

| 層 | 技術 | 版本 |
|----|------|------|
| 前端 | Vue 3 + TypeScript + Vite | Vue 3.5, TS 5.9, Vite 7 |
| 後端 | ASP.NET Core Web API | .NET 10 (LTS) |
| 資料收集 | .NET Console App | .NET 10 |
| ORM | Entity Framework Core | 10.0.3 |
| 資料庫 | SQL Server | LocalDB（開發）|

替代方案評估：

| 層 | 考慮選項 | 捨棄原因 |
|----|----------|----------|
| 前端 | React | Boilerplate 較多，對此規模過重 |
| 前端 | Svelte | 生態較小，遇到問題資源較少 |
| 後端 | Node.js / Express | C# 型別系統對數據處理更嚴謹 |
| 後端 | Python / FastAPI | 與 .NET 混合增加複雜度 |
| 資料庫 | PostgreSQL | Windows 開發環境設定較繁瑣 |
| 資料庫 | SQLite | 不適合持續高頻寫入場景 |

## Consequences

**Positive:**
- .NET + EF Core 提供強型別模型，減少運行時錯誤
- Vue 3 Composition API 結構清晰，易於拆分元件
- LocalDB 開發環境零設定，快速啟動
- Collector 與 Backend 可透過 `shared/` 專案共用模型

**Negative:**
- SQL Server LocalDB 不適合正式環境，未來需遷移
- .NET 10 較新，部分套件生態尚在追趕
- 前後端分離需處理 CORS 設定
