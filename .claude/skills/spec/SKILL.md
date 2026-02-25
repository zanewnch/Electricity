---
name: spec
description: 功能規格文件管理。支援三個子指令：(1) create - 分析 frontend/backend 建立 spec 文件；(2) review - 審查 spec 完整性（implement 前）；(3) update - 修改 spec 內容並保持格式一致。當用戶說「建立 spec」「分析功能」「審查 spec」「review spec」「幫我 create spec」「幫我 review spec」「幫我改 spec」「修改 spec」「更新 spec」時觸發。
argument-hint: <create|review|update> [參數]
disable-model-invocation: false
user-invocable: true
model: opus
allowed-tools: Read, Grep, Glob, Write, Edit, Bash(mkdir:*)
---

# Spec - 功能規格文件管理

## 使用方式

```
/spec create [component/feature 名稱]   → 建立 spec 文件
/spec review [spec 資料夾路徑]          → 審查 spec（implement 前）
/spec update [spec 資料夾路徑]          → 修改 spec 內容
```

---

## 解析 $ARGUMENTS

```
$ARGUMENTS 格式：<子指令> [參數...]

子指令：
- create → 執行「建立 Spec」流程
- review → 執行「審查 Spec」流程
- update → 執行「修改 Spec」流程
```

---

# 子指令 1: create

## 輸入

```
/spec create [component 名稱或 feature 描述]
```

## 輸出位置

```
D:\GitHub\gateway\be\notes\spec\{feature-name}\spec.md
```

## 分析流程

使用 ultrathink 進行深度分析，確保 data workflow 每一步都正確串接。

### 可用的 MCP 工具

優先使用以下 MCP 工具來加速分析：

| MCP | 工具 | 用途 |
|-----|------|------|
| greptile | `mcp__plugin_greptile_greptile__search_greptile_comments` | 語意搜尋 codebase，找 data workflow 路徑 |
| serena | `mcp__plugin_serena_serena__find_symbol`, `mcp__plugin_serena_serena__find_referencing_symbols` | Symbol-level 程式碼分析，追蹤函數調用 |
| memory | `mcp__memory__create_entities`, `mcp__memory__search_nodes` | 記住分析結果供後續使用 |
| git | `Bash(git blame:*)`, `Bash(git log:*)` | 查看程式碼歷史和修改記錄 |

### 1. Frontend Analysis

- **Source Files**（必須標明）:
  - Component: `{path}/xxx.component.ts`
  - Service: `{path}/xxx.service.ts`
  - Model: `{path}/xxx.model.ts`（若有）
- **Intention**: component 的主要功能/目的
- **Request Paths**: service method + API 路徑對應表
- **UI Elements**: 與功能相關的 UI component

### 2. Backend Analysis

- **Source Files**（必須標明）:
  - Route: `gateway-server/routes/xxx.js`
  - Handler: `gateway-server/model/xxx.js`（若有）
  - Middleware: `gateway-server/model/xxx.js`（若有）
- **API Endpoints**: file:line number
- **DB Schema**: table name + columns
- **Data Workflow（完整追蹤）**:
  ```
  MQTT 取值 → file:line
      ↓
  dispatch 處理 → file:line
      ↓
  cache 存取 → file:line
      ↓
  storage 計算 → file:line
      ↓
  IPC 回傳 → file:line
      ↓
  handler 處理 → file:line
      ↓
  DB insert → file:line
  ```

### 3. Requirement Phases

每個 Phase 包含需求說明和程式碼變更：

```markdown
### 🟣 Phase N: 標題

**Requirement**: 具體需求

**Modification Context**:
- **為什麼改**：[原因]
- **怎麼改**：[方法]
- **影響範圍**：[範圍]

**程式碼變更**：
[程式碼區塊]
```

### 4. Implementation Steps（獨立區塊）

所有 Phase 的修改項目彙整成 checkbox 清單：

```markdown
## 🟣 Implementation Steps

- [ ] 修改 `{file:line}` - {簡短描述}
- [ ] 修改 `{file:line}` - {簡短描述}
```

## 完成後提示

