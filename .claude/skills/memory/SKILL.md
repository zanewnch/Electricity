---
name: memory
description: 專案記憶管理（Serena）。存檔到 .serena/memory/ 目錄。支援 add/list/remove/search 操作。當用戶說「記住」「add memory」「add」「delete memory」「delete」「update memory」「update」「list memory」「列出記憶」「搜尋記憶」時觸發。
argument-hint: <add|delete|update|list|remove|search> [參數]
disable-model-invocation: false
user-invocable: true
model: haiku
allowed-tools: mcp__plugin_serena_serena__write_memory,mcp__plugin_serena_serena__read_memory,mcp__plugin_serena_serena__list_memories,mcp__plugin_serena_serena__delete_memory,mcp__plugin_serena_serena__edit_memory
---

# Memory - 專案記憶管理

## 核心機制

**儲存位置**：`.serena/memory/` 目錄，可直接查看和編輯。

**命名規則**：檔案名自動加上 `gateway_` 前綴（避免多專案混淆）。

---

## 使用方式

**自然語言式：**
```
add memory <entity> <observation>    → 新增記憶
delete memory <entity>               → 刪除記憶
update memory <entity> <observation> → 更新記憶（同 add）
list memory                          → 列出本專案所有記憶
search memory <query>                → 搜尋記憶
```

**傳統式（仍支持）：**
```
/memory add <entity> <observation>   → 新增記憶
/memory list                         → 列出本專案所有記憶
/memory remove <entity>              → 刪除記憶
/memory search <query>               → 搜尋記憶
```

---

## 解析 $ARGUMENTS

```
$ARGUMENTS 格式：<子指令> [參數...]

子指令：
- add       → 新增或更新記憶
- update    → 同 add（更新記憶）
- delete    → 刪除記憶（同 remove）
- remove    → 刪除記憶（向後相容）
- list      → 列出當前專案記憶
- search    → 搜尋記憶
```

---

# 子指令 1: add

## 輸入

```
add memory <memory_name> <content>
```

## 執行邏輯

1. 解析記憶名稱和內容
2. 自動加上專案前綴：`gateway_<memory_name>`
3. 檢查記憶檔案是否已存在：
   - 存在 → 使用 `mcp__plugin_serena_serena__edit_memory` 更新
   - 不存在 → 使用 `mcp__plugin_serena_serena__write_memory` 建立
4. 檔案儲存在 `.serena/memory/gateway_<memory_name>.md`

## 範例

```
用戶：add memory MQTT_Config 使用 topic filter 模式，QoS=1
執行：建立檔案 .serena/memory/gateway_MQTT_Config.md
```

---

# 子指令 2: list

## 輸入

```
list memory
```

## 執行邏輯

1. 使用 `mcp__plugin_serena_serena__list_memories` 列出所有記憶檔案
2. 篩選出前綴為 `gateway_` 的檔案
3. 讀取檔案並格式化輸出

## 輸出格式

```markdown
## Gateway 記憶檔案

| 檔案名 | 內容預覽 |
|--------|---------|
| gateway_MQTT_Config | 使用 topic filter 模式，QoS=1 |
| gateway_Cache_Rules | TTL=3600 秒，支援部分更新 |
```

**路徑：** `.serena/memory/gateway_*.md`

---

# 子指令 3: update

## 輸入

```
update memory <memory_name> <new_content>
```

## 執行邏輯

1. 查詢檔案 `.serena/memory/gateway_<memory_name>.md`
2. 使用 `mcp__plugin_serena_serena__edit_memory` 替換內容
3. 若檔案不存在，建立新檔案

## 範例

```
用戶：update memory MQTT_Config 已改用 QoS=2，部分更新
執行：覆蓋 .serena/memory/gateway_MQTT_Config.md 的內容
```

---

# 子指令 4: delete

## 輸入

```
delete memory <memory_name>
```

## 執行邏輯

1. 自動加上專案前綴：`gateway_<memory_name>`
2. 使用 `mcp__plugin_serena_serena__delete_memory` 刪除檔案
3. 檔案從 `.serena/memory/` 中移除

## 範例

```
用戶：delete memory OLD_MQTT_CONFIG
執行：刪除 .serena/memory/gateway_OLD_MQTT_CONFIG.md
```

---

# 子指令 5: remove（向後相容）

## 輸入

```
remove memory <memory_name>
```

## 執行邏輯

同 `delete`。

---

# 子指令 6: search

## 輸入

```
search memory <query>
```

## 執行邏輯

1. 列出 `.serena/memory/` 中所有 `gateway_*` 檔案
2. 在檔案內容中搜尋關鍵字
3. 返回匹配的檔案名和內容片段

## 範例

```
用戶：search memory cache
執行：搜尋所有 gateway_*.md 檔案中包含 "cache" 的檔案
返回：
  - gateway_Cache_Rules.md: TTL=3600...
  - gateway_MQTT_Config.md: Cache policy...
```

---

## 檔案位置與管理

**儲存位置**：`.serena/memory/`

**檔案命名**：`gateway_<memory_name>.md`

**直接訪問**：可在 IDE 中直接查看、編輯或刪除 `.serena/memory/` 中的檔案

## 搜尋技巧

```bash
# 在 terminal 中搜尋所有 gateway 記憶
find .serena/memory -name "gateway_*" -type f

# 搜尋特定關鍵字
grep -r "MQTT" .serena/memory/gateway_*.md
```

---

$ARGUMENTS
