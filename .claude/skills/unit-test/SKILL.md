---
name: unit-test
description: Unit test creation and colored console logging tool. Supports two subcommands: (1) create - create new test files (based on spec path or NLP description); (2) update - add missing console.log and ANSI colors to existing test files. Triggered when users say "write unit test", "create test file", "add test", "add console.log", "update test", "add color to test", "colored test output", "integrate test".
argument-hint: <create [spec-path-or-description] | update [test-file-path]>
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(mkdir:*)
---

# Unit Test - Creation and Console Logging Tool

## Usage

```
/unit-test create [spec-path or description]  → Create new test file (based on spec or NLP description)
/unit-test update [test-file-path]            → Add missing console.log and colors to existing test file
```

---

## Parsing $ARGUMENTS

```
$ARGUMENTS format: <subcommand> [parameters]

Subcommands:
- create  → Create new test file (based on spec path or NLP description)
- update  → Add console.log and colors to existing test file
```

---

# Subcommand 1: create

## Input - Dual Mode

### Mode A: Based on Spec Path

```
/unit-test create [spec-folder-path]
```

**Example:**
```
/unit-test create be/notes/spec/data-correction
```

### Mode B: Based on NLP Description

```
/unit-test create "[test description]"
```

**Example:**
```
/unit-test create "Phase 1: table existence check, Phase 2: data population verification, Phase 3: kWh consistency validation"
```

## Input Detection Logic

| Condition | Type | Processing |
|-----------|------|-----------|
| Input is directory path | Spec path | Read `spec.md`, extract phases and requirements |
| Input contains description text | NLP description | Parse description text, auto-generate phase structure |

**Logic:**
```
if (input includes '/' or ends with '.md') {
  // Mode A: spec path
  Read spec.md → extract phases
} else {
  // Mode B: NLP description
  Parse description text → generate phase structure
}
```

## Output Location

```
tests/unit/{feature-name}.test.js
```

## Workflow

### 1. Read Spec

- Read `{spec-folder}/spec.md`
- Extract description from fileoverview
- Extract all Requirement Phases (as describe blocks)

### 2. Color Constants Definition

Automatically add ANSI color constants (at file top):

```javascript
// Define ANSI color codes
const colors = {
    purple: '\x1b[95m',      // Main title border
    yellow: '\x1b[93m',      // Tags ([SQL], [Result])
    blue: '\x1b[38;2;156;220;254m',   // Phase marker
    cyan: '\x1b[96m',        // General info
    green: '\x1b[92m',       // Success status
    red: '\x1b[91m',         // Failure/warning
    reset: '\x1b[0m'         // Reset color
};
```

### 3. Generate Test File

Generate test file based on following format (with colors applied):

