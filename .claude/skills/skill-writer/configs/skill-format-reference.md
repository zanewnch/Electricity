# SKILL.md Format Reference

Based on [official Claude Code Skills documentation](https://docs.anthropic.com/en/docs/claude-code/skills).

## Frontmatter Fields

| Field | Type | Required | Default | Description |
|-------|------|----------|---------|-------------|
| `name` | string | No | Directory name | Lowercase, hyphens only, max 64 chars. Becomes the `/slash-command`. |
| `description` | string | Recommended | First paragraph | What the skill does + when to use it. Claude uses this for auto-invocation. |
| `disable-model-invocation` | boolean | No | `false` | `true` = only user can invoke via `/name`. Claude cannot auto-invoke. |
| `user-invokable` | boolean | No | `true` | `false` = hidden from `/` menu. Only Claude can auto-invoke. |
| `argument-hint` | string | No | None | Hint text shown in the `/` menu for argument format. |
| `compatibility` | string | No | None | Compatibility metadata. |
| `license` | string | No | None | License information. |
| `metadata` | string | No | None | Additional metadata. |

**Note:** Only the above fields are supported. Fields like `model`, `allowed-tools`, `context`, and `agent` are NOT valid skill frontmatter.

## This Project's Required Frontmatter

Every skill in this project must include:

```yaml
---
name: {skill-name}
description: {description with natural language trigger phrases}
disable-model-invocation: false
user-invokable: true
---
```

## allowed-tools Syntax

| Pattern | Example | Matches |
|---------|---------|---------|
| Simple tool name | `Read, Grep, Glob` | All uses of those tools |
| Bash with wildcard | `Bash(npm run *)` | `npm run test`, `npm run build`, etc. |
| Bash specific command | `Bash(mkdir:*)` | Any mkdir command |
| Read with path | `Read(/src/**)` | Read anything in `<project>/src/` |
| MCP tools | `mcp__github__*` | All tools from github MCP server |

## Project Conventions

### Directory Structure

**Multi-workflow skills (preferred for complex skills):**
```
.claude/skills/{skill-name}/
├── SKILL.md              # Slim: frontmatter + intent detection + notes
├── configs/
│   ├── {action}-workflow.md      # One file per workflow
│   └── {topic}-reference.md      # Reference material
```

**Simple skills:**
```
.claude/skills/{skill-name}/
└── SKILL.md              # Everything in one file (under 500 lines)
```

### Intent Detection (no $ARGUMENTS)

Use a natural language routing table instead of argument parsing:

```markdown
## Intent Detection

| User says something like... | Workflow |
|----------------------------|----------|
| "trigger phrase 1" | [Action](configs/action-workflow.md) |

If intent is unclear, ask the user which workflow they need.
```

### Naming Conventions

| Item | Convention | Example |
|------|-----------|---------|
| Skill directory | lowercase, hyphens | `skill-writer`, `unit-test` |
| Workflow files | `{action}-workflow.md` | `create-workflow.md` |
| Reference files | `{topic}-reference.md` | `skill-format-reference.md` |
| Output format files | `{topic}-output-format.md` | `impl-output-format.md` |

### Description Best Practices

Include natural language phrases users would actually say:
- "create a {thing}", "help me write a {thing}"
- "review the {thing}", "check the {thing}", "is it correct"
- "update the {thing}", "revise", "it is wrong", "fix the {thing}"

### Key Rules

1. Keep SKILL.md under 500 lines
2. Referenced config files are loaded on-demand (saves tokens)
3. No `$ARGUMENTS`, `argument-hint`, or subcommand parsing
4. Always include `disable-model-invocation: false`, `user-invokable: true`
5. Fix issues directly in review/update workflows (don't just report)
