# PRD — Electricity Data Analysis System

> Version: 0.1
> Date: 2026-02-24
> Status: Draft

---

## Background and Motivation

This project aims to build a personal electricity monitoring system that continuously collects power data from sensor devices and provides a web interface for visual analysis.

Core validation goal: **Whether aggregating electricity usage at different time granularities (minute/hour/day/month) can reveal meaningful trends**, serving as the foundation for future energy-saving analysis or anomaly detection.

---

## Target Users

| Role | Description |
|------|-------------|
| System Developer (self) | Primary user, collecting and validating electricity data analysis |
| Future Extension | Equipment managers, energy management system integration |

---

## Functional Requirements

### F1 — Dashboard Real-time Overview

As a user, I want to see the latest measurements from each device on the home page so I can quickly understand current electricity usage.

**Acceptance Criteria:**
- Display the most recent record per device (grouped by BleAddress)
- Include current, voltage, and power values
- EnergyMeter additionally shows PowerFactor and Frequency
- Display last updated time

### F2 — Historical Data Query

As a user, I want to query raw sensor data within a specified time range so I can review electricity records from specific periods.

**Acceptance Criteria:**
- Support filtering by device type (EnergyMeter / Modbus / All)
- Support filtering by time range
- Results displayed in a paginated table, 50 records per page
- Displayed columns: time, device, voltage, current, power, power factor, frequency

### F3 — Time Aggregation Trend Analysis (Core Feature)

As a user, I want to select different time granularities (minute/hour/day/month) to view aggregated electricity trend charts, so I can validate which granularity is most meaningful for electricity analysis.

**Acceptance Criteria:**
- Support four granularities: `minute` / `hour` / `day` / `month`
- Each time period shows: avgWatt, maxWatt, minWatt, sumWatt
- Display time trends as line charts
- Support filtering by device

---

## Non-Functional Requirements

| Item | Requirement |
|------|-------------|
| Performance | List query ≤ 500ms; aggregate query ≤ 1s |
| Security | Connection strings must not be committed to version control |
| Maintainability | Collector data source can be replaced (dummy → real device) without affecting the backend |
| Development Environment | SQL Server LocalDB; frontend/backend separated development |

---

## Scope

### In Scope
- Simulated sensor data generation (Collector)
- Data persistence to SQL Server
- RESTful API (including time aggregation endpoints)
- Vue frontend (Dashboard, history query, trend analysis)

### Out of Scope
- Real BLE / MQTT device integration
- User authentication and authorization
- Mobile app
- Real-time push (WebSocket / SSE)

---

## Success Metrics

- [ ] Aggregation endpoints (minute/hour/day/month) successfully return meaningful trend data
- [ ] Frontend charts clearly display electricity differences across different granularities
- [ ] Collector runs stably and continuously writes data

---

## Open Questions

- [ ] Production database choice? (SQL Server / PostgreSQL)
- [ ] Frontend chart library? (ECharts / Chart.js / ApexCharts)
- [ ] After time aggregation validation, which granularity is most analytically meaningful?
- [ ] Data retention policy? (how long to keep, whether to archive)
- [ ] Will Collector eventually connect to real BLE or MQTT devices?

---

## Related Documents

| Document | Description |
|----------|-------------|
| [rfc/RFC-001-time-aggregation-query.md](./rfc/RFC-001-time-aggregation-query.md) | Time aggregation endpoint design proposal |
| [adr/ADR-001-tech-stack.md](./adr/ADR-001-tech-stack.md) | Tech stack decision |
| [architecture/c4-container.md](./architecture/c4-container.md) | System container architecture diagram |
| [api/openapi.yaml](./api/openapi.yaml) | Formal API specification |
| [test-plan.md](./test-plan.md) | Test strategy |