```javascript
/**
 * @fileoverview [spec description]
 * @see {@link be/notes/spec/{feature-name}/spec.md}
 */

import { describe, it, beforeAll, afterAll, expect } from 'vitest';

// Define ANSI color codes
const colors = {
    purple: '\x1b[95m',
    yellow: '\x1b[93m',
    blue: '\x1b[38;2;156;220;254m',
    cyan: '\x1b[96m',
    green: '\x1b[92m',
    red: '\x1b[91m',
    reset: '\x1b[0m'
};

describe('Overall Test Suite Name', () => {
    // Layer 1: Main Suite (purple border)
    console.log(colors.purple + '\n╔══════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  Overall Test Suite Name             ║' + colors.reset);
    console.log(colors.purple + '╚══════════════════════════════════════╝' + colors.reset);

    beforeAll(async () => {
        // setup
    });

    afterAll(async () => {
        // teardown
    });

    // Phase 1 (blue marker)
    describe('Phase 1: ...', () => {
        // ⚠️ Important: Use beforeAll to ensure output during test execution, not during describe definition
        beforeAll(() => {
            console.log(colors.blue + '\n── [1] Phase 1: ... ──' + colors.reset);
        });

        it('Test item 1', async () => {
            console.log(colors.cyan + '  → Test item 1' + colors.reset);
            // test code
            expect(...).toBe(...);
        });

        it('Test item 2', async () => {
            console.log(colors.cyan + '  → Test item 2' + colors.reset);
            // test code
            expect(...).toBe(...);
        });
    });

    // Phase 2
    describe('Phase 2: ...', () => {
        beforeAll(() => {
            console.log(colors.blue + '\n── [2] Phase 2: ... ──' + colors.reset);
        });

        it('Test item 1', async () => {
            console.log(colors.cyan + '  → Test item 1' + colors.reset);
            // test code with results
            const count = await getCount();
            console.log(colors.yellow + '  [Result]' + colors.reset);
            console.log(`  Count: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
        });
    });
});
```

## Console.log Format Rules

| Level | Color | Format | Example |
|-------|-------|--------|---------|
| **Main Suite** | purple | `console.log(colors.purple + '╔═══════════╗' + colors.reset)` | Bordered title |
| **Phase** | blue | `beforeAll(() => { console.log(...) })` | Block separator (must use beforeAll) |
| **Test Case** | cyan | `console.log(colors.cyan + '  → Test name' + colors.reset)` | Step marker |
| **Tag** | yellow | `console.log(colors.yellow + '  [Result]' + colors.reset)` | [SQL], [Result], [Sample] |
| **Success** | green | `colors.green + 'OK' + colors.reset` | Positive result |
| **Failure** | red | `colors.red + 'ERROR' + colors.reset` | Negative result |
| **General Info** | cyan | `console.log(colors.cyan + '  info...' + colors.reset)` | Additional notes |

### ⚠️ Important: Console.log Execution Timing

```javascript
describe('Phase 1', () => {
    // ❌ Wrong: console.log here executes during describe definition (on file load)
    console.log('Phase 1 started');  // Outputs before all tests start

    // ✅ Correct: Use beforeAll to ensure output before Phase 1 tests execute
    beforeAll(() => {
        console.log('Phase 1 started');  // Outputs before first Phase 1 test
    });

    it('Test 1', () => {
        // ✅ Correct: console.log here outputs during test execution
        console.log('Test 1 running');
    });
});
```

**Principle:** Vitest/Jest executes all `describe()` callbacks immediately on file load, but code inside `beforeAll`/`it` is delayed until actual test execution.

## Completion Message

```
✅ Test file created

📍 Path: tests/unit/{feature-name}.test.js

📌 Run tests:
npx vitest run tests/unit/{feature-name}.test.js --reporter=verbose

💡 Tips:
- Each console.log outputs to test runner logs
- Stages are clearly visible in UI
```

---

# Subcommand 2: update

## Input

```
/unit-test update [test-file-path]
```

**Example:**
```
/unit-test update tests/unit/dataCorrection.test.js
```

## Functionality

Automatically add missing console.log to describe/it blocks in existing test file.

### Checklist

- [ ] Are color constants defined?
- [ ] Does main describe block start with purple border console.log?
- [ ] Do all Phase describe blocks have blue marker console.log wrapped in `beforeAll`?
- [ ] Does each it block start with cyan marker console.log?
- [ ] Do all result/data outputs use correct colors (green/red/yellow)?

### Enhancement Logic

#### 1. Color Constants Definition

Check if colors are defined at file top, add if missing:

```javascript
// ❌ No color constants
import { describe, it, expect } from 'vitest';

describe('Overall name', () => { ... });

// ✅ Enhanced to:
import { describe, it, expect } from 'vitest';

const colors = {
    purple: '\x1b[95m',
    yellow: '\x1b[93m',
    blue: '\x1b[38;2;156;220;254m',
    cyan: '\x1b[96m',
    green: '\x1b[92m',
    red: '\x1b[91m',
    reset: '\x1b[0m'
};

