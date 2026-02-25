# Workflow: Update

## Prerequisites

Read the following reference documents before executing:
- [colors-guide.md](colors-guide.md) - ANSI color constants and usage
- [format-guide.md](format-guide.md) - Three-level console.log format

## Input

User provides an existing test file to enhance. Example:
```
Add console.log and colors to tests/unit/dataCorrection.test.js
```

If the user has a file open in IDE, automatically use that path. Otherwise, ask which test file to update.

## Functionality

Automatically add missing console.log to describe/it blocks in existing test file.

## Checklist

- [ ] Are color constants defined?
- [ ] Does main describe block start with purple border console.log?
- [ ] Do all Phase describe blocks have blue marker console.log wrapped in `beforeAll`?
- [ ] Does each it block start with cyan marker console.log?
- [ ] Do all result/data outputs use correct colors (green/red/yellow)?

## Enhancement Logic

### 1. Color Constants Definition

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

### 2. Main Describe Block

Check if purple border console.log exists, add if missing:

```javascript
describe('Overall name', () => {
    // ❌ Missing or wrong color
    console.log('\n========================================');
    console.log('Overall name');
    console.log('========================================\n');
    it('test', () => { ... });

    // ✅ Enhanced to:
    console.log(colors.purple + '\n╔══════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  Overall name                       ║' + colors.reset);
    console.log(colors.purple + '╚══════════════════════════════════════╝' + colors.reset);

    it('test', () => { ... });
});
```

### 3. Phase Describe Blocks

Check if Phase describe block has blue marker console.log (must be in `beforeAll`), add if missing:

```javascript
describe('Phase 1: Basic validation', () => {
    // ❌ Wrong: Direct console.log in describe (executes at define time, not test time)
    console.log('\nPhase 1: Basic validation');
    it('test', () => { ... });

    // ✅ Correct: Use beforeAll to ensure output before Phase 1 tests execute
    beforeAll(() => {
        console.log(colors.blue + '\n── [1] Phase 1: Basic validation ──' + colors.reset);
    });

    it('test', () => { ... });
});
```

### 4. It Blocks

Check if each it block starts with cyan marker console.log, add if missing:

```javascript
// ❌ No console.log as first line
it('Verify DB connection', async () => {
    const result = await query();
    expect(result).toBe(true);
});

// ✅ Enhanced to:
it('Verify DB connection', async () => {
    console.log(colors.cyan + '  → Verify DB connection' + colors.reset);
    const result = await query();
    expect(result).toBe(true);
});
```

### 5. Result and Data Output Colors

Check if conditional color output is used (green for success, red for failure), add if missing:

```javascript
// ❌ No color distinction
console.log(`  Count: ${count}`);

// ✅ Enhanced to:
console.log(colors.yellow + '  [Result]' + colors.reset);
console.log(`  Count: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
```

## Check and Enhancement Process

```
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
Test file updated.

Change Summary:
- Color constants: [Added/Existing]
- Main describe border: +N items
- Phase describe marker: +N items
- Test case marker: +N items
- Result color conditions: +N items

File: tests/unit/xxx.test.js

Run tests to verify:
npx vitest run tests/unit/xxx.test.js --reporter=verbose
```
