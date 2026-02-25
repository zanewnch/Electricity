# Unit Test Console.log Format Guide

## Design Philosophy

**Goal:** Clearly see progress and intent of each test in Test Runner UI

### Three-Level Console.log Structure

```
Layer 1: Main Suite (separator line style)
  ↓
Layer 2: Phases (🔵 marker)
  ↓
Layer 3: Test Cases (→ marker)
```

---

## Level 1️⃣ - Main Suite

**Location:** Start of describe block

**Format:**
```javascript
describe('Data Correction Validation Tests', () => {
    console.log('\n========================================');
    console.log('📍 Data Correction Validation Tests');
    console.log('========================================\n');

    // Other describe blocks...
});
```

**Output effect:**
```
========================================
📍 Data Correction Validation Tests
========================================
```

**Purpose:** Clearly mark the beginning of entire test suite with separator lines

---

## Level 2️⃣ - Phase Blocks

**Location:** Start of each Phase describe block

**Format:**
```javascript
describe('Phase 1: Basic Connection and Table Structure Validation', () => {
    console.log('\n📍 Phase 1: Basic Connection and Table Structure Validation');

    it('test case 1', () => { ... });
    it('test case 2', () => { ... });
});

describe('Phase 2: Data Existence Validation', () => {
    console.log('\n📍 Phase 2: Data Existence Validation');

    it('test case 1', () => { ... });
});
```

**Output effect:**
```
📍 Phase 1: Basic Connection and Table Structure Validation
  → test case 1
  → test case 2

📍 Phase 2: Data Existence Validation
  → test case 1
```

**Purpose:** Distinguish different phases, visually segment test cases

---

## Level 3️⃣ - Test Cases (It Blocks)

**Location:** Inside each it block, as first line of logic

**Format:**
```javascript
it('Verify DB connection success', async () => {
    console.log('  → Verify DB connection success');

    const [result] = await connection.query('SELECT 1 as test');
    expect(result[0].test).toBe(1);
});

it('Minute price table has data', async () => {
    console.log('  → Minute price table has data');

    const count = await getRowCount(connection, tableName);
    console.log(`  📊 ${tableName}: ${count} records`);
    expect(count).toBeGreaterThan(0);
});
```

**Output effect:**
```
  → Verify DB connection success
  ✓

  → Minute price table has data
  📊 cElePResult_1_2025_1: 150 records
  ✓
```

**Purpose:** Each test case at a glance, indented to distinguish from phases

---

## Indentation Rules

| Level | Indentation | Example |
|-------|-------------|---------|
| Main Suite | None | `console.log('========================================')` |
| Phase | None | `console.log('\n📍 Phase 1: ...')` |
| Test Case | 2 spaces | `console.log('  → Test case name')` |
| Test Case Info | 2 spaces | `console.log('  📊 Data: ...')` |

---

## Emoji Usage Rules

| Emoji | Purpose | Location |
|-------|---------|----------|
| 📍 | Phase/Suite marker | Phase describe, Main describe |
| → | Test case start | First line in It block |
| 📊 | Test result statistics | Inside test case |
| ✓ | Success (auto by vitest) | When test passes |
| ✗ | Failure (auto by vitest) | When test fails |
| ⚠️ | Warning/Skip | Table anomalies |
| 🗓️ | Time/Date related | Date parameter output |

---

## Real Examples

### ✅ Perfect Test File Structure (with colors)

```javascript
/**
 * @fileoverview Data Correction Validation Tests
 * @description Verify correctness of electricity price calculation and database writes
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

describe('Data Correction Validation Tests', () => {
    // Layer 1: Main Suite (purple border)
    console.log(colors.purple + '\n╔════════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  Data Correction Validation Tests      ║' + colors.reset);
    console.log(colors.purple + '╚════════════════════════════════════════╝' + colors.reset);

    beforeAll(async () => { /* ... */ });
    afterAll(async () => { /* ... */ });

    // Layer 2 + 3: Phase + Test Cases
    describe('Phase 1: Basic Connection and Table Structure Validation', () => {
        console.log(colors.blue + '\n── [1] Phase 1: Basic Connection and Table Structure Validation ──' + colors.reset);

        it('DB connection success', async () => {
            console.log(colors.cyan + '  → DB connection success' + colors.reset);
            const [result] = await connection.query('SELECT 1');
            expect(result[0].test).toBe(1);
        });

        it('Contract ID 1 price table exists', async () => {
            console.log(colors.cyan + '  → Contract ID 1 price table exists' + colors.reset);
            const tables = getElePriceTables(1);
            for (const table of tables) {
                const exists = await tableExists(connection, table.name);
                console.log(`  ${exists ? colors.green + '✓' : colors.red + '✗'}${colors.reset} ${table.name}`);
            }
        });
    });

    describe('Phase 2: Data Existence Validation', () => {
        console.log(colors.blue + '\n── [2] Phase 2: Data Existence Validation ──' + colors.reset);

        it('Minute price table has data', async () => {
            console.log(colors.cyan + '  → Minute price table has data' + colors.reset);
            const count = await getRowCount(connection, tableName);
            console.log(colors.yellow + '  [Result]' + colors.reset);
            console.log(`  📊 ${tableName}: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset} records`);
            expect(count).toBeGreaterThan(0);
        });
    });
});
```

### 📊 Test Runner Output Effect (with colors)

```
✓ tests/unit/dataCorrection.test.js (3 tests) 5234ms

╔════════════════════════════════════════╗
║  Data Correction Validation Tests      ║
╚════════════════════════════════════════╝

── [1] Phase 1: Basic Connection and Table Structure Validation ──
  → DB connection success
  ✓ expected 1 to be 1

  ✓ Contract ID 1 price table exists
  [Result]
  📊 cElePResult_1_2025_1: 150 records
  ✓ expected 150 to be greater than 0

── [2] Phase 2: Data Existence Validation
  → Minute price table has data
  [Result]
  Count: 150 records
  ✓ expected 150 to be greater than 0
```

**Color effect explanation:**
- 🟪 **Purple border** - Start marker for entire test suite
- 🔵 **Blue marker** - Block markers for each phase
- 🔷 **Cyan marker** - Start of each test case
- 🟨 **Yellow tag** - Classification for result, SQL, sample, etc.
- 🟢 **Green** - Success, pass, exists
- 🔴 **Red** - Failure, not exists, anomaly

---

## When to Use Console.log vs Test Logic

### ✅ Console.log is appropriate for

- Step markers at test start
- Data statistics (record count, time range)
- Intermediate results display (tables, matches)
- Test skip reasons

### ❌ Don't use Console.log for

- Test logic judgments (use expect)
- Complex multi-line output (pollutes logs)
- Debug information (use Debug level log)

---

## Best Practices

1. **Concise** - Console.log uses only one line, complex info uses table format
2. **Clear hierarchy** - Use indentation to show level relationships
3. **Consistent** - All test files use same format
4. **Appropriate** - Don't over-log, keep readability

