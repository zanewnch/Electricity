---
name: unit-test
description: Unit Test 編寫與彩色 Console Logging 工具。支援兩個子指令：(1) create - 建立新的 test file（可基於 spec 路徑或 NLP 描述）；(2) update - 為現有 test file 補充缺失的 console.log 與 ANSI 顏色。當用戶說「寫 unit test」「建立 test file」「新增 test」「補充 console.log」「更新 test」「為 test 加顏色」「test 彩色輸出」「整合 test」時觸發。
argument-hint: <create [spec-path-or-description] | update [test-file-path]>
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(mkdir:*)
---

# Unit Test - 編寫與 Console Logging 工具

## 使用方式

```
/unit-test create [spec-path 或 description]  → 建立新的 test file（基於 spec 或 NLP 描述）
/unit-test update [test-file-path]            → 為現有 test file 補充 console.log 和顏色
```

---

## 解析 $ARGUMENTS

```
$ARGUMENTS 格式：<子指令> [參數]

子指令：
- create  → 建立新 test file（基於 spec 路徑或 NLP 描述）
- update  → 為現有 test file 補充 console.log 和顏色
```

---

# 子指令 1: create

## 輸入 - 雙模式

### 方式 A：基於 Spec 路徑

```
/unit-test create [spec-folder-path]
```

**範例：**
```
/unit-test create be/notes/spec/data-correction
```

### 方式 B：基於 NLP 描述

```
/unit-test create "[test description]"
```

**範例：**
```
/unit-test create "Phase 1: table existence check, Phase 2: data population verification, Phase 3: kWh consistency validation"
```

## 輸入檢測邏輯

| 條件 | 類型 | 處理方式 |
|------|------|---------|
| 輸入是目錄路徑 | Spec 路徑 | 讀取 `spec.md`，提取 phases 和 requirements |
| 輸入包含描述文字 | NLP 描述 | 解析描述文字，自動生成 phase structure |

**邏輯判斷：**
```
if (輸入 .includes('/') 或 輸入 .endsWith('.md')) {
  // 方式 A：spec 路徑
  讀取 spec.md → 提取 phases
} else {
  // 方式 B：NLP 描述
  解析描述文字 → 生成 phase structure
}
```

## 輸出位置

```
tests/unit/{feature-name}.test.js
```

## 流程

### 1. 讀取 Spec

- 讀取 `{spec-folder}/spec.md`
- 提取 fileoverview 的 description
- 提取所有 Requirement Phases（作為 describe blocks）

### 2. 顏色常數定義

自動加入 ANSI 顏色常數（文件頂部）：

```javascript
// 定義 ANSI 色碼常數
const colors = {
    purple: '\x1b[95m',      // 主標題邊框
    yellow: '\x1b[93m',      // 標籤（[SQL]、[結果]）
    blue: '\x1b[38;2;156;220;254m',   // Phase 標記
    cyan: '\x1b[96m',        // 一般資訊
    green: '\x1b[92m',       // 成功狀態
    red: '\x1b[91m',         // 失敗/警告
    reset: '\x1b[0m'         // 重置顏色
};
```

### 3. 生成 Test File

基於以下格式生成 test file（含顏色套用）：

