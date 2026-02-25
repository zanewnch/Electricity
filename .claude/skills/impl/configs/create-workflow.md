# Workflow: Create

## Output Location

```
docs/impl/{feature-name}/impl.md
```

## Analysis Workflow

Use deep analysis to ensure every step of data workflow is correctly connected.

### 1. Frontend Analysis (Vue 3 + TypeScript)

- **Source Files** (must be specified):
  - Component: `frontend/src/components/{Name}.vue` or `frontend/src/views/{Name}.vue`
  - Composable: `frontend/src/composables/use{Name}.ts` (if applicable)
  - Store: `frontend/src/stores/{name}.ts` (Pinia, if applicable)
  - Service/API: `frontend/src/services/{name}.ts` or `frontend/src/api/{name}.ts` (if applicable)
  - Type: `frontend/src/types/{name}.ts` (if applicable)
- **Intention**: Main functionality/purpose of component
- **Request Paths**: Service method + API path mapping table
- **UI Elements**: Vue components and template elements related to functionality

### 2. Backend Analysis (.NET 10 / C#)

- **Source Files** (must be specified):
  - Controller: `backend/Controllers/{Name}Controller.cs`
  - Service: `backend/Services/{Name}Service.cs` (if applicable)
  - DTO: `backend/DTOs/{Name}Dto.cs` (if applicable)
  - Model/Entity: `shared/Models/{Name}.cs`
  - DbContext: `shared/Data/MqttDbContext.cs`
- **API Endpoints**: file:line number
- **DB Schema**: Entity name + properties (from EF Core model)
- **Data Workflow** (complete tracing):
  ```
  Data source (Collector/API) → file:line
      ↓
  Service layer processing → file:line
      ↓
  DbContext access → file:line
      ↓
  Controller response → file:line
      ↓
  Frontend consumption → file:line
  ```

### 3. Collector Analysis (.NET Console App, if applicable)

- **Source Files**:
  - Program: `collector/Program.cs`
  - Service: `collector/Services/{Name}.cs`
- **Data Collection Flow**: How data is fetched, processed, and stored

### 4. Requirement Phases

Each Phase contains requirement description and code changes:

```markdown
### Phase N: Title

**Requirement**: Specific requirement

**Modification Context**:
- **Why change**: [Reason]
- **How to change**: [Method]
- **Impact scope**: [Scope]

**Code Changes**:
[Code block]
```

### 5. Implementation Steps (independent section)

Consolidate all Phase modifications into checkbox list:

```markdown
## Implementation Steps

- [ ] Modify `{file:line}` - {Brief description}
- [ ] Modify `{file:line}` - {Brief description}
```

## Completion Message

```
Impl created at docs/impl/{feature-name}/impl.md
```
