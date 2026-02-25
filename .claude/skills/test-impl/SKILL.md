---
name: test-impl
description: Unit test implementation spec management. Use when user wants to create a test-impl, write a test spec, plan unit tests, review/check a test-impl, update/revise/fix a test-impl, or says things like "create test-impl", "write test spec", "test implementation spec", "plan tests for", "review test-impl", "update test-impl", "fix the test spec".
disable-model-invocation: false
user-invocable: true
model: opus
---

# Test Impl - Unit Test Implementation Spec Management

## Intent Detection

Determine which workflow to execute based on the user's natural language:

| User says something like... | Workflow |
|----------------------------|----------|
| "create test-impl", "write test spec", "plan tests for", "test implementation spec" | [Create](configs/create-workflow.md) |
| "review test-impl", "check the test spec", "is the test spec correct", "verify test-impl" | [Review](configs/review-workflow.md) |
| "update test-impl", "revise test spec", "test spec is wrong", "fix the test-impl" | [Update](configs/update-workflow.md) |

If intent is unclear, ask the user which workflow they need.

## Output Format Reference

See [configs/test-impl-output-format.md](configs/test-impl-output-format.md) for the standard test-impl output format example.

## Notes

1. **Test cases must map to source code** - Every test case must reference the source `file:line` being tested
2. **file:line verification must execute** - Use Read tool to confirm source code existence
3. **Test phases must be concrete** - Each phase is a logical group of related test cases, not vague descriptions
4. **Follow project test conventions** - xUnit for .NET backend/collector, Vitest + Vue Test Utils for frontend (see `docs/test-plan.md`)
5. **Output co-located with impl** - `test-impl.md` lives alongside `impl.md` in `docs/impl/{feature-name}/`
6. **Shared models live in `shared/`** - Models and DbContext are shared across backend and collector
