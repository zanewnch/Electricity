---
name: spec
description: Specification document management. Supports three subcommands: (1) create - analyze frontend/backend and create spec file; (2) review - verify spec completeness (before implementation); (3) update - modify spec content while maintaining format consistency. Triggered when users say "create spec", "analyze feature", "review spec", "help me create spec", "help me review spec", "help me change spec", "modify spec", "update spec".
argument-hint: <create|review|update> [parameters]
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(mkdir:*)
---

# Spec - Specification Document Management

## Usage

```
/spec create [component/feature name]   → Create spec file
/spec review [spec folder path]         → Review spec (before implementation)
/spec update [spec folder path]         → Modify spec content
```

---

## Parsing $ARGUMENTS

```
$ARGUMENTS format: <subcommand> [parameters...]

Subcommands:
- create → Execute "Create Spec" workflow
- review → Execute "Review Spec" workflow
- update → Execute "Modify Spec" workflow
```

---

# Subcommand 1: create

## Input

```
/spec create [component name or feature description]
```

## Output Location

```
D:\GitHub\gateway\be\notes\spec\{feature-name}\spec.md
```

## Analysis Workflow

Use deep analysis to ensure every step of data workflow is correctly connected.

### Available MCP Tools

Prioritize using following MCP tools to accelerate analysis:

| MCP | Tool | Purpose |
|-----|------|---------|
| greptile | `mcp__plugin_greptile_greptile__search_greptile_comments` | Semantic codebase search, find data workflow paths |
| serena | `mcp__plugin_serena_serena__find_symbol`, `mcp__plugin_serena_serena__find_referencing_symbols` | Symbol-level code analysis, track function calls |
| memory | `mcp__memory__create_entities`, `mcp__memory__search_nodes` | Remember analysis results for later use |
| git | `Bash(git blame:*)`, `Bash(git log:*)` | View code history and change records |

### 1. Frontend Analysis

- **Source Files** (must be specified):
  - Component: `{path}/xxx.component.ts`
  - Service: `{path}/xxx.service.ts`
  - Model: `{path}/xxx.model.ts` (if applicable)
- **Intention**: Main functionality/purpose of component
- **Request Paths**: Service method + API path mapping table
- **UI Elements**: UI components related to functionality

### 2. Backend Analysis

- **Source Files** (must be specified):
  - Route: `gateway-server/routes/xxx.js`
  - Handler: `gateway-server/model/xxx.js` (if applicable)
  - Middleware: `gateway-server/model/xxx.js` (if applicable)
- **API Endpoints**: file:line number
- **DB Schema**: table name + columns
- **Data Workflow** (complete tracing):
  ```
  MQTT data retrieval → file:line
      ↓
  Dispatch processing → file:line
      ↓
  Cache access → file:line
      ↓
  Storage calculation → file:line
      ↓
  IPC return → file:line
      ↓
  Handler processing → file:line
      ↓
  DB insert → file:line
  ```

### 3. Requirement Phases

Each Phase contains requirement description and code changes:

```markdown
### 🟣 Phase N: Title

**Requirement**: Specific requirement

**Modification Context**:
- **Why change**: [Reason]
- **How to change**: [Method]
- **Impact scope**: [Scope]

**Code Changes**:
[Code block]
```

### 4. Implementation Steps (independent section)

Consolidate all Phase modifications into checkbox list:

```markdown
## 🟣 Implementation Steps

- [ ] Modify `{file:line}` - {Brief description}
- [ ] Modify `{file:line}` - {Brief description}
```

## Completion Message

```
Spec created at {path}/spec.md

Recommended: Run /spec review {path} to review spec content.
```

---

# Subcommand 2: review

## Input

```
/spec review [spec folder path]
```

## Core Principle

**Fix directly, don't just report issues.**

When problems are found, immediately fix the spec file instead of just listing issues and asking user confirmation.

## Review Focus (spec content itself)

### 1. Format Completeness

| Check Item | Pass Standard |
|----------|---|
| Index directory | Exists and synced with titles |
| Title Emoji | All main blocks have correct Emoji (🔵🟢🟣🟡🟠📄) |
| Related files section | Exists and lists all involved files |

### 2. Analysis Completeness (based on spec type)

**After determining spec type, only check existing sections:**

| Spec Type | Check Item |
|-----------|---|
| Frontend only | Frontend Analysis complete (intention, UI elements) |
| Backend only | Backend Analysis complete (API endpoints, DB schema, Data Workflow) |
| Full-stack | Check both |

**Note:** Missing sections don't fail, but existing sections must be complete.

### 3. file:line Verification

- Use Read tool to check each file:line actually exists
- Confirm code at that line matches description
- Mark as ❌ if not found

### 4. Phase Structure Completeness

