# Test-Impl Output Format Example

The following is the standard output format template. When the Create workflow runs, it generates a `test-impl.md` file at `docs/impl/{feature-name}/test-impl.md` following this structure:

---

# {Feature Name} Test-Impl

## Overview

**Feature**: {feature name}
**Test Layer**: {Backend Unit / Backend Integration / Frontend Component / Collector / Multi-layer}
**Framework**: {xUnit / Vitest + Vue Test Utils}
**Date**: {creation date}

---

## Source Code Under Test

| File | Purpose | Key Methods/Logic |
|------|---------|-------------------|
| `backend/Services/SensorDataService.cs:42` | Business logic / aggregation | GetPaginated(), Aggregate() |
| `collector/Services/DataGenerator.cs:25` | Data generation | GenerateEnergyMeterData(), GenerateModbusData() |
| `shared/Models/SensorData.cs:8` | Entity model | Property constraints |

---

## Test Framework & Setup

**Framework**: xUnit (.NET)
**Dependencies**:
- `Microsoft.EntityFrameworkCore.InMemory` (for integration tests)
- `Moq` (for mocking, if needed)
- `WebApplicationFactory<Program>` (for integration tests)

**Test Project**: `tests/backend/UnitTests/`

**Setup**:
```csharp
// Example: InMemory DbContext setup
var options = new DbContextOptionsBuilder<MqttDbContext>()
    .UseInMemoryDatabase(databaseName: "TestDb")
    .Options;
var context = new MqttDbContext(options);
```

---

## Test Phases

### Phase 1: {test group name}

**Requirement**: Validate that {specific behavior description}

**Test Cases**:

| # | Test Name | Input / Precondition | Expected Result | Source Ref |
|---|-----------|---------------------|-----------------|------------|
| 1 | Should return correct voltage range | EnergyMeter device type | Voltage between 218-222V | `collector/Services/DataGenerator.cs:30` |
| 2 | Should return null PowerFactor for Modbus | Modbus device type | PowerFactor is null | `collector/Services/DataGenerator.cs:45` |
| 3 | Should calculate Watt correctly | Current=10A, Voltage=220V | Watt = 2200.00 (2 decimal places) | `collector/Services/DataGenerator.cs:52` |

**Setup / Teardown**:
- Setup: Initialize DataGenerator with test configuration
- Teardown: N/A (stateless)

---

### Phase 2: {test group name}

**Requirement**: Validate that {specific behavior description}

**Test Cases**:

| # | Test Name | Input / Precondition | Expected Result | Source Ref |
|---|-----------|---------------------|-----------------|------------|
| 1 | Should return paginated results | page=1, pageSize=10, 25 total records | 10 results, total=25 | `backend/Services/SensorDataService.cs:42` |
| 2 | Should throw on invalid granularity | granularity="invalid" | ArgumentException thrown | `backend/Services/SensorDataService.cs:68` |
| 3 | Should return empty list when no data | Empty database | Empty list, total=0 | `backend/Services/SensorDataService.cs:42` |

**Setup / Teardown**:
- Setup: Seed InMemory database with test data
- Teardown: Dispose DbContext

---

## Implementation Steps

- [ ] Create `tests/backend/UnitTests/DataGeneratorTests.cs` - Unit tests for data generation logic
- [ ] Add test: `Should_Return_Correct_Voltage_Range` - Tests `collector/Services/DataGenerator.cs:30`
- [ ] Add test: `Should_Return_Null_PowerFactor_For_Modbus` - Tests `collector/Services/DataGenerator.cs:45`
- [ ] Add test: `Should_Calculate_Watt_Correctly` - Tests `collector/Services/DataGenerator.cs:52`
- [ ] Create `tests/backend/UnitTests/SensorDataServiceTests.cs` - Unit tests for service logic
- [ ] Add test: `Should_Return_Paginated_Results` - Tests `backend/Services/SensorDataService.cs:42`
- [ ] Add test: `Should_Throw_On_Invalid_Granularity` - Tests `backend/Services/SensorDataService.cs:68`
- [ ] Add test: `Should_Return_Empty_When_No_Data` - Tests `backend/Services/SensorDataService.cs:42`

---

## Related Files

**Source Files Under Test:**
- Service: `backend/Services/SensorDataService.cs`
- Model: `shared/Models/SensorData.cs`
- DbContext: `shared/Data/MqttDbContext.cs`
- Collector: `collector/Services/DataGenerator.cs`

**Test Files (to create/modify):**
- `tests/backend/UnitTests/DataGeneratorTests.cs`
- `tests/backend/UnitTests/SensorDataServiceTests.cs`

---

## Notes

- {note 1}
- {note 2}
