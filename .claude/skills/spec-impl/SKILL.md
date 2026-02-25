---
name: spec-impl
description: 執行 spec 實作與審查。支援兩個子指令：(1) run - 根據 spec 執行實作；(2) review - 審查實作結果是否符合 spec。當用戶說「實作 spec」「執行 spec」「審查實作」「review implementation」時觸發。
argument-hint: <run|review> [spec 資料夾路徑]
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(npm:*, node:*)
---

# Spec-Impl - 規格實作與審查

## 使用方式

```
/spec-impl run [spec 資料夾路徑]      → 執行實作
/spec-impl review [spec 資料夾路徑]   → 審查實作結果（implement 後）
```

---

## 解析 $ARGUMENTS

```
$ARGUMENTS 格式：<子指令> [spec 資料夾路徑]

子指令：
- run    → 執行「實作 Spec」流程
- review → 執行「審查實作結果」流程
```

---

# 子指令 1: run

## 輸入

```
/spec-impl run [spec 資料夾路徑]
```

例如：`/spec-impl run D:\GitHub\gateway\be\notes\spec\energy-meter-dashboard`

## 可用的 MCP 工具

| MCP | 工具 | 用途 |
|-----|------|------|
| greptile | `mcp__plugin_greptile_greptile__search_greptile_comments` | 語意搜尋，理解修改位置上下文、確認影響範圍 |
| serena | `mcp__plugin_serena_serena__find_symbol`, `mcp__plugin_serena_serena__replace_symbol_body` | Symbol-level 編輯，精準修改函數 |
| git | `Bash(git diff:*)`, `Bash(git status:*)` | 確認修改內容 |
| memory | `mcp__memory__search_nodes` | 取得之前分析的 spec 資訊 |

## 執行流程

### Step 1: 讀取 Spec

讀取 `{path}/spec.md` 檔案內容。

### Step 2: 建立 Todo List

從 spec 的 Implementation Steps 提取所有 checkbox 項目：

1. 解析所有 Phase 的 Implementation Steps
2. 提取每個 `- [ ]` checkbox 項目
3. 使用 TodoWrite 建立追蹤清單

### Step 3: 逐步實作

按照 Phase 順序執行：

**3.1 開始 Phase**
1. 將當前 todo 標記為 `in_progress`
2. 讀取該 Phase 的 Modification Context

**3.2 執行修改**
1. 從 checkbox 描述提取 file:line
2. 使用 Read tool 讀取該檔案
3. 使用 Edit tool 進行修改
4. 標記 todo 為 `completed`

**3.3 Phase 間過渡**
1. 確認該 Phase 所有 checkbox 已完成
2. 開始下一個 Phase

### Step 4: 驗證

1. 確認所有 todo 都為 `completed`
2. 如有測試，執行相關測試

## 完成後提示

```
實作完成 ✅

完成項目：
✅ Phase 1: {phase 名稱}
  - ✅ {checkbox 1}
  - ✅ {checkbox 2}

建議執行 /spec-impl review {path} 審查實作結果。
```

---

# 子指令 2: review

## 輸入

```
/spec-impl review [spec 資料夾路徑]
```

## 審查重點（實作結果）

### 1. 修改完成度檢查

對每個 implementation step 的 checkbox：

| 檢查項目 | 驗證方式 |
|---------|---------|
| 修改是否存在 | 檢查 file:line 附近是否有新增/修改的程式碼 |
| 修改是否正確 | 比對 spec 的 modification context 描述 |
| 邏輯是否完整 | 確認修改有處理所有必要的 case |

### 2. 功能驗證（如適用）

- 執行相關測試
- 確認新功能正常運作
- 確認沒有破壞現有功能

### 3. 程式碼品質檢查

參考專案的 coding standards：

| 檢查項目 | 參考文件 |
|---------|---------|
| JSDoc 註解 | `.claude/coding-standards.md` |
| Logger 使用 | `.claude/logger-standards.md` |
| Error Handling | 確認有適當的錯誤處理 |

## 輸出

`{spec-folder}/impl-review-report.md`

```markdown
# Implementation Review Report

**Spec**: {spec 名稱}
**審查日期**: {日期}
**審查類型**: 實作結果審查（implement 後）

---

## 審查結果摘要

- 修改完成度: X / Y (XX%)
- 程式碼品質: ✅ 通過 / ⚠️ 需改善
- 整體狀態: ✅ 通過 / ❌ 需修正

---

## 修改驗證結果

### Phase 1: {phase 名稱}

| Checkbox | file:line | 狀態 | 說明 |
|----------|-----------|------|------|
| {描述} | {位置} | ✅/⚠️/❌ | {說明} |

---

## 需修正項目

1. ❌ {問題描述}

---

## 改善建議

1. 💡 {建議}
```

## 完成後提示

**通過：**
```
實作審查完成 ✅

報告已存於 {path}/impl-review-report.md

所有修改都正確完成，符合 spec 要求。
```

**未通過：**
```
實作審查完成 ❌

報告已存於 {path}/impl-review-report.md

請根據報告中的問題進行修正。
```

---

## 注意事項

1. **run**: 嚴格按照 spec 執行，不要自行添加額外修改
2. **review**: 比對 spec 描述，以 modification context 為準
3. **遇到問題時停止**: 如果發現 spec 有問題，停止並通知用戶

---

## 錯誤處理

### 如果 file:line 不存在

```
⚠️ 錯誤：spec 中的 file:line 不存在

檔案: {file_path}
行數: {line_number}

建議：
1. 執行 /spec review {path} 重新審查 spec
2. 或手動修正 spec.md 中的 file:line
```

$ARGUMENTS