```
Spec 已建立於 {path}/spec.md

建議執行 /spec review {path} 審查 spec 內容。
```

---

# 子指令 2: review

## 輸入

```
/spec review [spec 資料夾路徑]
```

## 核心原則

**直接修正，不只報告問題。**

發現問題時立即修正 spec 檔案，而非只列出問題清單詢問用戶是否修正。

## 審查重點（spec 內容本身）

### 1. 格式完整性

| 檢查項目 | 通過標準 |
|---------|---------|
| Index 目錄 | 存在且與標題同步 |
| 標題 Emoji | 所有主要區塊都有正確的 Emoji（🔵🟢🟣🟡🟠📄） |
| 相關文件區塊 | 存在且列出所有涉及的檔案 |

### 2. Analysis 完整性（根據 spec 類型）

**判斷 spec 類型後，只檢查存在的區塊：**

| Spec 類型 | 檢查項目 |
|----------|---------|
| 純 Frontend | Frontend Analysis 完整（intention、UI elements） |
| 純 Backend | Backend Analysis 完整（API endpoints、DB schema、Data Workflow） |
| Full-stack | 兩者都檢查 |

**注意：** 不存在的區塊不扣分，但存在的區塊必須完整。

### 3. file:line 驗證

- 使用 Read tool 檢查每個 file:line 是否真實存在
- 確認該行程式碼與描述相符
- 如果不存在，標記為 ❌

### 4. Phase 結構完整性

| 檢查項目 | 通過標準 |
|---------|---------|
| Requirement | 每個 Phase 有 `**Requirement**` |
| Modification Context | 每個 Phase 有為什麼改、怎麼改、影響範圍 |
| 程式碼變更 | 有具體的程式碼區塊（若適用） |

### 5. Implementation Steps 可執行性

| 檢查項目 | 通過標準 |
|---------|---------|
| 獨立區塊 | 存在 `## 🟣 Implementation Steps` 區塊 |
| Checkbox 格式 | 使用 `- [ ]` 格式 |
| file:line 參照 | 每個 checkbox 都有明確的 file:line |
| 描述清晰 | 具體可執行（不是模糊描述） |

## 執行流程

### Step 1: 審查 + 自動修正

發現以下問題時，**直接修改 spec.md**：

| 問題類型 | 自動修正行為 |
|---------|-------------|
| 行號錯誤 | 用 Read tool 找到正確行號，直接更新 |
| 變數命名不一致 | 依專案慣例修正 |
| 缺少區塊 | 補全缺少的結構 |
| 格式問題 | 直接格式化 |

### Step 2: 無法自動修正的問題

僅在以下情況詢問用戶：
- 邏輯矛盾（需求不明確）
- 找不到對應程式碼（可能需求已過時）

## 完成後提示

```
Spec 審查並修正完成 ✅

修正項目：
- [具體修正 1]
- [具體修正 2]

可執行 /spec-impl run {path} 開始實作。
```

---

# 子指令 3: update

## 輸入

```
/spec update [spec 資料夾路徑]
```

**路徑範例：**
- `be/notes/spec/data-correction`
- `be/notes/spec/energy-meter-setting`

**自動偵測：** 若用戶在 IDE 開啟了 spec 檔案，自動使用該路徑。

## 執行流程（全自動）

**重要：不詢問用戶，直接執行完整格式化流程。**

### Step 1: 讀取現有 Spec

```
讀取 {spec-folder}/spec.md 或 {spec-folder}/spec_v*.md
```

### Step 2: 分析現有內容結構

識別現有區塊：
- 有哪些標題/區塊？
- 哪些區塊缺少 Emoji？
- 是否有 Index 目錄？
- 是否有「相關文件」區塊？
- **Phase 結構是否完整？** 每個 Phase 應包含：
  - `**Requirement**`: 具體需求
  - `**Modification Context**`: 為什麼改、怎麼改、影響範圍
