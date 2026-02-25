# Impl Output Format Example

The following is the standard output format template. When the Create workflow runs, it generates a `impl.md` file at `docs/impl/{feature-name}/impl.md` following this structure:

---

# {Feature Name} Impl

## Overview

**Feature**: {feature name}
**Component**: {frontend component / backend service name}
**Date**: {creation date}

---

## Frontend Analysis (Vue 3 + TypeScript)

### 1. Intention

The main purpose of {component} is to:
- {feature description 1}
- {feature description 2}

### 2. Request Paths (API calls)

| Service/API File | Request Path | Purpose |
|-----------------|--------------|---------|
| `frontend/src/api/electricity.ts:42` | GET `/api/sensor-data` | Fetch sensor data |
| `frontend/src/api/electricity.ts:56` | POST `/api/sensor-data` | Submit new reading |

### 3. UI Elements

| Element | Location | Displays |
|---------|----------|---------|
| `<ElectricityChart />` | `frontend/src/components/ElectricityChart.vue:24` | Time-series chart |
| `<DataTable />` | `frontend/src/components/DataTable.vue:48` | Tabular data |

---

## Backend Analysis (.NET 10 / C#)

### 1. API Endpoints

| Controller File | Method | HTTP Method + Path |
|----------------|--------|-------------------|
| `backend/Controllers/SensorDataController.cs:28` | GetAll() | GET `/api/sensor-data` |
| `backend/Controllers/SensorDataController.cs:45` | Create() | POST `/api/sensor-data` |

### 2. DB Schema (EF Core)

#### Entity: `SensorData`

| Property | Type | Description |
|----------|------|-------------|
| Id | int | Primary Key |
| DeviceId | string | Device identifier |
| KwValue | decimal | Real-time power (kW) |
| Timestamp | DateTime | Reading timestamp |

**DbContext**: `shared/Data/MqttDbContext.cs`
**Model**: `shared/Models/SensorData.cs`

### 3. Data Workflow (full trace)

```
1. Data collection
   └─ collector/Services/DataGenerator.cs:30 - Generate or fetch sensor data
         ↓
2. Database storage
   └─ shared/Data/MqttDbContext.cs:15 - EF Core SaveChanges()
         ↓
3. API query
   └─ backend/Controllers/SensorDataController.cs:28 - GET endpoint
         ↓
4. Service processing
   └─ backend/Services/SensorDataService.cs:42 - Business logic / aggregation
         ↓
5. Frontend consumption
   └─ frontend/src/api/electricity.ts:42 - Axios/fetch call
         ↓
6. UI rendering
   └─ frontend/src/components/ElectricityChart.vue:58 - Display data
```

---

## Collector Analysis (.NET Console App)

### 1. Data Collection Flow

| File | Purpose |
|------|---------|
| `collector/Program.cs:12` | Entry point, configure services |
| `collector/Services/DataGenerator.cs:25` | Data generation / collection logic |

---

## Requirement Phases

### Phase 1: {phase name}

**Requirement**:
{specific requirement description}

**Modification Context**:
- **Why**: {reason for change}
- **How**: {approach}
- **Impact scope**: {affected files/features}

**Implementation Steps**:
- [ ] Modify `backend/Controllers/SensorDataController.cs:28` - Add new endpoint
- [ ] Modify `shared/Models/SensorData.cs:15` - Add new property
- [ ] Modify `frontend/src/components/Dashboard.vue:34` - Add display logic
- [ ] Modify `frontend/src/api/electricity.ts:42` - Add API call

---

### Phase 2: {phase name}

**Requirement**:
{specific requirement description}

**Modification Context**:
- **Why**: {reason for change}
- **How**: {approach}

**Implementation Steps**:
- [ ] {modification item 1} (include file:line)
- [ ] {modification item 2} (include file:line)

---

## Related Files

**Frontend (Vue 3):**
- Component: `frontend/src/components/{Name}.vue`
- Composable: `frontend/src/composables/use{Name}.ts`
- Store: `frontend/src/stores/{name}.ts`
- API: `frontend/src/api/{name}.ts`
- Type: `frontend/src/types/{name}.ts`

**Backend (.NET):**
- Controller: `backend/Controllers/{Name}Controller.cs`
- Service: `backend/Services/{Name}Service.cs`
- DTO: `backend/DTOs/{Name}Dto.cs`

**Shared:**
- Model: `shared/Models/{Name}.cs`
- DbContext: `shared/Data/MqttDbContext.cs`

**Collector:**
- Service: `collector/Services/{Name}.cs`

---

## Notes

- {note 1}
- {note 2}