```javascript
/**
 * @fileoverview [spec description]
 * @see {@link be/notes/spec/{feature-name}/spec.md}
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

describe('整體 Test Suite 名稱', () => {
    // Layer 1: Main Suite（purple 邊框）
    console.log(colors.purple + '\n╔══════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  整體 Test Suite 名稱                 ║' + colors.reset);
    console.log(colors.purple + '╚══════════════════════════════════════╝' + colors.reset);

    beforeAll(async () => {
        // setup
    });

    afterAll(async () => {
        // teardown
    });

    // Phase 1（blue 標記）
    describe('Phase 1: ...', () => {
        // ⚠️ 重要：使用 beforeAll 確保在測試執行時才輸出，而非 describe 定義時
        beforeAll(() => {
            console.log(colors.blue + '\n── [1] Phase 1: ... ──' + colors.reset);
        });

        it('測試項目 1', async () => {
            console.log(colors.cyan + '  → 測試項目 1' + colors.reset);
            // test code
            expect(...).toBe(...);
        });

        it('測試項目 2', async () => {
            console.log(colors.cyan + '  → 測試項目 2' + colors.reset);
            // test code
            expect(...).toBe(...);
        });
    });

    // Phase 2
    describe('Phase 2: ...', () => {
        beforeAll(() => {
            console.log(colors.blue + '\n── [2] Phase 2: ... ──' + colors.reset);
        });

        it('測試項目 1', async () => {
            console.log(colors.cyan + '  → 測試項目 1' + colors.reset);
            // test code with results
            const count = await getCount();
            console.log(colors.yellow + '  [結果]' + colors.reset);
            console.log(`  筆數: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
        });
    });
});
```

## Console.log 格式規則

| 層級 | 顏色 | 格式 | 例子 |
|------|------|------|------|
| **Main Suite** | purple | `console.log(colors.purple + '╔═══════════╗' + colors.reset)` | 邊框式標題 |
| **Phase** | blue | `beforeAll(() => { console.log(...) })` | 區塊分隔（必須用 beforeAll） |
| **Test Case** | cyan | `console.log(colors.cyan + '  → 測試名稱' + colors.reset)` | 步驟標記 |
| **標籤** | yellow | `console.log(colors.yellow + '  [結果]' + colors.reset)` | [SQL]、[結果]、[樣本] |
| **成功** | green | `colors.green + 'OK' + colors.reset` | 積極結果 |
| **失敗** | red | `colors.red + 'ERROR' + colors.reset` | 負面結果 |
| **一般資訊** | cyan | `console.log(colors.cyan + '  info...' + colors.reset)` | 補充說明 |

### ⚠️ 重要：Console.log 執行時機

```javascript
describe('Phase 1', () => {
    // ❌ 錯誤：這裡的 console.log 在 describe 定義時就執行（test file 載入時）
    console.log('Phase 1 開始');  // 會在所有測試開始前就輸出

    // ✅ 正確：使用 beforeAll 確保在該 describe 的測試執行前才輸出
    beforeAll(() => {
        console.log('Phase 1 開始');  // 會在 Phase 1 的第一個測試前輸出
    });

    it('測試 1', () => {
        // ✅ 正確：這裡的 console.log 在測試執行時才輸出
        console.log('測試 1 進行中');
    });
});
```

**原理：** Vitest/Jest 在載入 test file 時會立即執行所有 `describe()` 的 callback，但 `beforeAll`/`it` 內的程式碼會延遲到測試實際執行時才跑。

## 完成後提示

```
✅ Test file 已建立

📍 路徑: tests/unit/{feature-name}.test.js

📌 執行測試：
npx vitest run tests/unit/{feature-name}.test.js --reporter=verbose

💡 提示：
- 每個 console.log 會輸出到 test runner 的日誌中
- 在 UI 上可以直觀看到每個階段的進度
```

---

# 子指令 2: update

## 輸入

```
/unit-test update [test-file-path]
```

**範例：**
```
/unit-test update tests/unit/dataCorrection.test.js
```

## 功能

自動為現有 test file 中缺少 console.log 的 describe/it blocks 補充。

### 檢查清單

- [ ] 是否有 colors 常數定義？
- [ ] Main describe block 開始有 purple 邊框 console.log？
- [ ] 所有 Phase describe blocks 有 `beforeAll` 包裝的 blue 標記 console.log？
- [ ] 所有 it blocks 開始有 cyan 標記 console.log？
- [ ] 所有結果/資料輸出是否套用正確顏色（green/red/yellow）？

### 補充邏輯

#### 1. Colors 常數定義

檢查文件頂部是否有 colors 定義，若無則補上：

```javascript
// ❌ 沒有 colors 定義
import { describe, it, expect } from 'vitest';

describe('整體名稱', () => { ... });

// ✅ 補充為：
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

describe('整體名稱', () => { ... });
```

#### 2. Main Describe Block

檢查是否已有 purple 邊框 console.log，若無則補上：

```javascript
describe('整體名稱', () => {
    // ❌ 沒有或顏色不對
    console.log('\n========================================');
    console.log('📍 整體名稱');
    console.log('========================================\n');
    it('test', () => { ... });

    // ✅ 補充為：
    console.log(colors.purple + '\n╔══════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  整體名稱                            ║' + colors.reset);
    console.log(colors.purple + '╚══════════════════════════════════════╝' + colors.reset);
    
    it('test', () => { ... });
});
```

#### 3. Phase Describe Blocks

檢查 Phase describe block 開始是否有 blue 標記 console.log（必須放在 `beforeAll` 內），若無則補上：

```javascript
describe('Phase 1: 基礎驗證', () => {
    // ❌ 錯誤：直接在 describe 內 console.log（會在 describe 定義時執行，而非測試時）
    console.log('\n📍 Phase 1: 基礎驗證');
    it('test', () => { ... });

    // ✅ 正確：使用 beforeAll 確保在測試執行時才輸出
    beforeAll(() => {
        console.log(colors.blue + '\n── [1] Phase 1: 基礎驗證 ──' + colors.reset);
    });

    it('test', () => { ... });
});
```

#### 4. It Blocks

檢查每個 it block 開始是否有 cyan 標記 console.log，若無則補上：

```javascript
it('驗證 DB 連線', async () => {
    // ❌ 沒有 console.log 作為第一行
    const result = await query();
    expect(result).toBe(true);

    // ✅ 補充為：
    it('驗證 DB 連線', async () => {
        console.log(colors.cyan + '  → 驗證 DB 連線' + colors.reset);
        const result = await query();
        expect(result).toBe(true);
    });
});
```

#### 5. 結果與資料輸出顏色

檢查是否有使用條件色彩輸出結果（green 成功、red 失敗），若無則補上：

```javascript
// ❌ 沒有顏色區分
console.log(`  筆數: ${count}`);

