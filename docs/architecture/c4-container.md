# C4 — Container Diagram

展開系統邊界，顯示各容器（可獨立部署的單元）及其互動。

```mermaid
C4Container
    title 電力數據分析系統 — Container Diagram

    Person(user, "使用者", "透過瀏覽器操作")

    System_Boundary(electricity, "電力數據分析系統") {
        Container(frontend, "Frontend", "Vue 3 / TypeScript / Vite", "提供 Dashboard、歷史查詢、趨勢分析的 Web UI\nport: 5173")
        Container(backend, "Backend API", "ASP.NET Core / .NET 10", "提供 RESTful API，處理查詢與聚合邏輯\nport: 5086 / 7105")
        Container(collector, "Collector", ".NET 10 Console App", "定期產生模擬感測數據並寫入資料庫\n每 3.5 秒一次")
        ContainerDb(db, "Database", "SQL Server LocalDB", "儲存所有感測器原始數據\nDatabase: MqttDb\nTable: SensorData")
    }

    Rel(user, frontend, "操作", "HTTPS / Browser")
    Rel(frontend, backend, "呼叫 REST API", "HTTP/JSON")
    Rel(backend, db, "讀取數據", "EF Core / SQL")
    Rel(collector, db, "寫入數據", "EF Core / SQL")
```

---

## 容器說明

| 容器 | 技術 | 職責 |
|------|------|------|
| Frontend | Vue 3 + Vite | Web UI，呼叫 Backend API 顯示數據 |
| Backend API | ASP.NET Core | 提供查詢、篩選、時間聚合 API |
| Collector | .NET Console | 模擬感測器，定時寫入數據 |
| Database | SQL Server LocalDB | 持久化所有 SensorData |

## 關鍵資料流

```
Collector ──(每 3.5 秒寫入)──► Database
                                    │
                               Backend API ◄──(HTTP)──► Frontend ◄──(HTTPS)──► 使用者
```

## 部署備註

- Collector 與 Backend 共用 `shared/` 專案中的 `MqttDbContext` 與 `SensorData` 模型
- 開發環境下 Collector 與 Backend 可同時在本機執行，連到同一個 LocalDB
