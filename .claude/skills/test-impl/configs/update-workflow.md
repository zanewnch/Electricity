# Workflow: Update

**Auto-detect:** If user has test-impl file open in IDE, automatically use that path. Otherwise, ask the user which test-impl to update.

## Execution Workflow (fully automatic)

**Important: Don't ask user, directly execute complete formatting process.**

### Step 1: Read Existing Test-Impl

```
Read {impl-folder}/test-impl.md
```

### Step 2: Analyze Existing Content Structure

Identify existing sections:
- What titles/sections exist?
- Does Index directory exist?
- Does "Related Files" section exist?
- **Is Phase structure complete?** Each Phase should have:
  - `**Requirement**`: What to validate
  - `**Test Cases**`: Table with test name, input, expected result, source ref
- **Is there independent Implementation Steps section?** Executable checkbox items
- **Determine test layer:**
  - **Backend Unit only**: Only backend service/logic tests → Don't add frontend sections
  - **Backend Integration only**: Only API endpoint tests → Don't add frontend sections
  - **Frontend only**: Only component tests → Don't add backend sections
  - **Collector only**: Only collector tests → Don't add other sections
  - **Multi-layer**: Multiple exist → Keep complete

**Optional section principle:** Don't force complete large sections (Backend/Frontend/Collector) that don't exist, but must complete sub-structure of existing sections.

### Step 3: Auto-format (execute all)

#### 3.1 Generate Index Directory

Auto-generate clickable index based on titles:

```markdown
## Index

- [Source Code Under Test](#source-code-under-test)
- [Test Framework & Setup](#test-framework--setup)
- [Test Phases](#test-phases)
  - [Phase 1: ...](#phase-1-)
- [Implementation Steps](#implementation-steps)
- [Related Files](#related-files)
```

#### 3.2 Complete Related Files Section

Extract all file paths from test-impl content and generate:

```markdown
## Related Files

**Source Files Under Test:**
- Service: `backend/Services/{Name}Service.cs`
- Controller: `backend/Controllers/{Name}Controller.cs`
- Model: `shared/Models/{Name}.cs`
- Component: `frontend/src/components/{Name}.vue`
- Collector: `collector/Services/{Name}.cs`

**Test Files (to create/modify):**
- `tests/backend/UnitTests/{Name}Tests.cs`
- `tests/backend/IntegrationTests/{Name}ControllerTests.cs`
- `tests/frontend/components/{Name}.test.ts`
```

#### 3.3 Complete Phase Structure

Check each `### Phase N:` section, complete if missing:

```markdown
### Phase N: Title

**Requirement**: [Infer from existing or mark as TBD]

**Test Cases**:

| # | Test Name | Input / Precondition | Expected Result | Source Ref |
|---|-----------|---------------------|-----------------|------------|
| 1 | [Infer or TBD] | [Infer or TBD] | [Infer or TBD] | `{file:line}` |

**Setup / Teardown**: [Infer or mark as N/A]
```

#### 3.4 Generate Implementation Steps Section

Extract test items from all Phases, generate independent checkbox list:

```markdown
## Implementation Steps

- [ ] Create `{test-file-path}` - {Brief description}
- [ ] Add test: `{test name}` - Tests `{source-file:line}`
```

#### 3.5 Verify file:line References

Use Read tool to verify all `file:line` format references. Auto-correct when correct line found, mark as needs confirmation if not found.

#### 3.6 Format Consistency Check

- [ ] Table format alignment (Test Cases tables)
- [ ] Code block language tags correct
- [ ] file:line format unified (full path:line number)
- [ ] Each Phase has Requirement + Test Cases table
- [ ] Implementation Steps section exists and uses checkbox format

### Step 4: Output Updated Test-Impl

**Filename rules:**
- Read `test-impl.md` → Output overwrites `test-impl.md`
- Don't create `.bak` backup (use git for version management)

## Completion Output

```
Test-impl formatting complete.

Change Summary:
- [List of changes applied]

Output file: {path}/test-impl.md
```
