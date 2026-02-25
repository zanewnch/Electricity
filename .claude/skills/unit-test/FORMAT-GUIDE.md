# Unit Test Console.log 格式指南

## 設計哲學

**目標：** 在 Test Runner UI 中清楚看到每個測試的進度與意圖

### 三層級 Console.log 結構

```
Layer 1: Main Suite (分隔線樣式)
  ↓
Layer 2: Phases (📍 標記)
  ↓
Layer 3: Test Cases (→ 標記)
```

---

## 層級 1️⃣ - Main Suite

**位置：** describe block 最開始

**格式：**
```javascript
describe('Data Correction 驗證測試', () => {
    console.log('\n========================================');
    console.log('📍 Data Correction 驗證測試');
    console.log('========================================\n');
    
    // 其他 describe blocks...
});
```

**輸出效果：**
```
========================================
📍 Data Correction 驗證測試
========================================
```

**用途：** 用分隔線清楚標示整個 test suite 的開始

---

## 層級 2️⃣ - Phase Blocks

**位置：** 每個 Phase describe block 開始

**格式：**
```javascript
describe('Phase 1: 基礎連線與表結構驗證', () => {
    console.log('\n📍 Phase 1: 基礎連線與表結構驗證');
    
    it('test case 1', () => { ... });
    it('test case 2', () => { ... });
});

describe('Phase 2: 資料存在性驗證', () => {
    console.log('\n📍 Phase 2: 資料存在性驗證');
    
    it('test case 1', () => { ... });
});
```

**輸出效果：**
```
📍 Phase 1: 基礎連線與表結構驗證
  → test case 1
  → test case 2

📍 Phase 2: 資料存在性驗證
  → test case 1
```

**用途：** 區分不同 Phase，視覺上分段

---

## 層級 3️⃣ - Test Cases (It Blocks)

**位置：** 每個 it block 內部，作為第一行邏輯

**格式：**
```javascript
it('驗證 DB 連線成功', async () => {
    console.log('  → 驗證 DB 連線成功');
    
    const [result] = await connection.query('SELECT 1 as test');
    expect(result[0].test).toBe(1);
});

it('分鐘電價表有資料', async () => {
    console.log('  → 分鐘電價表有資料');
    
    const count = await getRowCount(connection, tableName);
    console.log(`  📊 ${tableName}: ${count} 筆`);
    expect(count).toBeGreaterThan(0);
});
```

**輸出效果：**
```
  → 驗證 DB 連線成功
  ✓ 

  → 分鐘電價表有資料
  📊 cElePResult_1_2025_1: 150 筆
  ✓
```

**用途：** 每個 test case 一目瞭然，加上 indent 與 Phase 區隔

---

## 縮排規則

| 層級 | 縮排 | 例子 |
|------|------|------|
| Main Suite | 無 | `console.log('========================================')` |
| Phase | 無 | `console.log('\n📍 Phase 1: ...')` |
| Test Case | 2 spaces | `console.log('  → Test case name')` |
| Test Case 內部資訊 | 2 spaces | `console.log('  📊 Data: ...')` |

---

## Emoji 使用規則

| Emoji | 用途 | 位置 |
|-------|------|------|
| 📍 | Phase/Suite 標記 | Phase describe, Main describe |
| → | Test case 開始 | It block 內第一行 |
| 📊 | 測試結果統計 | Test case 內部 |
| ✓ | 成功（由 vitest 自動） | 測試通過時 |
| ✗ | 失敗（由 vitest 自動） | 測試失敗時 |
| ⚠️ | 警告/跳過 | 表格異常時 |
| 🗓️ | 時間/日期相關 | 日期參數輸出 |

---

## 實際範例

### ✅ 完美的 Test File 結構（含顏色）

