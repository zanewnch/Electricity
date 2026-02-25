# Spec Output Format Example

The following is the standard output format for `spec.md`:

---

# {Feature Name} Spec

## Overview

**Feature**: {feature name}
**Component**: {frontend component name}
**Date**: {creation date}

---

## Frontend Analysis

### 1. Intention

The main purpose of {component} is to:
- {feature description 1}
- {feature description 2}

### 2. Request Paths (API calls)

| Service File | Request Path | Purpose |
|-------------|--------------|---------|
| `fe/src/app/services/xxx.service.ts:42` | GET `/api/xxx` | Fetch xxx data |
| `fe/src/app/services/xxx.service.ts:56` | POST `/api/xxx` | Create xxx |

### 3. UI Elements

| Element | Location | Displays |
|---------|----------|---------|
| `<app-energy-display>` | `fe/src/app/components/xxx.component.html:24` | kW value |
| `<mat-table>` | `fe/src/app/components/xxx.component.html:48` | List data |

---

## Backend Analysis

### 1. API Endpoints

| Route File | Handler File | HTTP Method + Path |
|-----------|-------------|-------------------|
| `be/routes/api.js:128` | `be/controller/xxxController.js:45` | GET `/api/xxx` |
| `be/routes/api.js:135` | `be/controller/xxxController.js:89` | POST `/api/xxx` |

### 2. DB Schema

#### Table: `energy_meter_data`

| Column | Type | Description |
|--------|------|-------------|
| id | INT | Primary Key |
| device_id | VARCHAR(50) | Device ID |
| kw_value | DECIMAL(10,2) | Real-time power |
| timestamp | DATETIME | Timestamp |

### 3. Data Workflow (full trace)

```
1. MQTT receive
   └─ be/mqtt.js:156 - Subscribe to topic "/device/+/energy"
         ↓
2. Dispatch processing
   └─ be/mqtt.js:178 - dispatchMessage() parses payload
         ↓
3. Cache access
   └─ be/cache/energyCache.js:34 - setDeviceData() stores latest value
         ↓
4. Storage calculation
   └─ be/storage/energyStorage.js:67 - calculateAggregation() computes aggregation
         ↓
5. IPC response
   └─ be/ipc/ipcHandler.js:89 - sendToMain() sends to main process
         ↓
6. Handler processing
   └─ be/handler/energyHandler.js:112 - processEnergyData() formats data
         ↓
7. DB insert
   └─ be/model/energyModel.js:45 - insertEnergyData() writes to database
```

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
- [ ] Modify `be/handler/energyHandler.js:112` - Add energy meter data processing
- [ ] Modify `be/model/energyModel.js:45` - Add insert fields
- [ ] Modify `fe/src/app/components/xxx.component.ts:34` - Add display logic
- [ ] Modify `fe/src/app/components/xxx.component.html:24` - Add UI element

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

## Notes

- {note 1}
- {note 2}
