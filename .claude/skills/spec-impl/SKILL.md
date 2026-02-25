---
name: spec-impl
description: Execute spec implementation and review. Supports two subcommands: (1) run - implement based on spec; (2) review - review implementation results against spec. Triggered when user says "implement spec", "execute spec", "review implementation".
argument-hint: <run|review> [spec folder path]
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(npm:*, node:*)
---

# Spec-Impl - Specification Implementation & Review

## Usage

```
/spec-impl run [spec folder path]      → Execute implementation
/spec-impl review [spec folder path]   → Review implementation results (after implement)
```

---

## Parsing $ARGUMENTS

```
$ARGUMENTS format: <subcommand> [spec folder path]

Subcommands:
- run    → Execute "Implement Spec" workflow
- review → Execute "Review Implementation Results" workflow
```

---

# Subcommand 1: run

## Input

```
/spec-impl run [spec folder path]
```

Example: `/spec-impl run D:\GitHub\gateway\be\notes\spec\energy-meter-dashboard`

## Available MCP Tools

| MCP | Tool | Purpose |
|-----|------|---------|
| greptile | `mcp__plugin_greptile_greptile__search_greptile_comments` | Semantic search to understand modification context and confirm impact scope |
| serena | `mcp__plugin_serena_serena__find_symbol`, `mcp__plugin_serena_serena__replace_symbol_body` | Symbol-level editing for precise function modifications |
| git | `Bash(git diff:*)`, `Bash(git status:*)` | Verify modification contents |
| memory | `mcp__memory__search_nodes` | Retrieve previously analyzed spec information |

## Execution Workflow

### Step 1: Read Spec

Read the contents of `{path}/spec.md`.

### Step 2: Build Todo List

Extract all checkbox items from the spec's Implementation Steps:

1. Parse all Phase Implementation Steps
2. Extract each `- [ ]` checkbox item
3. Use TodoWrite to create a tracking list

### Step 3: Implement Step by Step

Execute in Phase order:

**3.1 Start Phase**
1. Mark current todo as `in_progress`
2. Read that Phase's Modification Context

**3.2 Execute Modifications**
1. Extract file:line from checkbox description
2. Use Read tool to read the file
3. Use Edit tool to make modifications
4. Mark todo as `completed`

**3.3 Phase Transition**
1. Confirm all checkboxes in that Phase are completed
2. Start next Phase

### Step 4: Validation

1. Confirm all todos are `completed`
2. Run related tests if any exist

## Completion Prompt

```
Implementation complete ✅

Completed items:
✅ Phase 1: {phase name}
  - ✅ {checkbox 1}
  - ✅ {checkbox 2}

Recommended: run /spec-impl review {path} to review implementation results.
```

---

# Subcommand 2: review

## Input

```
/spec-impl review [spec folder path]
```

## Review Focus (Implementation Results)

### 1. Modification Completeness Check

For each implementation step checkbox:

| Check Item | Verification Method |
|-----------|---------------------|
| Modification exists | Check if new/modified code exists near file:line |
| Modification is correct | Compare against spec's modification context description |
| Logic is complete | Confirm modification handles all necessary cases |

### 2. Functional Validation (if applicable)

- Run related tests
- Confirm new features work correctly
- Confirm existing features are not broken

### 3. Code Quality Check

Reference the project's coding standards:

| Check Item | Reference File |
|-----------|---------------|
| JSDoc comments | `.claude/coding-standards.md` |
| Logger usage | `.claude/logger-standards.md` |
| Error Handling | Confirm appropriate error handling exists |

## Output

`{spec-folder}/impl-review-report.md`

```markdown
# Implementation Review Report

**Spec**: {spec name}
**Review Date**: {date}
**Review Type**: Implementation Result Review (post-implement)

---

## Review Summary

- Modification Completeness: X / Y (XX%)
- Code Quality: ✅ Pass / ⚠️ Needs Improvement
- Overall Status: ✅ Pass / ❌ Needs Correction

---

## Modification Validation Results

### Phase 1: {phase name}

| Checkbox | file:line | Status | Notes |
|----------|-----------|--------|-------|
| {description} | {location} | ✅/⚠️/❌ | {notes} |

---

## Items Requiring Correction

1. ❌ {issue description}

---

## Improvement Suggestions

1. 💡 {suggestion}
```

## Completion Prompt

**Pass:**
```
Implementation review complete ✅

Report saved at {path}/impl-review-report.md

All modifications are correctly completed and meet spec requirements.
```

**Fail:**
```
Implementation review complete ❌

Report saved at {path}/impl-review-report.md

Please fix the issues listed in the report.
```

---

## Notes

1. **run**: Strictly follow the spec, do not add extra modifications on your own
2. **review**: Compare against spec description, use modification context as the reference
3. **Stop on issues**: If you find a problem with the spec, stop and notify the user

---

## Error Handling

### If file:line does not exist

```
⚠️ Error: file:line specified in spec does not exist

File: {file_path}
Line: {line_number}

Suggestions:
1. Run /spec review {path} to re-review the spec
2. Or manually correct the file:line in spec.md
```

$ARGUMENTS
