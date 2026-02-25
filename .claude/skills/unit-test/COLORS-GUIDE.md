# Unit Test 顏色使用指南

## 定義 ANSI 顏色常數

所有 test file 開頭都應包含此常數定義：

```javascript
// 定義 ANSI 色碼常數
const colors = {
    purple: '\x1b[95m',                    // 主標題邊框
    yellow: '\x1b[93m',                    // 標籤/分類
    blue: '\x1b[38;2;156;220;254m',       // Phase 標記（冷色 RGB）
    cyan: '\x1b[96m',                      // 步驟/一般資訊
    green: '\x1b[92m',                     // 成功/正面結果
    red: '\x1b[91m',                       // 失敗/負面結果
    reset: '\x1b[0m'                       // 重置顏色
};
```

---

## 顏色用途對應表

| 顏色 | 色碼 | 用途 | 位置 | 例子 |
|------|------|------|------|------|
| **Purple** | `\x1b[95m` | 主標題邊框 | Main describe block | `╔═══════════╗` |
| **Blue** | `\x1b[38;2;156;220;254m` | Phase 分隔標記 | Phase describe block | `── [1] Phase 1: ... ──` |
| **Cyan** | `\x1b[96m` | 步驟標記/一般資訊 | It block 開始 | `→ 驗證 DB 連線` |
| **Yellow** | `\x1b[93m` | 標籤/分類 | 結果分類 | `[結果]`、`[SQL]`、`[樣本]` |
| **Green** | `\x1b[92m` | 成功/存在 | 結果判斷 | 表示通過、✓、資料存在 |
| **Red** | `\x1b[91m` | 失敗/不存在 | 結果判斷 | 表示失敗、✗、無資料 |
| **Reset** | `\x1b[0m` | 重置顏色 | 每個色彩段末尾 | `... + colors.reset` |

---

## 層級別顏色使用

### 1️⃣ Main Suite（紫色邊框）

**目的：** 清楚標示整個 test suite 的開始

**位置：** `describe('Main Suite Name', () => { ... })` 的最開始

**格式：**
```javascript
describe('EmCsvRepo 整合測試', () => {
    console.log(colors.purple + '\n╔════════════════════════════════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  EmCsvRepo 整合測試 - Debug Mode                              ║' + colors.reset);
    console.log(colors.purple + '╚════════════════════════════════════════════════════════════════╝' + colors.reset);
    
    // ... rest of describe
});
```

**UI 輸出：**
```
╔════════════════════════════════════════════════════════════════╗
║  EmCsvRepo 整合測試 - Debug Mode                              ║
╚════════════════════════════════════════════════════════════════╝
```

**為什麼用 Purple？**
- 視覺上的「大標題」，與其他層級區分明顯
- 紫色較少用於結果判斷，避免混淆

---

### 2️⃣ Phase Blocks（藍色標記）

**目的：** 區分不同的測試階段/功能群組

**位置：** 每個 `describe('Phase N: ...', () => { ... })` 的最開始

**格式：**
```javascript
describe('1. 資料表存在性檢查', () => {
    console.log(colors.blue + '\n── [1] 資料表存在性檢查 ──' + colors.reset);
    
    it('LOG_Current 表應存在', () => { ... });
    it('eMeterAcckWhByHour 表應存在', () => { ... });
});

describe('2. 直接 SQL 查詢', () => {
    console.log(colors.blue + '\n── [2] 直接 SQL 查詢 ──' + colors.reset);
    
    it('minutes 模式應有資料', () => { ... });
});
```

**UI 輸出：**
```
── [1] 資料表存在性檢查 ──
  → LOG_Current 表應存在
  ✓
  
  → eMeterAcckWhByHour 表應存在
  ✓

── [2] 直接 SQL 查詢 ──
  → minutes 模式應有資料
  ✓
```

**為什麼用 Blue？**
- 冷色調，傳達「結構化」、「邏輯區分」的感覺
- RGB(156, 220, 254) 是 VS Code 預設的藍色，很舒適

