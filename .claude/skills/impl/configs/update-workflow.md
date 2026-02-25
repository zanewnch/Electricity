# Workflow: Update

**Auto-detect:** If user has impl file open in IDE, automatically use that path. Otherwise, ask the user which impl to update.

## Execution Workflow (fully automatic)

**Important: Don't ask user, directly execute complete formatting process.**

### Step 1: Read Existing Impl

```
Read {impl-folder}/impl.md
```

### Step 2: Analyze Existing Content Structure

Identify existing sections:
- What titles/sections exist?
- Does Index directory exist?
- Does "Related Files" section exist?
- **Is Phase structure complete?** Each Phase should have:
  - `**Requirement**`: Specific requirement
  - `**Modification Context**`: Why/How/Impact
- **Is there independent Implementation Steps section?** Executable checkbox items
- **Determine Impl type:**
  - **Frontend only**: Only Frontend Analysis, no Backend → Don't add Backend section
  - **Backend only**: Only Backend Analysis → Don't add Frontend section
  - **Collector only**: Only Collector Analysis → Don't add other sections
  - **Full-stack**: Multiple exist → Keep complete

**Optional section principle:** Don't force complete large sections (Frontend/Backend/Collector Analysis) that don't exist, but must complete sub-structure of existing sections.

### Step 3: Auto-format (execute all)

#### 3.1 Generate Index Directory

Auto-generate clickable index based on titles:

```markdown
## Index

- [Frontend Analysis](#frontend-analysis)
- [Backend Analysis](#backend-analysis)
- [Requirement Phases](#requirement-phases)
  - [Phase 1: ...](#phase-1-)
- [Related Files](#related-files)
```

#### 3.2 Complete Related Files Section

Extract all file paths from impl content and generate:

```markdown
## Related Files

**Frontend (Vue 3):**
- Component: `frontend/src/components/{Name}.vue`
- Composable: `frontend/src/composables/use{Name}.ts`
- Store: `frontend/src/stores/{name}.ts`

**Backend (.NET):**
- Controller: `backend/Controllers/{Name}Controller.cs`
- Service: `backend/Services/{Name}Service.cs`

**Shared:**
- Model: `shared/Models/{Name}.cs`
- DbContext: `shared/Data/MqttDbContext.cs`

**Collector:**
- Service: `collector/Services/{Name}.cs`
```

#### 3.3 Complete Phase Structure

Check each `### Phase N:` section, complete if missing:

```markdown
### Phase N: Title

**Requirement**: [Infer from existing or mark as TBD]

**Modification Context**:
- **Why change**: [Infer or TBD]
- **How to change**: [Infer or TBD]
- **Impact scope**: [Infer or TBD]

**Code Changes**:
[Preserve existing code block]
```

#### 3.4 Generate Implementation Steps Section

Extract modifications from all Phases, generate independent checkbox list:

```markdown
## Implementation Steps

- [ ] Modify `{file:line}` - {Brief description}
- [ ] Modify `{file:line}` - {Brief description}
```

#### 3.5 Verify file:line References

Use Read tool to verify all `file:line` format references. Auto-correct when correct line found, mark as needs confirmation if not found.

#### 3.6 Format Consistency Check

- [ ] Table format alignment
- [ ] Code block language tags correct
- [ ] file:line format unified (full path:line number)
- [ ] Each Phase has Requirement + Modification Context
- [ ] Implementation Steps section exists and uses checkbox format

### Step 4: Output Updated Impl

**Filename rules:**
- Read `impl.md` → Output overwrites `impl.md`
- Don't create `.bak` backup (use git for version management)

## Completion Output

```
Impl formatting complete.

Change Summary:
- [List of changes applied]

Output file: {path}/impl.md
```
