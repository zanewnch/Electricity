# Workflow: Create

## Prerequisites

Read the following reference documents before executing:
- [colors-guide.md](colors-guide.md) - ANSI color constants and usage
- [format-guide.md](format-guide.md) - Three-level console.log format

## Input - Dual Mode

### Mode A: Based on Spec Path

User provides a spec folder path. Example:
```
Help me create a unit test based on be/notes/spec/data-correction
```

### Mode B: Based on NLP Description

User describes what to test. Example:
```
Write a unit test: Phase 1 table existence check, Phase 2 data population verification, Phase 3 kWh consistency validation
```

## Input Detection Logic

| Condition | Type | Processing |
|-----------|------|-----------|
| Input contains a directory path | Spec path | Read `spec.md`, extract phases and requirements |
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

## Workflow Steps

### 1. Read Spec / Parse Description

- If spec path: Read `{spec-folder}/spec.md`, extract description from fileoverview and all Requirement Phases
- If NLP: Parse description text, auto-generate phase structure

### 2. Color Constants Definition

Automatically add ANSI color constants at file top:

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

Generate test file with this structure:

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

### Console.log Execution Timing

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
Test file created.

Path: tests/unit/{feature-name}.test.js

Run tests:
npx vitest run tests/unit/{feature-name}.test.js --reporter=verbose
```
