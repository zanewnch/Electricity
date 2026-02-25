# Workflow: Update

**Auto-detect:** If user has a skill file open in IDE, automatically use that path. Otherwise, ask which skill to update.

## Prerequisites

**Before making changes**, read [configs/skill-format-reference.md](skill-format-reference.md) to understand the official SKILL.md format specification and this project's conventions. Ensure all changes conform to that standard.

## Execution Workflow (fully automatic)

**Important: Don't ask user for confirmation, directly execute changes.**

### Step 1: Read Existing Skill

Read the SKILL.md and all referenced config files to understand the current structure.

### Step 2: Understand the Change

From user's natural language, determine what needs to change:
- Adding a new workflow → Create new config file + update intent detection table
- Changing description/triggers → Update frontmatter description
- Fixing incorrect content → Edit the specific file
- Restructuring → Move content between SKILL.md and configs/
- Removing a workflow → Delete config file + update intent detection table

### Step 3: Apply Changes

**Maintain consistency with project conventions:**
- Keep SKILL.md as the slim routing file (intent detection + references + notes)
- Workflow details go in `configs/{action}-workflow.md`
- All config file links must resolve after changes
- Frontmatter must keep required fields: `disable-model-invocation: false`, `user-invocable: true`, `model: opus`

### Step 4: Verify After Changes

- [ ] All markdown links in SKILL.md resolve to real files
- [ ] No orphan config files (unreferenced from SKILL.md)
- [ ] Frontmatter still has all required fields
- [ ] Description still includes relevant trigger phrases
- [ ] SKILL.md still under 500 lines
- [ ] Intent detection table matches available config files

## Completion Message

```
Skill updated.

Changes applied:
- [List of changes]

Updated file(s): {paths}
```
