# RFC-001: Time Aggregation Query Design

## Status

- [ ] Draft
- [x] Under Review
- [ ] Accepted
- [ ] Rejected

## Problem

The current system collects data at a frequency of one record every 3.5 seconds. The raw data volume is large, and directly charting it on the frontend has two problems:
1. Excessive data transfer volume affects performance
2. Data points are too dense, making trends difficult to read

An aggregation layer is needed to verify whether "electricity usage trends grouped by minute/hour/day/month can reveal meaningful patterns."

## Goals

- Add API endpoint to support time aggregation queries
- Support four granularities: `minute` / `hour` / `day` / `month`
- Return avgWatt, maxWatt, minWatt, sumWatt, and count for each time period
- Support filtering by device type

## Non-Goals

- Pre-calculation or caching of aggregated results (current data volume doesn't require it)
- Frontend chart implementation (handled separately)
- kWh conversion (frontend converts as needed)

## Proposed Solution

### Endpoint Design

```
GET /api/sensor-data/aggregate
```

| Parameter | Type | Required | Description |
|------|------|------|------|
| `granularity` | `string` | Yes | `minute` / `hour` / `day` / `month` |
| `deviceType` | `string` | No | `EnergyMeter` / `Modbus` |
| `bleAddress` | `string` | No | Device MAC address |
| `from` | `string` | No | ISO 8601 start time |
| `to` | `string` | No | ISO 8601 end time |

Response example (`granularity=hour`):

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

### Implementation Approach: EF Core GroupBy

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

| Option | Pros | Cons |
|------|------|------|
| EF Core GroupBy (Recommended) | Type-safe, maintainable | Complex GroupBy may not fully translate to SQL, requires testing |
| Raw SQL (`FromSqlRaw`) | SQL behavior is explicit and controllable | Requires parameterized queries to prevent injection; poor cross-database portability |
| Frontend aggregation | No backend changes needed | Excessive data transfer, not viable |
| Pre-calculated cache table | Extremely fast queries | Complex maintenance, current data volume doesn't require it |

## Open Questions

- [ ] Can EF Core `DateTime.GroupBy` correctly translate to SQL Server `DATEPART`? (Requires testing to confirm)
- [ ] Do aggregation results need pagination? (Month granularity has at most 12 records, should not be needed)

## References

- [PRD — F3 Time Aggregation Trend Analysis](../PRD.md)
- [ADR-002 — ORM Selection](../adr/ADR-002-orm-selection.md)
- [api/openapi.yaml — aggregate endpoint definition](../api/openapi.yaml)