// ✅ 補充為：
console.log(colors.yellow + '  [結果]' + colors.reset);
console.log(`  筆數: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
```

### 檢查與補充流程

```bash
1. 檢查文件頂部是否有 colors 常數，若無則補上
2. 解析 AST 或 Regex 尋找所有 describe/it blocks
3. 檢查每個 block 的開始是否有正確顏色的 console.log
   - Main describe → purple 邊框（直接 console.log）
   - Phase describe → blue 標記（必須放在 beforeAll 內）
   - It block → cyan 標記（直接 console.log）
4. 檢查結果/資料輸出是否有條件顏色（green/red/yellow）
5. 補充缺失的 console.log 和顏色（保持原有縮排）
6. 驗證所有修改後的程式碼是否有效
```

## 完成後提示

```
✅ Test file 更新完成

📊 修改統計：
- Colors 常數: [新增/已存在]
- Main describe 邊框: +N 個
- Phase describe 標記: +N 個
- Test case 標記: +N 個
- 結果顏色條件: +N 個

🎨 顏色套用：
- purple (邊框): 新增 1 個
- blue (Phase): 新增 N 個
- cyan (步驟): 新增 N 個
- yellow (標籤): 新增 N 個
- green/red (結果): 新增 N 個

📍 文件: tests/unit/xxx.test.js

💡 建議執行測試確認效果：
npx vitest run tests/unit/xxx.test.js --reporter=verbose
```

---

## Console.log 輸出範例

### 在 Test Runner UI 看到的效果

```
✓ Data Correction 驗證測試

========================================
📍 Data Correction 驗證測試
========================================

✓ MySQL 連線建立成功

📍 Phase 1: 基礎連線與表結構驗證
  → DB 連線成功
  ✓ expected 1 to be 1
  
  → 契約 ID 1 的電價表存在
  ✓ expected true to be true

📍 Phase 2: 資料存在性驗證
  → 分鐘電價表有資料
  📊 cElePResult_1_2025_1: 150 筆
  ✓ expected 150 to be greater than 0

📍 Phase 3: kW/kWh 一致性驗證
  → 驗證 computekWResult.kW / 60 === cElePResult.kWh
  📊 驗證 10 筆 EM 資料
  ✓ expected true to be true
```

---

## 注意事項

1. **console.log 格式統一** - 三層級的格式保持一致
2. **縮排正確** - 根據程式碼層級自動調整縮排
3. **不修改測試邏輯** - 只補充 console.log，不改變原有的測試程式碼
4. **保留現有 console.log** - 如果已有 console.log，不重複添加
5. **⚠️ Phase 的 console.log 必須放在 beforeAll 內** - 避免在 test file 載入時就執行

---

## 參考文件與指南

**Skill 文檔：**
- [FORMAT-GUIDE.md](references/FORMAT-GUIDE.md) - Console.log 三層級格式詳解
- [COLORS-GUIDE.md](references/COLORS-GUIDE.md) - ANSI 顏色使用完整指南
- [USAGE-EXAMPLES.md](references/USAGE-EXAMPLES.md) - 使用範例與場景說明

**實際範例：**
- [dataCorrection.test.js](../../../tests/unit/dataCorrection.test.js) - 基礎格式範例
- [emCsvRepo.test.js](../../../tests/unit/em/emCsvRepo.test.js) - 完整顏色應用範例

**外部參考：**
- [Vitest Reporter Docs](https://vitest.dev/guide/reporters.html)
- [ANSI Color Codes](https://en.wikipedia.org/wiki/ANSI_escape_code)

$ARGUMENTS
