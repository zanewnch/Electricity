# RFC-001: 時間聚合查詢設計

## Status

- [ ] Draft
- [x] Under Review
- [ ] Accepted
- [ ] Rejected

## Problem

目前系統以每 3.5 秒一筆的頻率收集數據。原始資料量大，直接在前端繪圖有兩個問題：
1. 傳輸量過大，影響效能
2. 資料點密集，趨勢不易閱讀

需要一個聚合層，驗證「依分/時/日/月分組的用電趨勢，是否能反映出有意義的規律」。

## Goals

- 新增 API 端點支援時間聚合查詢
- 支援四種粒度：`minute` / `hour` / `day` / `month`
- 回傳各時間段的 avgWatt、maxWatt、minWatt、sumWatt、count
- 支援依裝置類型篩選

## Non-Goals

- 預計算或快取聚合結果（目前數據量不需要）
- 前端圖表實作（另行處理）
- kWh 換算（由前端依需求換算）

## Proposed Solution

### 端點設計

```
GET /api/sensor-data/aggregate
```

| 參數 | 型別 | 必填 | 說明 |
|------|------|------|------|
| `granularity` | `string` | 是 | `minute` / `hour` / `day` / `month` |
| `deviceType` | `string` | 否 | `EnergyMeter` / `Modbus` |
| `bleAddress` | `string` | 否 | 裝置 MAC |
| `from` | `string` | 否 | ISO 8601 起始時間 |
| `to` | `string` | 否 | ISO 8601 結束時間 |

Response 範例（`granularity=hour`）：

```json
{
  "granularity": "hour",
  "data": [
    {
      "period": "2026-02-24T10:00:00",
      "avgWatt": 9252.38,
      "maxWatt": 21890.00,
      "minWatt": 100.50,
      "sumWatt": 33308.57,
      "count": 1028
    }
  ]
}
```

### 實作方式：EF Core GroupBy

```csharp
var grouped = granularity switch
{
    "hour" => query.GroupBy(x => new {
        x.Timestamp.Year, x.Timestamp.Month,
        x.Timestamp.Day, x.Timestamp.Hour
    }),
    "day"  => query.GroupBy(x => new {
        x.Timestamp.Year, x.Timestamp.Month, x.Timestamp.Day
    }),
    // ... minute, month
    _ => throw new ArgumentException("Invalid granularity")
};
```

## Alternatives Considered

| 選項 | Pros | Cons |
|------|------|------|
| EF Core GroupBy（推薦） | 型別安全、可維護 | 複雜 GroupBy 可能無法完整翻譯成 SQL，需實測 |
| Raw SQL (`FromSqlRaw`) | SQL 行為明確可控 | 需參數化查詢防 injection；跨資料庫移植性差 |
| 前端聚合 | 不需後端改動 | 傳輸量過大，不可行 |
| 預計算快取表 | 查詢極快 | 維護複雜，目前數據量不需要 |

## Open Questions

- [ ] EF Core `DateTime.GroupBy` 是否能正確翻譯為 SQL Server `DATEPART`？（需實測確認）
- [ ] 聚合結果是否需要分頁？（month 粒度最多 12 筆，應不需要）

## References

- [PRD — F3 時間聚合趨勢分析](../PRD.md)
- [ADR-002 — ORM 選擇](../adr/ADR-002-orm-selection.md)
- [api/openapi.yaml — aggregate 端點定義](../api/openapi.yaml)
