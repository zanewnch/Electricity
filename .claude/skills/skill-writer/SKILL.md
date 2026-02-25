---
name: skill-writer
description: Skill file management. Use when user wants to create a new skill, review/check an existing skill, update/revise/fix a skill file, or says things like "create a skill", "help me write a skill", "review the skill", "check the skill file", "update the skill", "the skill is wrong", "fix the skill file".
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(mkdir:*)
---

# Skill Writer - Skill File Management

## IMPORTANT: Read Official Documentation First

**Before executing ANY workflow**, you MUST read [configs/skill-format-reference.md](configs/skill-format-reference.md) first. This contains the official SKILL.md format specification, valid frontmatter fields, allowed-tools syntax, and this project's conventions. Do not proceed with any action until you have read it.

## Intent Detection

Determine which workflow to execute based on the user's natural language:

| User says something like... | Workflow |
|----------------------------|----------|
| "create a skill", "write a skill", "make a new skill", "help me build a skill" | [Create](configs/create-workflow.md) |
| "review the skill", "check the skill file", "is the skill correct", "verify skill" | [Review](configs/review-workflow.md) |
| "update the skill", "revise the skill", "the skill is wrong", "fix the skill file", "change the skill" | [Update](configs/update-workflow.md) |

If intent is unclear, ask the user which workflow they need.