| Check Item | Pass Standard |
|-----------|---|
| Requirement | Each Phase has `**Requirement**` |
| Modification Context | Each Phase has why/how/impact |
| Code Changes | Has concrete code block (if applicable) |

### 5. Implementation Steps Executability

| Check Item | Pass Standard |
|-----------|---|
| Independent section | `## 🟣 Implementation Steps` section exists |
| Checkbox format | Uses `- [ ]` format |
| file:line reference | Each checkbox has explicit file:line |
| Clear description | Concrete and executable (not vague) |

## Execution Workflow

### Step 1: Review + Auto-fix

When issues found, **directly modify spec.md**:

| Issue Type | Auto-fix Behavior |
|-----------|---|
| Wrong line numbers | Use Read tool to find correct line, update directly |
| Inconsistent naming | Fix per project conventions |
| Missing sections | Complete missing structure |
| Format issues | Format directly |

### Step 2: Issues that cannot auto-fix

Only ask user in these cases:
- Logic contradictions (requirement unclear)
- Cannot find corresponding code (requirement may be outdated)

## Completion Message

```
Spec reviewed and corrected ✅

Corrections applied:
- [Specific correction 1]
- [Specific correction 2]

Can run /spec-impl run {path} to start implementation.
```

---

# Subcommand 3: update

## Input

```
/spec update [spec folder path]
```

**Path examples:**
- `be/notes/spec/data-correction`
- `be/notes/spec/energy-meter-setting`

**Auto-detect:** If user has spec file open in IDE, automatically use that path.

## Execution Workflow (fully automatic)

**Important: Don't ask user, directly execute complete formatting process.**

### Step 1: Read Existing Spec

```
Read {spec-folder}/spec.md or {spec-folder}/spec_v*.md
```

### Step 2: Analyze Existing Content Structure

Identify existing sections:
- What titles/sections exist?
- Which sections are missing Emoji?
- Does Index directory exist?
- Does "Related Files" section exist?
- **Is Phase structure complete?** Each Phase should have:
  - `**Requirement**`: Specific requirement
  - `**Modification Context**`: Why/How/Impact
- **Is there independent Implementation Steps section?** Executable checkbox items
- **Determine Spec type:**
  - **Frontend only**: Only Frontend Analysis, no Backend → Don't add Backend section
  - **Backend only**: Only Backend Analysis → Don't add Frontend section
  - **Full-stack**: Both exist → Keep complete

**Optional section principle:** Don't force complete large sections (Frontend/Backend Analysis) that don't exist, but must complete sub-structure of existing sections.

### Step 3: Auto-format (execute all)

#### 3.1 Convert Title Format

Convert all titles to Emoji format:

| Keyword Match | Emoji | Result |
|---|---|---|
| Frontend / Validation Rules / FormControl | 🔵 | `## 🔵 Frontend Analysis` |
| Backend | 🟢 | `## 🟢 Backend Analysis` |
| Requirement / Phase | 🟣 | `## 🟣 Requirement Phases` |
| Implementation | 🟣 | `## 🟣 Implementation Steps` |
| Effect / Result | 🟡 | `## 🟡 Effect` |
| Data Structure / Schema | 🟡 | `## 🟡 ...` |
| Test | 🟠 | `## 🟠 Test` |
| Related Files | 📄 | `## 📄 Related Files` |

**Subtitle rule:** Use same color as parent title

```markdown
## 🟣 Requirement Phases
### 🟣 Phase 1: Name length limit    ← Same color
### 🟣 Implementation Steps          ← Same color
```

#### 3.2 Generate Index Directory

Auto-generate clickable index based on converted titles:

```markdown
## 📑 Index

- [🔵 Frontend Analysis](#-frontend-analysis)
- [🟢 Backend Analysis](#-backend-analysis)
- [🟣 Requirement Phases](#-requirement-phases)
  - [Phase 1: Name length limit](#phase-1-name-length-limit)
- [📄 Related Files](#-related-files)
```

#### 3.3 Complete Related Files Section

Extract all file paths from spec content and generate:

