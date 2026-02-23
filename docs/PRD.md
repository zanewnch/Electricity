# PRD — 電力數據分析系統

> 版本：0.1
> 日期：2026-02-24
> 狀態：草稿

---

## 背景與動機

本專案的出發點是建立一套個人用電監控系統，能夠從感測裝置持續收集電力數據，並透過 Web 介面進行視覺化分析。

核心驗證目標：**不同時間粒度（分/時/日/月）的用電聚合，是否能反映出有意義的趨勢**，作為後續節能分析或異常偵測的基礎。

---

## 目標使用者

| 角色 | 說明 |
|------|------|
| 系統開發者（自己） | 主要使用者，進行用電數據的收集與分析驗證 |
| 未來擴充 | 設備管理人員、能源管理系統整合 |

---

## 功能需求

### F1 — Dashboard 即時總覽

作為使用者，我希望在首頁看到各裝置的最新量測值，以便快速掌握當前用電狀況。

**驗收條件：**
- 顯示每台裝置（依 BleAddress 分組）的最新一筆資料
- 包含電流、電壓、功率數值
- EnergyMeter 額外顯示 PowerFactor 與 Frequency
- 顯示最後更新時間

### F2 — 歷史資料查詢

作為使用者，我希望能查詢指定時間範圍內的原始感測資料，以便回溯特定時段的用電記錄。

**驗收條件：**
- 支援依裝置類型篩選（EnergyMeter / Modbus / 全部）
- 支援依時間範圍篩選
- 結果以分頁表格呈現，每頁 50 筆
- 顯示欄位：時間、裝置、電壓、電流、功率、功率因數、頻率

### F3 — 時間聚合趨勢分析（核心功能）

作為使用者，我希望能選擇不同時間粒度（分/時/日/月）查看聚合用電趨勢圖，以便驗證哪種粒度對用電分析最有意義。

**驗收條件：**
- 支援四種粒度：`minute` / `hour` / `day` / `month`
- 每個時間段顯示：avgWatt、maxWatt、minWatt、sumWatt
- 以折線圖呈現時間趨勢
- 支援依裝置篩選

---

## 非功能需求

| 項目 | 需求 |
|------|------|
| 效能 | 列表查詢 ≤ 500ms；聚合查詢 ≤ 1s |
| 安全性 | 連線字串不得提交至版本控制 |
| 可維護性 | Collector 的數據來源可替換（dummy → 真實裝置）不影響後端 |
| 開發環境 | SQL Server LocalDB；前後端分離開發 |

---

## 範圍

### 納入（In Scope）
- 模擬感測器數據產生（Collector）
- 數據持久化至 SQL Server
- RESTful API（含時間聚合端點）
- Vue 前端（Dashboard、歷史查詢、趨勢分析）

### 排除（Out of Scope）
- 真實 BLE / MQTT 裝置整合
- 使用者身份驗證與權限管理
- 行動裝置 App
- 即時推播（WebSocket / SSE）

---

## 成功指標

- [ ] 聚合端點（minute/hour/day/month）成功回傳有意義的趨勢數據
- [ ] 前端圖表能清楚呈現不同粒度下的用電差異
- [ ] Collector 可穩定運行並持續寫入數據

---

## Open Questions

- [ ] 正式環境資料庫選型？（SQL Server / PostgreSQL）
- [ ] 前端圖表套件選擇？（ECharts / Chart.js / ApexCharts）
- [ ] 時間聚合驗證後，哪個粒度最有分析意義？
- [ ] 資料保留政策？（保留多久、是否封存）
- [ ] Collector 未來是否接真實 BLE 或 MQTT 裝置？

---

## 相關文件

| 文件 | 說明 |
|------|------|
| [rfc/RFC-001-time-aggregation-query.md](./rfc/RFC-001-time-aggregation-query.md) | 時間聚合端點設計提案 |
| [adr/ADR-001-tech-stack.md](./adr/ADR-001-tech-stack.md) | 技術棧選擇決策 |
| [architecture/c4-container.md](./architecture/c4-container.md) | 系統容器架構圖 |
| [api/openapi.yaml](./api/openapi.yaml) | API 正式規格 |
| [test-plan.md](./test-plan.md) | 測試策略 |
