# Workflow: Review

## Core Principle

**Fix directly, don't just report issues.**

When problems are found, immediately fix the test-impl file instead of just listing issues and asking user confirmation.

## Review Focus (test-impl content itself)

### 1. Format Completeness

| Check Item | Pass Standard |
|----------|---|
| Index directory | Exists and synced with titles |
| Related Files section | Exists and lists all source files + test file paths |
| Overview section | Has feature name, test layer, framework, date |

### 2. Test Coverage Completeness (based on test layer)

**After determining test layer, only check existing sections:**

| Test Layer | Check Item |
|-----------|---|
| Backend Unit | Service methods covered, edge cases identified, value ranges tested |
| Backend Integration | API endpoints covered, happy path + error cases, response codes tested |
| Frontend Component | Render behavior, conditional display, user interactions covered |
| Collector | Data generation ranges, value constraints, null handling covered |

**Note:** Missing layers don't fail, but existing layers must have complete coverage.

### 3. Test Case Quality

| Check Item | Pass Standard |
|-----------|---|
| Test name | Descriptive, follows "should {behavior} when {condition}" or similar |
| Input / Precondition | Specific values, not vague descriptions |
| Expected Result | Concrete assertion (exact value, error type, behavior) |
| Source Ref | Points to actual source code line being tested |

### 4. file:line Verification

- Use Read tool to check each `file:line` actually exists
- Confirm code at that line matches the method/logic being tested
- Mark as wrong if not found

### 5. Phase Structure Completeness

| Check Item | Pass Standard |
|-----------|---|
| Requirement | Each Phase has `**Requirement**` |
| Test Cases table | Each Phase has table with #, Test Name, Input, Expected Result, Source Ref |
| Setup / Teardown | Noted if the phase requires it |

### 6. Implementation Steps Executability

| Check Item | Pass Standard |
|-----------|---|
| Independent section | `## Implementation Steps` section exists |
| Checkbox format | Uses `- [ ]` format |
| Test file paths | Each checkbox specifies test file location |
| Clear description | Concrete and executable (not vague) |

## Execution Workflow

### Step 1: Review + Auto-fix

When issues found, **directly modify test-impl.md**:

| Issue Type | Auto-fix Behavior |
|-----------|---|
| Wrong line numbers | Use Read tool to find correct line, update directly |
| Missing edge case tests | Add obvious edge cases (null, empty, boundary) |
| Incomplete Test Cases table | Fill in missing columns |
| Format issues | Format directly |

### Step 2: Issues that cannot auto-fix

Only ask user in these cases:
- Unclear what behavior to test (requirement ambiguous)
- Cannot find corresponding source code (code may have been refactored)
- Test scope disagreement (what to include/exclude)

## Completion Message

```
Test-impl reviewed and corrected.

Corrections applied:
- [Specific correction 1]
- [Specific correction 2]

Can start writing tests based on the reviewed test-impl.
```
