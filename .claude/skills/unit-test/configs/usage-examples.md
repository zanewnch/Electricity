# Unit Test Skill - Usage Examples

## Dual-trigger Mechanism

You can trigger this skill using **Natural Language** or **Manual Commands** in two ways.

---

## Method 1️⃣: NLP Trigger (Auto-recognition)

### Example 1: Create new test file

```
I need to write a unit test for data-correction
```

**Claude will auto-recognize this is a create operation and ask for spec path**

---

### Example 2: Add console.log

```
Help me add console.log and colors to existing test file
```

**Claude will auto-recognize this is an update operation and ask for test file path**

---

### Example 3: Colored output

```
I want emCsvRepo.test.js to have colored console output
```

**Claude will auto-recognize this is an update operation**

---

### Example 4: Integrate test

```
Help me integrate this test with console.log and colors
```

**Claude will auto-trigger update**

---

### ✅ Keywords for NLP trigger

```
write unit test
create test file
add test
add console.log
enhance test
add color to test
colored test output
integrate test
```

Any natural language containing these keywords will auto-trigger the skill.

---

## Method 2️⃣: Manual Commands (Argument)

### Create Subcommand

Create new test file (based on spec):

```
/unit-test create be/notes/spec/data-correction
```

**Execution flow:**
1. Read `be/notes/spec/data-correction/spec.md`
2. Extract description, phases, requirements
3. Generate `tests/unit/dataCorrection.test.js`
4. Automatically add console.log + ANSI colors

---

### Update Subcommand

Enhance existing test file:

```
/unit-test update tests/unit/emCsvRepo.test.js
```

**Execution flow:**
1. Read `tests/unit/emCsvRepo.test.js`
2. Check for missing:
   - Color constants definition
   - Main describe border
   - Phase describe markers
   - It block markers
   - Result color conditions
3. Add all missing console.log + colors
4. Verify syntax correctness

---

## Scenario Mapping Table

| Scenario | NLP | Manual Command |
|----------|-----|---|
| **Create test** | "Help me write unit test" | `/unit-test create path` |
| **Add logs** | "Add console.log" | `/unit-test update path` |
| **Add colors** | "Add colors to test" | `/unit-test update path` |
| **Full integration** | "Integrate this test" | `/unit-test update path` |

---

## Real-world Examples

### Scenario 1: Just finished spec, want to auto-generate test file

**You can say:**
```
Based on be/notes/spec/data-correction spec, help me create test file
```

Or use direct command:
```
/unit-test create be/notes/spec/data-correction
```

**Result:**
- Auto-generates `tests/unit/dataCorrection.test.js`
- Includes describe blocks for each phase
- Each block has formatted console.log + ANSI colors

---

### Scenario 2: Have old test file, want to add console.log and colors

**You can say:**
```
My emCsvRepo.test.js already has colors defined, help me add missing console.log
```

Or use direct command:
```
/unit-test update tests/unit/em/emCsvRepo.test.js
```

**Result:**
- Checks for color constants (preserve if exists, add if missing)
- Adds missing console.log (Main, Phase, It blocks)
- Adds result color conditions (green/red)
- Doesn't modify existing correct console.log

---

### Scenario 3: Existing test needs overall improvement

**You can say:**
```
Help me integrate dataCorrection.test.js with complete console.log format and colored output
```

Or use command:
```
/unit-test update tests/unit/dataCorrection.test.js
```

---

## Mixed Usage

You can also start with NLP, and Claude will confirm path before executing:

```
User: I need to add colors to this test
     ↓
Claude: Which test file should I add colors to? (lists or prompts for path)
     ↓
User: tests/unit/emCsvRepo.test.js
     ↓
Claude: Running /unit-test update tests/unit/em/emCsvRepo.test.js
     ↓
Result: ✅ Completed
```

---

## Best Practices

### ✅ When to use NLP

- You're unsure of exact paths
- You want Claude to analyze your needs
- You want more interaction and confirmation

### ✅ When to use manual commands

- You already know exact paths
- You want quick execution without confirmation
- You prefer CLI-style commands

---

## Advanced Usage

### Process multiple test files at once

```
I want to add colored output to all test files under tests/unit
```

Claude will:
1. Scan `tests/unit` directory
2. List all `.test.js` files
3. Execute update for each
4. Generate complete change report

---

## FAQ

**Q: Will NLP auto-execute?**
A: No, Claude will recognize your needs, ask for path/parameter confirmation, then execute.

**Q: Can I mix NLP and commands?**
A: Yes. You can describe needs with NLP first, then specify exact path with command.

**Q: What's the difference between create and update?**
A:
- **Create**: Generate brand new test file from spec from scratch
- **Update**: Improve existing test file, add missing console.log and colors

**Q: Will update duplicate if test file already has console.log?**
A: No. Update intelligently detects and only adds missing parts.