```javascript
/**
 * @fileoverview Data Correction 驗證測試
 * @description 驗證電價計算和資料庫寫入的正確性
 */

import { describe, it, beforeAll, afterAll, expect } from 'vitest';

// 定義 ANSI 色碼常數
const colors = {
    purple: '\x1b[95m',
    yellow: '\x1b[93m',
    blue: '\x1b[38;2;156;220;254m',
    cyan: '\x1b[96m',
    green: '\x1b[92m',
    red: '\x1b[91m',
    reset: '\x1b[0m'
};

describe('Data Correction 驗證測試', () => {
    // Layer 1: Main Suite（purple 邊框）
    console.log(colors.purple + '\n╔════════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  Data Correction 驗證測試              ║' + colors.reset);
    console.log(colors.purple + '╚════════════════════════════════════════╝' + colors.reset);

    beforeAll(async () => { /* ... */ });
    afterAll(async () => { /* ... */ });

    // Layer 2 + 3: Phase + Test Cases
    describe('Phase 1: 基礎連線與表結構驗證', () => {
        console.log(colors.blue + '\n── [1] Phase 1: 基礎連線與表結構驗證 ──' + colors.reset);

        it('DB 連線成功', async () => {
            console.log(colors.cyan + '  → DB 連線成功' + colors.reset);
            const [result] = await connection.query('SELECT 1');
            expect(result[0].test).toBe(1);
        });

        it('契約 ID 1 的電價表存在', async () => {
            console.log(colors.cyan + '  → 契約 ID 1 的電價表存在' + colors.reset);
            const tables = getElePriceTables(1);
            for (const table of tables) {
                const exists = await tableExists(connection, table.name);
                console.log(`  ${exists ? colors.green + '✓' : colors.red + '✗'}${colors.reset} ${table.name}`);
            }
        });
    });

    describe('Phase 2: 資料存在性驗證', () => {
        console.log(colors.blue + '\n── [2] Phase 2: 資料存在性驗證 ──' + colors.reset);

        it('分鐘電價表有資料', async () => {
            console.log(colors.cyan + '  → 分鐘電價表有資料' + colors.reset);
            const count = await getRowCount(connection, tableName);
            console.log(colors.yellow + '  [結果]' + colors.reset);
            console.log(`  📊 ${tableName}: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset} 筆`);
            expect(count).toBeGreaterThan(0);
        });
    });
});
```

### 📊 Test Runner 輸出效果（含顏色）

```
✓ tests/unit/dataCorrection.test.js (3 tests) 5234ms

╔════════════════════════════════════════╗
║  Data Correction 驗證測試              ║
╚════════════════════════════════════════╝

── [1] Phase 1: 基礎連線與表結構驗證 ──
  → DB 連線成功
  ✓ expected 1 to be 1
  
  ✓ 契約 ID 1 的電價表存在
  [結果]
  📊 cElePResult_1_2025_1: 150 筆
  ✓ expected 150 to be greater than 0

── [2] Phase 2: 資料存在性驗證
  → 分鐘電價表有資料
  [結果]
  筆數: 150 筆
  ✓ expected 150 to be greater than 0
```

**顏色效果說明：**
- 🟪 **紫色邊框** - 整個 test suite 的開始標記
- 🔵 **藍色標記** - 各 Phase 的區塊標記
- 🔷 **青色標記** - 每個 test case 的開始
- 🟨 **黃色標籤** - 結果、SQL、樣本等分類
- 🟢 **綠色** - 成功、通過、存在
- 🔴 **紅色** - 失敗、不存在、異常

---

## 何時使用 Console.log vs 測試邏輯

### ✅ Console.log 適合

- 測試開始時的步驟標記
- 資料統計（筆數、時間範圍）
- 中間結果展示（表格、匹配項目）
- 測試跳過原因

### ❌ 不要用 Console.log

- 測試邏輯判斷（用 expect）
- 複雜的多行輸出（會污染日誌）
- 調試資訊（用 Debug level log）

---

## 最佳實踐

1. **簡潔** - console.log 只用一行，複雜資訊用表格格式
2. **清層** - 用縮排清楚顯示層級關係
3. **一致** - 所有 test file 用同樣的格式
4. **適量** - 不要過度 logging，影響可讀性

