# Workflow: Review

## Core Principle

**Fix directly, don't just report issues.**

When problems are found, immediately fix the impl file instead of just listing issues and asking user confirmation.

## Review Focus (impl content itself)

### 1. Format Completeness

| Check Item | Pass Standard |
|----------|---|
| Index directory | Exists and synced with titles |
| Related files section | Exists and lists all involved files |

### 2. Analysis Completeness (based on impl type)

**After determining impl type, only check existing sections:**

| Impl Type | Check Item |
|-----------|---|
| Frontend only | Frontend Analysis complete (intention, UI elements) |
| Backend only | Backend Analysis complete (API endpoints, DB schema, Data Workflow) |
| Collector only | Collector Analysis complete (data collection flow) |
| Full-stack | Check all applicable sections |

**Note:** Missing sections don't fail, but existing sections must be complete.

### 3. file:line Verification

- Use Read tool to check each file:line actually exists
- Confirm code at that line matches description
- Mark as wrong if not found

### 4. Phase Structure Completeness

| Check Item | Pass Standard |
|-----------|---|
| Requirement | Each Phase has `**Requirement**` |
| Modification Context | Each Phase has why/how/impact |
| Code Changes | Has concrete code block (if applicable) |

### 5. Implementation Steps Executability

| Check Item | Pass Standard |
|-----------|---|
| Independent section | `## Implementation Steps` section exists |
| Checkbox format | Uses `- [ ]` format |
| file:line reference | Each checkbox has explicit file:line |
| Clear description | Concrete and executable (not vague) |

## Execution Workflow

### Step 1: Review + Auto-fix

When issues found, **directly modify impl.md**:

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
Impl reviewed and corrected.

Corrections applied:
- [Specific correction 1]
- [Specific correction 2]

Can start implementation based on the reviewed impl.
```