```markdown
## 📄 Related Files

**Frontend:**
- Component: `{extracted .component.ts path}`
- Template: `{extracted .component.html path}`

**Backend:**
- Route: `{extracted routes/*.js path}`

**i18n:**
- `gateway-server-frontend/src/assets/i18n/*.json`
```

#### 3.4 Complete Phase Structure

Check each `### Phase N:` section, complete if missing:

```markdown
### 🟣 Phase N: Title

**Requirement**: [Infer from existing or mark as TBD]

**Modification Context**:
- **Why change**: [Infer or TBD]
- **How to change**: [Infer or TBD]
- **Impact scope**: [Infer or TBD]

**Code Changes**:
[Preserve existing code block]
```

#### 3.5 Generate Implementation Steps Section

Extract modifications from all Phases, generate independent checkbox list:

```markdown
## 🟣 Implementation Steps

- [ ] Modify `{file:line}` - {Brief description}
- [ ] Modify `{file:line}` - {Brief description}
```

**Extraction rules:**
- Extract file:line from `**File**:` or code comments
- Extract description from Phase title or Requirement
- One checkbox per modification point

#### 3.6 Verify file:line References

Use Read tool to verify all `file:line` format references:

```
🔍 Verifying file:line references...

✅ equipment-group-setting.component.ts:54
✅ equipment-group-setting.component.html:26
❌ equipment-group-setting.component.ts:999 (line number out of range)
```

**Verification failure handling:**
- Attempt to search for relevant code in file
- Auto-correct when correct line found
- Mark as `❓ Needs confirmation` if not found

#### 3.7 Format Consistency Check

- [ ] Table format alignment
- [ ] Code block language tags correct
- [ ] file:line format unified (full path:line number)
- [ ] Each Phase has Requirement + Modification Context
- [ ] Implementation Steps section exists and uses checkbox format

### Step 4: Output Updated Spec

**Filename rules:**
- Read `spec.md` → Output overwrites `spec.md`
- Read `spec_v2.md` → Output overwrites `spec_v2.md` (maintain original filename)
- Don't create `.bak` backup (use git for version management)

## Completion Output

```
Spec formatting complete ✅

📝 Change Summary:
- Added Index directory
- Added Emoji to titles (🔵🟢🟣📄)
- Completed Phase structure (Requirement + Modification Context)
- Generated Implementation Steps section (N checkboxes)
- Completed Related Files section
- Verified N file:line references (N ✅ / N ❌)

📄 Output file: {path}/spec.md

Recommended: Run /spec review {path} to verify completeness.
```

---

## Output Format Example

Reference [references/spec-output-format.md](references/spec-output-format.md)

---

## Notes

1. **Data workflow must be complete** - No gaps, each step must have file:line
2. **file:line verification must execute** - Use Read tool to confirm existence
3. **Implementation steps must be concrete** - Not vague, executable items

---

## Spec Format Standards

### 1. 📑 Index Directory (required)

Spec beginning must contain clickable index:

```markdown
## 📑 Index

- [🔵 Frontend Analysis](#-frontend-analysis)
  - [Source Files](#-source-files)
  - [Intention](#-intention)
  - [Request Paths](#-request-paths)
  - [UI Elements](#-ui-elements)
- [🟢 Backend Analysis](#-backend-analysis)
  - [Source Files](#-source-files-1)
  - [API Endpoints](#-api-endpoints)
  - [DB Schema](#-db-schema)
  - [Data Workflow](#-data-workflow)
- [🟣 Requirement Phases](#-requirement-phases)
- [📄 Related Files](#-related-files)
```

### 2. Title Color Emoji (required)

Use solid color circles to distinguish sections and improve readability:

| Section | Emoji | Example |
|---------|-------|---------|
| Frontend / Validation Rules | 🔵 | `## 🔵 Frontend Analysis` |
| Backend | 🟢 | `## 🟢 Backend Analysis` |
| Effect / Data Structure | 🟡 | `## 🟡 Effect` |
| Requirement / Implementation | 🟣 | `## 🟣 Requirement Phases` |
| Test | 🟠 | `## 🟠 Test` |
| Related Files | 📄 | `## 📄 Related Files` |

**Subtitle rule**: Use same color as parent title

```markdown
## 🟢 Backend Analysis
### 🟢 API Endpoints      ← Same color
### 🟢 DB Schema          ← Same color
### 🟢 Data Workflow      ← Same color
```

**Available solid circles**: 🔴 🟠 🟡 🟢 🔵 🟣 🟤 ⚫ ⚪

### 3. 📄 Related Files Section (required)

Every spec must end with Related Files section listing all involved file paths:

```markdown
## 📄 Related Files

**Frontend:**
- Component: `gateway-server-frontend/src/app/{module}/{component}.component.ts`
- Service: `gateway-server-frontend/src/app/{module}/@services/{service}.service.ts`
- Model: `gateway-server-frontend/src/app/{module}/@models/{model}.model.ts`
- Template: `gateway-server-frontend/src/app/{module}/{component}.component.html`

**Backend:**
- Route: `gateway-server/routes/{route}.js`
- Handler: `gateway-server/model/{handler}.js`
- Middleware: `gateway-server/model/{middleware}.js`
```

**Purpose:**
- ✅ Helps subsequent readers quickly locate code
- ✅ Convenient IDE click navigation
- ✅ Clearly defines modification scope

$ARGUMENTS
