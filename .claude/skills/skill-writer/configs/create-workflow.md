# Workflow: Create

## Output Location

```
.claude/skills/{skill-name}/SKILL.md
.claude/skills/{skill-name}/configs/   (if multiple workflows)
```

## Workflow

### Step 0: Read the Official Format Reference

**Before doing anything else**, read [configs/skill-format-reference.md](skill-format-reference.md) to understand the official SKILL.md format specification, valid frontmatter fields, allowed-tools syntax, and this project's conventions. Do not proceed until you have read it.

### Step 1: Understand What the Skill Should Do

Gather from user context or ask:
- What is the skill's purpose?
- What actions/workflows does it support? (e.g., create/review/update)
- What tools does it need? (Read, Write, Edit, Grep, Glob, Bash, etc.)
- Should it be auto-invocable by Claude or manual-only?

### Step 2: Determine Structure

| Condition | Structure |
|-----------|----------|
| Skill has 1 simple workflow | Everything in SKILL.md |
| Skill has 2+ workflows | SKILL.md (intent detection) + `configs/{action}-workflow.md` per workflow |
| Skill needs reference material | Add `configs/{topic}-reference.md` or `references/{topic}.md` |

### Step 3: Create Directory

```
mkdir -p .claude/skills/{skill-name}/configs
```

### Step 4: Generate SKILL.md

**Required frontmatter:**
```yaml
---
name: {skill-name}
description: {concise description with natural language trigger phrases}
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: {comma-separated list of needed tools}
---
```

**Body structure (for multi-workflow skills):**
```markdown
# {Skill Name} - {Brief Description}

## Intent Detection

Determine which workflow to execute based on the user's natural language:

| User says something like... | Workflow |
|----------------------------|----------|
| "trigger phrase 1", "trigger phrase 2" | [Action Name](configs/action-workflow.md) |

If intent is unclear, ask the user which workflow they need.

## Notes

1. {Important convention or constraint}
```

**Body structure (for single-workflow skills):**
```markdown
# {Skill Name}

## Workflow

{Full workflow instructions here}

## Notes

1. {Important convention or constraint}
```

### Step 5: Create Config Files (if multi-workflow)

For each workflow, create `configs/{action}-workflow.md` with:
- Clear heading: `# Workflow: {Action}`
- Step-by-step instructions
- Expected output format
- Completion message

### Step 6: Verify

- [ ] SKILL.md frontmatter has all required fields
- [ ] Description includes natural language trigger phrases
- [ ] All config file links in intent detection table resolve to real files
- [ ] SKILL.md is under 500 lines
- [ ] No `$ARGUMENTS` or subcommand parsing

## Completion Message

```
Skill created at .claude/skills/{skill-name}/SKILL.md
```
