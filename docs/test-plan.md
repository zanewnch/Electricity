# Test Plan

> Version: 0.1
> Date: 2026-02-24
> Status: Draft (no tests implemented yet)

---

## Test Strategy

Layered testing strategy, from bottom to top by importance:

```
         E2E Tests (few, end-to-end validation)
        ──────────────────────────────
       Integration Tests (API layer validation)
      ────────────────────────────────
     Unit Tests (logic units, fast and numerous)
```

---

## Test Scope by Layer

### 1. Unit Tests

**Goal:** Validate isolated business logic without database or external service dependencies.

| Test Subject | Test Cases |
|---|---|
| `DataGenerator` | EnergyMeter data value ranges are correct (voltage 218-222V, power factor 0.85-0.99) |
| `DataGenerator` | Modbus data PowerFactor / Frequency are null |
| `DataGenerator` | Watt = Current × Voltage (accurate to 2 decimal places) |
| `SensorDataService` (TBD) | Pagination logic is correct (page / pageSize / total calculation) |
| `SensorDataService` | `granularity` parameter validation (invalid values should throw exceptions) |

**Tools:** xUnit (.NET test framework, `tests/` directory)

---

### 2. Integration Tests

**Goal:** Validate API endpoint behavior using In-Memory or SQLite test databases, without depending on real SQL Server.

| Endpoint | Test Cases |
|---|---|
| `GET /api/sensor-data` | No filters, returns paginated results |
| `GET /api/sensor-data` | Filter by `deviceType`, returns only matching devices |
| `GET /api/sensor-data` | Filter by `from`/`to`, returns only data within the time range |
| `GET /api/sensor-data/{id}` | Existing ID returns 200 + correct data |
| `GET /api/sensor-data/{id}` | Non-existing ID returns 404 |
| `GET /api/sensor-data/latest` | Returns the most recent record per device |
| `GET /api/sensor-data/aggregate` | `granularity=hour` returns correct aggregation structure |
| `GET /api/sensor-data/aggregate` | Invalid `granularity` value returns 400 |
| `GET /api/sensor-data/aggregate` | `period` format is correct for each granularity (minute/hour/day/month) |

**Tools:** `WebApplicationFactory<Program>` + `Microsoft.EntityFrameworkCore.InMemory`

---

### 3. Frontend Component Tests

**Goal:** Validate Vue component rendering behavior and interaction logic.

| Component | Test Cases |
|---|---|
| `DeviceCard` | Correctly displays current, voltage, and power values |
| `DeviceCard` | EnergyMeter shows PowerFactor; Modbus does not |
| `FilterBar` | Selecting deviceType triggers correct API call |
| `AggregateChart` | Switching granularity triggers a new API call |

**Tools:** Vitest + Vue Test Utils

---

### 4. E2E Tests (End-to-End)

**Goal:** Validate complete user flows, requires backend service to be running.

| Flow | Test Cases |
|---|---|
| Dashboard load | Page displays the latest data for each device |
| Trend analysis | Switching granularity updates the chart |

**Tools:** Playwright (TBD)

---

## Coverage Targets

| Layer | Target Coverage |
|---|---|
| Backend unit tests | ≥ 80% |
| Backend integration tests | At least 1 happy path + 1 error case per API endpoint |
| Frontend component tests | Core components (DeviceCard, AggregateChart) ≥ 70% |
| E2E | Main user flows covered |

---

## Test Directory Structure (To Be Created)

```
tests/
├── backend/
│   ├── UnitTests/
│   │   ├── DataGeneratorTests.cs
│   │   └── SensorDataServiceTests.cs
│   └── IntegrationTests/
│       └── SensorDataControllerTests.cs
└── frontend/
    └── components/
        ├── DeviceCard.test.ts
        └── AggregateChart.test.ts
```
