# Workflow: Create

## Output Location

```
docs/impl/{feature-name}/test-impl.md
```

## Analysis Workflow

Use deep analysis to identify all testable logic, branches, and edge cases in the source code.

### 1. Test Scope Identification

Determine which test layer(s) apply based on the feature:

| Test Layer | Framework | When to Use |
|-----------|-----------|-------------|
| Backend Unit Tests | xUnit (.NET) | Isolated business logic (services, data generation, calculations) |
| Backend Integration Tests | xUnit + WebApplicationFactory + InMemory DB | API endpoint behavior, request/response validation |
| Frontend Component Tests | Vitest + Vue Test Utils | Vue component rendering, interaction logic |
| Collector Unit Tests | xUnit (.NET) | Data collection, generation, and storage logic |

### 2. Source Code Analysis

Read the source files under test to understand testable logic:

**Backend (.NET 10 / C#):**
- Service: `backend/Services/{Name}Service.cs` - Business logic methods, edge cases, parameter validation
- Controller: `backend/Controllers/{Name}Controller.cs` - Endpoint routing, response codes, DTO mapping
- Model/Entity: `shared/Models/{Name}.cs` - Data constraints, relationships

**Frontend (Vue 3 + TypeScript):**
- Component: `frontend/src/components/{Name}.vue` - Render behavior, conditional display, event handling
- Composable: `frontend/src/composables/use{Name}.ts` - Reactive logic, computed properties
- Store: `frontend/src/stores/{name}.ts` - State mutations, action side effects

**Collector (.NET Console App):**
- Service: `collector/Services/{Name}.cs` - Data generation ranges, value constraints
- Program: `collector/Program.cs` - Service configuration, scheduling

For each source file:
- **Read the file** to identify methods/functions
- **Identify testable behaviors**: What does each method do? What are the inputs/outputs?
- **Identify edge cases**: Null inputs, boundary values, empty collections, invalid parameters
- **Identify branches**: Conditional logic (if/else, switch) that requires separate test cases

### 3. Test Framework & Setup

Specify the required test infrastructure:

**Backend (.NET xUnit):**
```
- Framework: xUnit
- Mocking: Moq (if needed)
- DB: Microsoft.EntityFrameworkCore.InMemory (for integration tests)
- Setup: WebApplicationFactory<Program> (for integration tests)
- Project: tests/backend/UnitTests/ or tests/backend/IntegrationTests/
```

**Frontend (Vitest):**
```
- Framework: Vitest
- Utils: @vue/test-utils
- Setup: vitest.config.ts
- Project: tests/frontend/components/
```

### 4. Test Phases

Each Phase is a logical group of related test cases:

```markdown
### Phase N: {test group name}

**Requirement**: What behavior to validate

**Test Cases**:

| # | Test Name | Input / Precondition | Expected Result | Source Ref |
|---|-----------|---------------------|-----------------|------------|
| 1 | {descriptive test name} | {input or setup state} | {expected output or behavior} | `{file:line}` |
| 2 | {descriptive test name} | {edge case input} | {expected error or boundary behavior} | `{file:line}` |

**Setup / Teardown** (if needed):
- Setup: {what to prepare before tests in this phase}
- Teardown: {what to clean up after tests in this phase}
```

**Phase design principles:**
- Group by feature area or method under test (not by test type)
- Each phase should be independently runnable
- Include both happy path AND edge case tests
- Source Ref must point to the actual line of code being tested

### 5. Implementation Steps (independent section)

Consolidate all test files to create into checkbox list:

```markdown
## Implementation Steps

- [ ] Create `{test-file-path}` - {Brief description of test file purpose}
- [ ] Add test: `{test name}` - Tests `{source-file:line}` {what it validates}
- [ ] Add test: `{test name}` - Tests `{source-file:line}` {what it validates}
```

## Completion Message

```
Test-impl created at docs/impl/{feature-name}/test-impl.md
```