describe('Overall name', () => { ... });
```

#### 2. Main Describe Block

Check if purple border console.log exists, add if missing:

```javascript
describe('Overall name', () => {
    // ❌ Missing or wrong color
    console.log('\n========================================');
    console.log('📍 Overall name');
    console.log('========================================\n');
    it('test', () => { ... });

    // ✅ Enhanced to:
    console.log(colors.purple + '\n╔══════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  Overall name                       ║' + colors.reset);
    console.log(colors.purple + '╚══════════════════════════════════════╝' + colors.reset);

    it('test', () => { ... });
});
```

#### 3. Phase Describe Blocks

Check if Phase describe block has blue marker console.log (must be in `beforeAll`), add if missing:

```javascript
describe('Phase 1: Basic validation', () => {
    // ❌ Wrong: Direct console.log in describe (executes at define time, not test time)
    console.log('\n📍 Phase 1: Basic validation');
    it('test', () => { ... });

    // ✅ Correct: Use beforeAll to ensure output before Phase 1 tests execute
    beforeAll(() => {
        console.log(colors.blue + '\n── [1] Phase 1: Basic validation ──' + colors.reset);
    });

    it('test', () => { ... });
});
```

#### 4. It Blocks

Check if each it block starts with cyan marker console.log, add if missing:

```javascript
it('Verify DB connection', async () => {
    // ❌ No console.log as first line
    const result = await query();
    expect(result).toBe(true);

    // ✅ Enhanced to:
    it('Verify DB connection', async () => {
        console.log(colors.cyan + '  → Verify DB connection' + colors.reset);
        const result = await query();
        expect(result).toBe(true);
    });
});
```

#### 5. Result and Data Output Colors

Check if conditional color output is used (green for success, red for failure), add if missing:

```javascript
// ❌ No color distinction
console.log(`  Count: ${count}`);

// ✅ Enhanced to:
console.log(colors.yellow + '  [Result]' + colors.reset);
console.log(`  Count: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
```

### Check and Enhancement Process

```bash
1. Check if colors constants exist at file top, add if missing
2. Parse AST or Regex to find all describe/it blocks
3. Check each block start for correct color console.log
   - Main describe → purple border (direct console.log)
   - Phase describe → blue marker (must be in beforeAll)
   - It block → cyan marker (direct console.log)
4. Check result/data outputs for conditional colors (green/red/yellow)
5. Add missing console.log and colors (maintain original indentation)
6. Verify all modified code is valid
```

## Completion Message

```
✅ Test file updated

📊 Change Summary:
- Color constants: [Added/Existing]
- Main describe border: +N items
- Phase describe marker: +N items
- Test case marker: +N items
- Result color conditions: +N items

🎨 Color Applications:
- purple (border): Added 1
- blue (Phase): Added N
- cyan (steps): Added N
- yellow (tags): Added N
- green/red (results): Added N

📍 File: tests/unit/xxx.test.js

💡 Recommended to run tests to verify:
npx vitest run tests/unit/xxx.test.js --reporter=verbose
```

---

## Console.log Output Example

### Effect visible in Test Runner UI

```
✓ Data Correction Validation Tests

========================================
📍 Data Correction Validation Tests
========================================

✓ MySQL connection established

📍 Phase 1: Basic connection and table structure validation
  → DB connection successful
  ✓ expected 1 to be 1

  → Contract ID 1 price table exists
  ✓ expected true to be true

📍 Phase 2: Data existence validation
  → Minute price table has data
  📊 cElePResult_1_2025_1: 150 records
  ✓ expected 150 to be greater than 0

📍 Phase 3: kW/kWh consistency validation
  → Verify computekWResult.kW / 60 === cElePResult.kWh
  📊 Validate 10 EM records
  ✓ expected true to be true
```

---

## Notes

1. **Consistent console.log format** - Keep three-level format consistent
2. **Correct indentation** - Auto-adjust indentation based on code level
3. **Don't modify test logic** - Only add console.log, don't change existing test code
4. **Preserve existing console.log** - Don't add duplicates if already exists
5. **⚠️ Phase console.log must be in beforeAll** - Avoid executing on file load

---

## Reference Documents and Guides

**Skill Documentation:**
- [FORMAT-GUIDE.md](references/FORMAT-GUIDE.md) - Three-level console.log format details
- [COLORS-GUIDE.md](references/COLORS-GUIDE.md) - Complete ANSI color usage guide
- [USAGE-EXAMPLES.md](references/USAGE-EXAMPLES.md) - Usage examples and scenarios

**Actual Examples:**
- [dataCorrection.test.js](../../../tests/unit/dataCorrection.test.js) - Basic format example
- [emCsvRepo.test.js](../../../tests/unit/em/emCsvRepo.test.js) - Complete color application example

**External References:**
- [Vitest Reporter Docs](https://vitest.dev/guide/reporters.html)
- [ANSI Color Codes](https://en.wikipedia.org/wiki/ANSI_escape_code)

$ARGUMENTS