---

### 3️⃣ Test Case（青色標記）

**目的：** 標示每個測試的開始

**位置：** 每個 `it('test name', async () => { ... })` 的第一行邏輯

**格式：**
```javascript
it('DB 連線成功', async () => {
    console.log(colors.cyan + '  → DB 連線成功' + colors.reset);
    
    const [result] = await connection.query('SELECT 1 as test');
    expect(result[0].test).toBe(1);
});

it('分鐘電價表有資料', async () => {
    console.log(colors.cyan + '  → 分鐘電價表有資料' + colors.reset);
    
    const count = await getRowCount(connection, tableName);
    console.log(`  📊 ${tableName}: ${count} 筆`);
    
    expect(count).toBeGreaterThan(0);
});
```

**UI 輸出：**
```
  → DB 連線成功
  ✓ expected 1 to be 1

  → 分鐘電價表有資料
  📊 cElePResult_1_2025_1: 150 筆
  ✓ expected 150 to be greater than 0
```

**為什麼用 Cyan？**
- 較淡的青色，不會搶眼但清晰可見
- 與 Phase 的藍色區分，表示「更細粒度」

---

### 4️⃣ 標籤（黃色）

**目的：** 分類不同的資訊區塊（SQL、結果、樣本等）

**位置：** 結果、資訊區塊的開頭

**格式：**
```javascript
it('minutes 模式應有資料', async () => {
    console.log(colors.cyan + '  → minutes 模式應有資料' + colors.reset);
    
    console.log(colors.yellow + '\n  [SQL]' + colors.reset);
    console.log(colors.cyan + '  SELECT COUNT(*) FROM LOG_Current...' + colors.reset);
    
    const [rows] = await connection.query(sql);
    const count = rows[0].cnt;
    
    console.log(colors.yellow + '\n  [結果]' + colors.reset);
    console.log(`  筆數: ${count > 0 ? colors.green + count : colors.red + count}${colors.reset}`);
    
    if (count > 0) {
        console.log(colors.yellow + '  [樣本]' + colors.reset);
        samples.forEach(row => {
            console.log(colors.cyan + `  ${row.BLEAddress} | ${row.timeStamp}` + colors.reset);
        });
    }
});
```

**UI 輸出：**
```
  → minutes 模式應有資料

  [SQL]
  SELECT COUNT(*) FROM LOG_Current...

  [結果]
  筆數: 150

  [樣本]
  EM001_t | 2025-01-23 10:30:45
  EM002_t | 2025-01-23 10:30:40
  EM003_t | 2025-01-23 10:30:35
```

**為什麼用 Yellow？**
- 醒目但不會喧賓奪主
- 用來標示「資訊分類」而非判斷結果

---

### 5️⃣ 成功結果（綠色）

**目的：** 表示正面結果、通過、存在

**位置：** 條件為真時的資訊輸出

**格式：**
```javascript
// 簡單的成功標示
const exists = await tableExists(connection, tableName);
console.log(`  ${exists ? colors.green + '✓' : colors.red + '✗'}${colors.reset} ${tableName}`);

// 條件色彩輸出（推薦）
const count = await getRowCount(connection, tableName);
console.log(colors.yellow + '  [筆數]' + colors.reset);
console.log(`  ${count > 0 ? colors.green + count + ' 筆' : colors.red + '無資料'}${colors.reset}`);

// 完整的成功訊息
if (allTestsPassed) {
    console.log(colors.green + '  ✅ 全部通過' + colors.reset);
}
```

**UI 輸出：**
```
  ✓ LOG_Current_202501
  筆數: 150 筆
  ✅ 全部通過
```

**為什麼用 Green？**
- 國際通用的「通過」、「正常」信號
- 用戶一眼就知道哪些部分成功

---

### 6️⃣ 失敗結果（紅色）

