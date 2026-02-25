---
name: impl
description: Implementation document management. Use when user wants to create an impl, review/check an impl, update/revise/fix an impl, or says things like "create impl", "help me create the impl for", "analyze feature", "review impl", "it is wrong help me revise", "I think it is better for xxx", "help me change the impl".
disable-model-invocation: false
user-invocable: true
model: opus
---

# Impl - Implementation Specification Document Management

## Intent Detection

Determine which workflow to execute based on the user's natural language:

| User says something like... | Workflow |
|----------------------------|----------|
| "create impl", "analyze feature", "make an impl for" | [Create](configs/create-workflow.md) |
| "review impl", "check the impl", "is it correct", "verify" | [Review](configs/review-workflow.md) |
| "update impl", "revise", "it is wrong", "change", "fix the impl", "I think it is better for xxx" | [Update](configs/update-workflow.md) |

If intent is unclear, ask the user which workflow they need.

## Output Format Reference

See [configs/impl-output-format.md](configs/impl-output-format.md) for the standard impl output format example.

## Notes

1. **Data workflow must be complete** - No gaps, each step must have file:line
2. **file:line verification must execute** - Use Read tool to confirm existence
3. **Implementation steps must be concrete** - Not vague, executable items
4. **Follow project conventions** - Vue 3 Composition API, .NET PascalCase naming
5. **Shared models live in `shared/`** - Models and DbContext are shared across backend and collector