- **是否有獨立的 Implementation Steps 區塊？** checkbox 格式的可執行項目
- **判斷 Spec 類型：**
  - **純 Frontend**：只有 Frontend Analysis、沒有 Backend → 不補 Backend 區塊
  - **純 Backend**：只有 Backend Analysis → 不補 Frontend 區塊
  - **Full-stack**：兩者都有 → 保持完整

**可選區塊原則：** 不強制補全不存在的大區塊（Frontend/Backend Analysis），但必須補全已存在區塊的子結構。

### Step 3: 自動格式化（全部執行）

#### 3.1 轉換標題格式

將所有標題轉換為帶 Emoji 的格式：

| 關鍵字匹配 | Emoji | 轉換結果 |
|-----------|-------|---------|
| Frontend / 前端 / 驗證規則 / FormControl | 🔵 | `## 🔵 Frontend Analysis` |
| Backend / 後端 | 🟢 | `## 🟢 Backend Analysis` |
| Requirement / 需求 / Phase | 🟣 | `## 🟣 Requirement Phases` |
| Implementation / 實現 / 步驟 | 🟣 | `## 🟣 Implementation Steps` |
| 效果 / Result / Effect | 🟡 | `## 🟡 效果` |
| 資料結構 / Data Structure / Schema | 🟡 | `## 🟡 ...` |
| 測試 / Test | 🟠 | `## 🟠 測試` |
| 相關文件 / Related Files | 📄 | `## 📄 相關文件` |

**子標題規則：** 使用與父標題相同顏色

```markdown
## 🟣 Requirement Phases
### 🟣 Phase 1: 名稱長度限制    ← 同色
### 🟣 Implementation Steps      ← 同色
```

#### 3.2 生成 Index 目錄

根據轉換後的標題，自動生成可點擊的目錄：

```markdown
## 📑 Index

- [🔵 Frontend Analysis](#-frontend-analysis)
- [🟢 Backend Analysis](#-backend-analysis)
- [🟣 Requirement Phases](#-requirement-phases)
  - [Phase 1: 名稱長度限制](#phase-1-名稱長度限制)
- [📄 相關文件](#-相關文件)
```

#### 3.3 補全「相關文件」區塊

從 spec 內容中提取所有檔案路徑，生成：