**目的：** 表示負面結果、失敗、不存在、異常

**位置：** 條件為假時的資訊輸出

**格式：**
```javascript
// 簡單的失敗標示
const exists = await tableExists(connection, tableName);
console.log(`  ${exists ? colors.green + '✓' : colors.red + '✗'}${colors.reset} ${tableName}`);

// 條件色彩輸出（推薦）
const count = await getRowCount(connection, tableName);
console.log(`  ${count > 0 ? colors.green + count : colors.red + count}${colors.reset} 筆`);

// 警告訊息
if (count === 0) {
    console.log(colors.red + '  ⚠️ 無資料！可能是彙總任務沒有執行' + colors.reset);
}
```

**UI 輸出：**
```
  ✗ eMeterAcckWhByHour_202501
  0 筆
  ⚠️ 無資料！可能是彙總任務沒有執行
```

**為什麼用 Red？**
- 國際通用的「失敗」、「異常」信號
- 與 Green 形成清楚對比

---

## 實際完整範例

參考 [emCsvRepo.test.js](../../../tests/unit/em/emCsvRepo.test.js) 第 60-90 行的實現方式：

```javascript
beforeAll(async () => {
    // ... DB setup ...
    
    console.log(colors.purple + '╔═══════════════════════════════════════════════════════════════╗' + colors.reset);
    console.log(colors.purple + '║  EmCsvRepo 整合測試 - Debug Mode                              ║' + colors.reset);
    console.log(colors.purple + '╚═══════════════════════════════════════════════════════════════╝' + colors.reset);
    console.log(colors.cyan + '\n測試設備: ' + colors.reset + testBLEAddress.join(', '));
    console.log(colors.cyan + '時間範圍: ' + colors.reset + startTimeStr + ' ~ ' + endTimeStr);
});

describe('EmCsvRepo 整合測試', () => {
    describe('1. 資料表存在性檢查', () => {
        console.log(colors.blue + '\n── [1] 資料表存在性檢查 ──' + colors.reset);
        
        it('LOG_Current 表應存在', async () => {
            console.log(colors.blue + '  [1-1] minutes: LOG_Current_YYYYMM 表應存在' + colors.reset);
            
            const [rows] = await mysqlPool.query(sql);
            const tableName = `mqtt.LOG_Current_${currentMonth}`;
            
            console.log(`   ${tableName}: ${rows[0].cnt > 0 ? colors.green + '✓ 存在' : colors.red + '✗ 不存在'}${colors.reset}`);
            expect(rows[0].cnt).toBeGreaterThan(0);
        });
    });
});
```

---

## 最佳實踐

### ✅ 應該做的

1. **一致性** - 所有 test file 用同樣的顏色規則
2. **層級感** - 用不同顏色清楚表達結構層次
3. **適度使用** - 不要過度著色，影響可讀性
4. **條件色彩** - 結果用 green/red 來視覺化成功/失敗

### ❌ 不應該做的

1. **隨意著色** - 每個 console.log 都換顏色會很亂
2. **過多層級** - 超過 3-4 層顏色會讓人眼花
3. **錯誤配對** - 不要用紅色表示成功
4. **忘記 reset** - 每個顏色段末尾一定要加 `colors.reset`

---

## Debug 技巧

### 檢查顏色代碼是否正確

```bash
# 在終端直接測試顏色輸出
node -e "console.log('\x1b[95m Purple Text \x1b[0m')"
```

### 常見問題

| 問題 | 原因 | 解決方案 |
|------|------|--------|
| 顏色沒顯示 | 終端不支持 ANSI | 使用支持 ANSI 的終端（VS Code、iTerm2 等） |
| 顏色變成亂碼 | 忘記加 `reset` | 確保每個顏色段後加 `colors.reset` |
| 背景被著色 | 使用了背景色代碼 | 只使用前景色（前三位數字） |
| 顏色淡淡的 | 終端設定問題 | 調整終端的顏色亮度設定 |

