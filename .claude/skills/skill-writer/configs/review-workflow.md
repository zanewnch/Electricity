# Workflow: Review

## Prerequisites

**Before reviewing**, read [configs/skill-format-reference.md](skill-format-reference.md) to understand the official SKILL.md format specification and this project's conventions. Review against that standard.

## Core Principle

**Fix directly, don't just report issues.**

When problems are found, immediately fix the skill file instead of just listing issues.

## Auto-detect

If user has a skill file open in IDE, automatically use that path. Otherwise, ask which skill to review.

## Review Checklist

### 1. Frontmatter Completeness

| Check Item | Pass Standard |
|----------|---|
| `name` | Present, lowercase with hyphens only |
| `description` | Present, includes natural language trigger phrases |
| `disable-model-invocation` | Present, set to `false` (unless user explicitly wants manual-only) |
| `user-invocable` | Present, set to `true` |
| `model` | Present, set to `opus` |
| `allowed-tools` | Present if skill uses tools, lists only needed tools |

### 2. Description Quality

| Check Item | Pass Standard |
|----------|---|
| Specificity | Describes what the skill does AND when to use it |
| Trigger phrases | Includes natural language phrases users would actually say |
| No "spec" leftovers | No terminology from other projects or outdated naming |

### 3. Structure Check

| Check Item | Pass Standard |
|----------|---|
| Intent detection table | Present if skill has multiple workflows |
| Config file links | All markdown links resolve to existing files |
| Line count | SKILL.md is under 500 lines |
| No $ARGUMENTS | No argument parsing or subcommand routing |
| Notes section | Present with relevant conventions |

### 4. Config Files Check (if exists)

| Check Item | Pass Standard |
|----------|---|
| Each workflow file exists | All files referenced in intent detection table exist |
| Workflow structure | Each file has clear heading, steps, and completion message |
| Consistency | Naming follows `{action}-workflow.md` pattern |
| No orphan files | No config files that aren't referenced from SKILL.md |

### 5. Cross-reference Verification

- Verify all file paths mentioned in workflows actually exist in the project
- Check that tool names in `allowed-tools` are valid Claude Code tools
- Verify the skill name matches the directory name

## Execution Workflow

### Step 1: Review + Auto-fix

When issues found, **directly modify the skill files**:

| Issue Type | Auto-fix Behavior |
|-----------|---|
| Missing frontmatter field | Add with correct default value |
| Broken config file link | Fix path or create missing file |
| Description too vague | Enhance with trigger phrases |
| Over 500 lines | Suggest splitting into configs/ |
| Leftover "spec" references | Replace with correct terminology |

### Step 2: Issues that cannot auto-fix

Only ask user in these cases:
- Unclear what the skill should do (purpose ambiguous)
- Conflicting instructions between SKILL.md and config files

## Completion Message

```
Skill reviewed and corrected.

Corrections applied:
- [Specific correction 1]
- [Specific correction 2]

Skill is ready to use.
```