```markdown
## 📄 相關文件

**Frontend:**
- Component: `{提取的 .component.ts 路徑}`
- Template: `{提取的 .component.html 路徑}`

**Backend:**
- Route: `{提取的 routes/*.js 路徑}`

**i18n:**
- `gateway-server-frontend/src/assets/i18n/*.json`
```

#### 3.4 補全 Phase 結構

檢查每個 `### Phase N:` 區塊，若缺少以下內容則補全：

```markdown
### 🟣 Phase N: 標題

**Requirement**: [從現有內容推斷或標記為待填寫]

**Modification Context**:
- **為什麼改**：[推斷或待填寫]
- **怎麼改**：[推斷或待填寫]
- **影響範圍**：[推斷或待填寫]

**程式碼變更**：
[保留現有的程式碼區塊]
```

#### 3.5 生成 Implementation Steps 區塊

從所有 Phase 中提取修改項目，生成獨立的 checkbox 清單：

```markdown
## 🟣 Implementation Steps

- [ ] 修改 `{file:line}` - {簡短描述}
- [ ] 修改 `{file:line}` - {簡短描述}
```

**提取規則：**
- 從 `**檔案**：` 或程式碼註解中提取 file:line
- 從 Phase 標題或 Requirement 提取描述
- 每個修改點一個 checkbox

#### 3.6 驗證 file:line 參照

使用 Read tool 驗證所有 `file:line` 格式的參照：

```
🔍 驗證 file:line 參照...

✅ equipment-group-setting.component.ts:54
✅ equipment-group-setting.component.html:26
❌ equipment-group-setting.component.ts:999（行號超出檔案範圍）
```

**驗證失敗處理：**
- 嘗試在檔案中搜尋相關程式碼
- 找到正確行號後自動修正
- 找不到則標記為 `❓ 待確認`

#### 3.7 格式一致性檢查

- [ ] 表格格式對齊
- [ ] 程式碼區塊語言標記正確
- [ ] file:line 格式統一（完整路徑:行號）
- [ ] 每個 Phase 都有 Requirement + Modification Context
- [ ] Implementation Steps 區塊存在且為 checkbox 格式

### Step 4: 輸出更新後的 Spec

**檔名規則：**
- 讀取 `spec.md` → 輸出覆蓋 `spec.md`
- 讀取 `spec_v2.md` → 輸出覆蓋 `spec_v2.md`（保持原檔名）
- 不建立 `.bak` 備份（用 git 管理版本）

## 完成後輸出

```
Spec 格式化完成 ✅

📝 修改摘要：
- 新增 Index 目錄
- 標題加入 Emoji（🔵🟢🟣📄）
- 補全 Phase 結構（Requirement + Modification Context）
- 生成 Implementation Steps 區塊（N 個 checkbox）
- 補全「相關文件」區塊
- 驗證 N 個 file:line 參照（N ✅ / N ❌）

📄 輸出檔案：{path}/spec.md

建議執行 /spec review {path} 確認完整性。
```

---

## 輸出格式範例

參考 [references/spec-output-format.md](references/spec-output-format.md)

---

## 注意事項

1. **Data workflow 必須完整** - 不能有斷點，每一步都要有 file:line
2. **file:line 驗證必須實際執行** - 用 Read tool 確認存在
3. **Implementation steps 要具體** - 不是模糊描述，而是可執行的項目

---

## Spec 格式規範

### 1. 📑 Index 目錄（必須）

Spec 開頭必須包含可點擊的目錄索引：

```markdown
## 📑 Index

- [🔵 Frontend Analysis](#-frontend-analysis)
  - [Source Files](#-source-files)
  - [Intention](#-intention)
  - [Request Paths](#-request-paths)
  - [UI Elements](#-ui-elements)
- [🟢 Backend Analysis](#-backend-analysis)
  - [Source Files](#-source-files-1)
  - [API Endpoints](#-api-endpoints)
  - [DB Schema](#-db-schema)
  - [Data Workflow](#-data-workflow)
- [🟣 Requirement Phases](#-requirement-phases)
- [📄 相關文件](#-相關文件)
```

### 2. 標題顏色 Emoji（必須）

使用純色圓球區分不同區塊，提升可讀性：

| 區塊 | Emoji | 範例 |
|------|-------|------|
| Frontend / 驗證規則 | 🔵 | `## 🔵 Frontend Analysis` |
| Backend | 🟢 | `## 🟢 Backend Analysis` |
| 效果 / 資料結構 | 🟡 | `## 🟡 效果` |
| Requirement / Implementation | 🟣 | `## 🟣 Requirement Phases` |
| 測試 | 🟠 | `## 🟠 測試` |
| 相關文件 | 📄 | `## 📄 相關文件` |

**子標題規則**：使用與父標題相同顏色

```markdown
## 🟢 Backend Analysis
### 🟢 API Endpoints      ← 同色
### 🟢 DB Schema          ← 同色
### 🟢 Data Workflow      ← 同色
```

**可用純色圓球**：🔴 🟠 🟡 🟢 🔵 🟣 🟤 ⚫ ⚪

### 3. 📄 相關文件區塊（必須）

每份 spec 結尾必須包含「相關文件」區塊，列出所有涉及的檔案路徑：

```markdown
## 📄 相關文件

**Frontend:**
- Component: `gateway-server-frontend/src/app/{module}/{component}.component.ts`
- Service: `gateway-server-frontend/src/app/{module}/@services/{service}.service.ts`
- Model: `gateway-server-frontend/src/app/{module}/@models/{model}.model.ts`
- Template: `gateway-server-frontend/src/app/{module}/{component}.component.html`

**Backend:**
- Route: `gateway-server/routes/{route}.js`
- Handler: `gateway-server/model/{handler}.js`
- Middleware: `gateway-server/model/{middleware}.js`
```

**用途：**
- ✅ 讓後續閱讀者快速定位程式碼
- ✅ 方便 IDE 點擊跳轉
- ✅ 明確定義修改範圍

$ARGUMENTS
