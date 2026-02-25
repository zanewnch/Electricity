# Unit Test Skill - 使用範例

## 雙向觸發機制

你可以用 **NLP 自然語言** 或 **手動指令** 兩種方式來觸發這個 skill。

---

## 方式 1️⃣：NLP 觸發（自動識別）

### 例子 1：新建 test file

```
我需要寫一個 data-correction 的 unit test
```

**Claude 會自動識別這是 scaffold 操作，詢問 spec 路徑**

---

### 例子 2：補充 console.log

```
幫我為現有的 test file 補充 console.log 和顏色
```

**Claude 會自動識別這是 enhance 操作，詢問 test file 路徑**

---

### 例子 3：彩色輸出

```
我希望 emCsvRepo.test.js 能有彩色的 console 輸出
```

**Claude 會自動識別這是 enhance 操作**

---

### 例子 4：整合 test

```
為我整合這個 test，加上 console.log 和顏色
```

**Claude 會自動觸發 enhance**

---

### ✅ NLP 觸發的關鍵詞

```
寫 unit test
建立 test file
新增 test
補充 console.log
增強 test
為 test 加顏色
test 彩色輸出
整合 test
```

任何包含這些關鍵詞的自然語言都會自動觸發 skill。

---

## 方式 2️⃣：手動指令（Argument）

### Scaffold 子指令

新建 test file（根據 spec）：

```
/unit-test scaffold be/notes/spec/data-correction
```

**執行流程：**
1. 讀取 `be/notes/spec/data-correction/spec.md`
2. 提取 description、phases、requirements
3. 生成 `tests/unit/dataCorrection.test.js`
4. 自動加入 console.log + ANSI 顏色

---

### Enhance 子指令

補充現有 test file：

```
/unit-test enhance tests/unit/emCsvRepo.test.js
```

**執行流程：**
1. 讀取 `tests/unit/emCsvRepo.test.js`
2. 檢查缺失的：
   - Colors 常數定義
   - Main describe 邊框
   - Phase describe 標記
   - It block 標記
   - 結果顏色條件
3. 補充所有缺失的 console.log + 顏色
4. 驗證語法正確性

---

## 場景對應表

| 場景 | NLP 說法 | 手動指令 |
|------|---------|--------|
| **新建 test** | 「幫我寫 unit test」 | `/unit-test scaffold path` |
| **補充 log** | 「補充 console.log」 | `/unit-test enhance path` |
| **加顏色** | 「為 test 加顏色」 | `/unit-test enhance path` |
| **完整整合** | 「整合這個 test」 | `/unit-test enhance path` |

---

## 實戰例子

### 場景 1：剛寫好 spec，想自動生成 test file

**你可以說：**
```
根據 be/notes/spec/data-correction 的 spec，幫我建立 test file
```

或者直接用指令：
```
/unit-test scaffold be/notes/spec/data-correction
```

**結果：**
- 自動生成 `tests/unit/dataCorrection.test.js`
- 包含 phases 對應的 describe blocks
- 每個 block 都有格式化的 console.log + ANSI 顏色

---

### 場景 2：有舊的 test file，想加上 console.log 和顏色

**你可以說：**
```
我的 emCsvRepo.test.js 已經有 colors 定義了，幫我補充缺失的 console.log
```

或者直接用指令：
```
/unit-test enhance tests/unit/em/emCsvRepo.test.js
```

**結果：**
- 檢查是否有 colors 常數（有則保留，無則補上）
- 補充缺失的 console.log（Main、Phase、It blocks）
- 補充結果顏色條件（green/red）
- 不修改現有的正確 console.log

---

### 場景 3：現有 test 需要整體改進

**你可以說：**
```
幫我整合 dataCorrection.test.js，加上完整的 console.log 格式和彩色輸出
```

或者用指令：
```
/unit-test enhance tests/unit/dataCorrection.test.js
```

---

## 混合使用

你也可以先用 NLP 說出你的需求，Claude 會幫你確認路徑後執行：

```
用戶: 我需要為這個 test 加彩色
      ↓
Claude: 要加彩色的是哪個 test file？(會列出或讓你輸入路徑)
      ↓
用戶: tests/unit/emCsvRepo.test.js
      ↓
Claude: 執行 /unit-test enhance tests/unit/em/emCsvRepo.test.js
      ↓
結果: ✅ 補充完成
```

---

## 最佳實踐

### ✅ 何時用 NLP

- 你不確定確切的路徑
- 你想讓 Claude 幫你分析需求
- 你想要更多互動和確認

### ✅ 何時用手動指令

- 你已經知道確切的路徑
- 你想快速執行，不需要確認
- 你習慣用 CLI 風格的指令

---

## 進階用法

### 同時處理多個 test files

```
我想為 tests/unit 底下所有的 test files 都加上彩色輸出
```

Claude 會：
1. 掃描 `tests/unit` 目錄
2. 列出所有 `.test.js` 文件
3. 逐個執行 enhance
4. 生成完整的修改報告

---

## FAQ

**Q: 如果我用 NLP，會自動執行嗎？**
A: 不會，Claude 會識別你的需求，詢問確認路徑或參數後再執行。

**Q: 我可以同時用 NLP 和指令混合嗎？**
A: 可以。你可以先用 NLP 描述需求，再用指令指定具體路徑。

**Q: Scaffold 和 Enhance 有什麼區別？**
A:
- **Scaffold**：從零開始根據 spec 生成全新 test file
- **Enhance**：改進現有 test file，補充缺失的 console.log 和顏色

**Q: 如果 test file 已經有 console.log，Enhance 會重複嗎？**
A: 不會。Enhance 會智能檢測，只補充缺失的部分。

