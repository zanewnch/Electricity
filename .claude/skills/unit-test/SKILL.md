---
name: unit-test
description: Unit test creation and colored console logging tool. Use when users say "write unit test", "create test file", "add test", "add console.log", "update test", "add color to test", "colored test output", "integrate test".
disable-model-invocation: false
user-invokable: true
---

# Unit Test - Creation and Console Logging Tool

## Intent Detection

| User says something like... | Workflow |
|----------------------------|----------|
| "write unit test", "create test file", "add test", "create test", "generate test" | [Create](configs/create-workflow.md) |
| "update test", "add console.log", "add color to test", "colored test output", "integrate test", "enhance test" | [Update](configs/update-workflow.md) |

If intent is unclear, ask the user which workflow they need.

## Notes

1. **Consistent console.log format** - Keep three-level format consistent (Main Suite → Phase → Test Case)
2. **Correct indentation** - Auto-adjust indentation based on code level
3. **Don't modify test logic** - Only add console.log, don't change existing test code
4. **Preserve existing console.log** - Don't add duplicates if already exists
5. **Phase console.log must be in beforeAll** - Avoid executing on file load

## Reference Documents

- [format-guide.md](configs/format-guide.md) - Three-level console.log format details
- [colors-guide.md](configs/colors-guide.md) - Complete ANSI color usage guide
- [usage-examples.md](configs/usage-examples.md) - Usage examples and scenarios
