# Unit Test Color Usage Guide

## Define ANSI Color Constants

All test files should include this constant definition at the beginning:

```javascript
// Define ANSI color codes
const colors = {
    purple: '\x1b[95m',                    // Main title border
    yellow: '\x1b[93m',                    // Tags/Categories
    blue: '\x1b[38;2;156;220;254m',       // Phase marker (cool color RGB)
    cyan: '\x1b[96m',                      // Steps/General info
    green: '\x1b[92m',                     // Success/Positive result
    red: '\x1b[91m',                       // Failure/Negative result
    reset: '\x1b[0m'                       // Reset color
};
```

---

## Color Purpose Mapping Table

| Color | Code | Purpose | Location | Example |
|-------|------|---------|----------|---------|
| **Purple** | `\x1b[95m` | Main title border | Main describe block | `╔═══════════╗` |
| **Blue** | `\x1b[38;2;156;220;254m` | Phase separator marker | Phase describe block | `── [1] Phase 1: ... ──` |
| **Cyan** | `\x1b[96m` | Step marker/General info | It block start | `→ Verify DB connection` |
| **Yellow** | `\x1b[93m` | Tag/Category | Result classification | `[Result]`, `[SQL]`, `[Sample]` |
| **Green** | `\x1b[92m` | Success/Exists | Result judgment | Indicates pass, ✓, data exists |
| **Red** | `\x1b[91m` | Failure/Not exists | Result judgment | Indicates fail, ✗, no data |
| **Reset** | `\x1b[0m` | Reset color | End of each color segment | `... + colors.reset` |

---

## Color Usage by Level

### 1️⃣ Main Suite (Purple Border)

**Purpose:** Clearly mark the beginning of entire test suite

**Location:** Start of `describe('Main Suite Name', () => { ... })`

**Format:**
```javascript
describe('EmCsvRepo Integration Tests', () => {
    console.log(colors.purple + '\n╔════════════════════════════════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  EmCsvRepo Integration Tests - Debug Mode                       ║' + colors.reset);
    console.log(colors.purple + '╚════════════════════════════════════════════════════════════════╝' + colors.reset);
```

**Output effect:**
```
╔════════════════════════════════════════════════════════════════╗
║  EmCsvRepo Integration Tests - Debug Mode                      ║
╚════════════════════════════════════════════════════════════════╝
```

---

### 2️⃣ Phase Blocks (Blue Marker)

**Purpose:** Mark beginning and separation of each phase block

**Location:** Inside `beforeAll(() => { ... })` at start of Phase describe block

**⚠️ Important:** Must be inside `beforeAll`, not directly in describe

**Format:**
```javascript
describe('Phase 1: Read CSV', () => {
    beforeAll(() => {
        console.log(colors.blue + '\n── [1] Phase 1: Read CSV ──' + colors.reset);
    });

    it('test case', () => { ... });
});
```

**Output effect:**
```
── [1] Phase 1: Read CSV ──
```

---

### 3️⃣ Test Cases (Cyan Marker)

**Purpose:** Mark individual test case execution

**Location:** First line of each `it()` block

**Format:**
```javascript
it('should read CSV file', async () => {
    console.log(colors.cyan + '  → Should read CSV file' + colors.reset);
    const data = await readFile();
    expect(data).toBeDefined();
});
```

**Output effect:**
```
  → Should read CSV file
```

---

## Special Colors

### Yellow (Tags)

**Purpose:** Classify or label result sections

**Use cases:**
```javascript
// Label result section
console.log(colors.yellow + '  [Result]' + colors.reset);

// Label SQL query
console.log(colors.yellow + '  [SQL]' + colors.reset);

// Label data sample
console.log(colors.yellow + '  [Sample]' + colors.reset);
```

### Green (Success)

**Purpose:** Indicate positive/successful results

**Use cases:**
```javascript
// Condition check
console.log(colors.green + '✓ Data exists' + colors.reset);

// Data output with condition
const count = 100;
console.log(`  Count: ${colors.green + count + colors.reset}`);

// Success message
console.log(colors.green + 'Setup completed successfully' + colors.reset);
```

### Red (Failure)

**Purpose:** Indicate negative/failure results

**Use cases:**
```javascript
// Condition check
console.log(colors.red + '✗ Data missing' + colors.reset);

// Data output with condition
const count = 0;
console.log(`  Count: ${colors.red + count + colors.reset}`);

// Error message
console.log(colors.red + 'Connection failed' + colors.reset);
```

---

## RGB Color (Blue)

The blue color uses RGB format for better visual control:

```javascript
blue: '\x1b[38;2;156;220;254m'   // RGB(156, 220, 254) - light blue
```

This produces a softer, cooler blue tone suitable for Phase markers.

---

## Color Combination Examples

### Example 1: Simple Result Check

```javascript
it('verify count > 0', async () => {
    console.log(colors.cyan + '  → Verify count > 0' + colors.reset);
    const count = await getCount();
    console.log(colors.yellow + '  [Result]' + colors.reset);
    console.log(`  Count: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
    expect(count).toBeGreaterThan(0);
});
```

**Output:**
```
  → Verify count > 0
  [Result]
  Count: 123
```

### Example 2: Multiple Checks

```javascript
it('verify data integrity', async () => {
    console.log(colors.cyan + '  → Verify data integrity' + colors.reset);
    const result = await verify();

    console.log(colors.yellow + '  [Checks]' + colors.reset);
    console.log(`  Schema valid: ${result.schemaValid ? colors.green + 'Yes' : colors.red + 'No'}${colors.reset}`);
    console.log(`  Data complete: ${result.complete ? colors.green + 'Yes' : colors.red + 'No'}${colors.reset}`);

    expect(result.valid).toBe(true);
});
```

**Output:**
```
  → Verify data integrity
  [Checks]
  Schema valid: Yes
  Data complete: Yes
```

---

## Common Mistakes & Fixes

### ❌ Mistake 1: Using color without reset

```javascript
// Wrong - color leaks to next line
console.log(colors.green + '✓ Success');

// Correct
console.log(colors.green + '✓ Success' + colors.reset);
```

### ❌ Mistake 2: Multiple color segments without reset between

```javascript
// Wrong - colors overlap
console.log(colors.green + 'Success ' + colors.red + 'Failure');

// Correct
console.log(colors.green + 'Success' + colors.reset + ' ' + colors.red + 'Failure' + colors.reset);
```

### ❌ Mistake 3: Phase marker outside beforeAll

```javascript
// Wrong - executes during file load
describe('Phase 1', () => {
    console.log(colors.blue + 'Phase 1' + colors.reset);
    it('test', () => { ... });
});

// Correct - executes during test run
describe('Phase 1', () => {
    beforeAll(() => {
        console.log(colors.blue + 'Phase 1' + colors.reset);
    });
    it('test', () => { ... });
});
```

---

## Testing Colors

To verify colors display correctly, run:

```bash
npx vitest run tests/unit/example.test.js --reporter=verbose
```

If colors don't display, check:
1. Terminal supports ANSI colors
2. Color codes are not escaped
3. Reset code (`colors.reset`) is properly placed

