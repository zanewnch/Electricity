---
name: codebase-search-guidance
description: 主動代碼搜索行為指南。當用戶問「在哪裡」「怎麼運作」時，主動用 Serena 工具搜索，找到重要信息後存入 Memory
---

# 主動代碼搜索指南

## 核心原則

**不要等用戶明確說「幫我搜索」，而是理解他們的問題意圖**

當用戶提出以下類型的問題時，**立即主動使用 Serena 工具進行搜索**：

```
用戶問題類型              你應該主動搜索
────────────────────────────────────────
"這個在哪裡？"            → 文件位置、類定義
"怎麼運作的？"            → 代碼流程、邏輯
"數據從哪裡來？"          → 數據來源、流向
"這個怎麼用？"            → 函數調用、使用方式
"系統架構是？"            → 整體流程、模塊關係
```

---

## 觸發關鍵詞

**強觸發詞**（一定要主動搜索）：
- "在哪"、"在哪裡"、"哪裡"
- "怎麼"、"如何"、"怎樣"
- "哪裡來"、"從哪"、"源"
- "定位"、"找到"、"位置"
- "架構"、"流程"、"運作"

**弱觸發詞**（可能需要搜索）：
- "是什麼"（涉及代碼時）
- "有哪些"（涉及具體實現時）
- "相關"、"關聯"

---

## 搜索策略（優先順序）

### 1️⃣ 明確的類名/函數名 → `serena find_symbol`

**觸發情況**：用戶明確指出某個類或函數名稱

**例子**：
```
用戶：「MqttMessageToCacheDispatcher 這個類在哪？」

你執行：
serena find_symbol('MqttMessageToCacheDispatcher', depth=1)
  ↓ 直接返回文件位置和方法列表
```

### 2️⃣ 模糊的架構/流程問題 → `Task(Explore, medium/very_thorough)`

**觸發情況**：用戶問流程、運作原理、數據流向

**例子**：
```
用戶：「modbus 數據在系統中怎麼運作的？」

你執行：
Task({
  subagent_type: 'Explore',
  description: '追蹤 modbus 數據的完整處理流程',
  prompt: '從 MQTT 接收到最終存儲，modbus 數據經過...'
})
```

**何時用 `medium` vs `very_thorough`**：
- `medium`：簡單問題（單個模塊、單個功能）
- `very_thorough`：複雜問題（跨模塊、完整架構）

### 3️⃣ 特定模式搜索 → `serena search_for_pattern`

**觸發情況**：要查找特定的代碼模式或概念

**例子**：
```
用戶：「系統中哪些地方在計算 kW？」

你執行：
serena search_for_pattern(
  substring_pattern='\\bkW\\b|compute.*[Kk]w|[Kk]w.*compute',
  relative_path='gateway-server'
)
```

### 4️⃣ 特定檔案搜索 → `serena read_file` + `serena search_for_pattern`

**觸發情況**：已知文件位置，但要在文件內找特定內容

**例子**：
```
用戶：「gwupdate 在 MqttMessageToCacheDispatcher.js 中怎麼處理？」

你執行：
serena search_for_pattern(
  substring_pattern='gwupdate|dispatchGW06CTMessage',
  relative_path='gateway-server/utils/mqtt/MqttMessageToCacheDispatcher.js'
)
```

---

## 完整工作流程

```
用戶提問
  ├─ 判斷問題類型
  │  ├─ 明確類名 → find_symbol
  │  ├─ 模糊流程 → Task(Explore)
  │  ├─ 模式查找 → search_for_pattern
  │  └─ 文件內查 → read_file + search
  │
  ↓
執行搜索，獲得結果
  │
  ├─ 簡單一次性問題
  │  └─ 直接回答用戶（不存 Memory）
  │
  └─ 重要知識點（未來會常用）
     ├─ add memory <entity> <observation>
     └─ 下次直接從 Memory 讀取 ✅
```

---

## 搜索後的下一步：存入 Memory

### 什麼情況下應該存 Memory？

✅ **應該存**：
- 複雜的架構理解（gwupdate vs emupdate）
- 系統設計決策（為什麼分三個表）
- 數據流向（CT → Cache → DB 的完整流程）
- 重複問題的答案（用戶可能常問）

❌ **不用存**：
- 簡單的一次性問題（「這行代碼什麼意思」）
- 代碼實現細節（除非涉及核心邏輯）
- 正在改動的臨時代碼

### 存 Memory 的方式

```bash
# 完整搜索後發現重要信息
add memory <entity_name> <observation>

# 例子
add memory modbus_data_processing_flow
  Modbus 數據來自 modbus topic
  → dispatchModbusMessage()
  → writeInCacheNewForModbus()
  → cacheBufferModbus (Map)
  → 定時器觸發 → gwStorageHandler 計算

# 下次用戶問類似問題
用戶：「modbus 怎麼處理的？」
你：直接讀 memory，或 read memory modbus_data_processing_flow
```

---

## 使用 Serena 的最佳實踐

### ✅ 好的搜索策略

1. **具體化問題**
   ```
   ❌ 「搜索 current」太廣
   ✅ 「find_symbol('writeInCacheNew')」有目標
   ```

2. **分層搜索**
   ```
   第1層：搜索文件位置
   第2層：搜索具體函數
   第3層：讀取函數實現
   ```

3. **結合 Context**
   ```
   不要盲目搜索，而是說出原因：
   「我要理解 CT 數據流向，搜索一下...」
   ```

### ❌ 常見錯誤

❌ **搜索太廣**
```
serena search_for_pattern('data')  // 太多結果
```

✅ **縮小範圍**
```
serena search_for_pattern(
  'BLEAddress.*Current',
  relative_path='gateway-server/model/gateWayCache.js'
)
```

---

## 與 Memory 的協作

**`.claude/skills/codebase-search.md`** (本檔案)
```
↓ 指導何時搜索、怎麼搜索
```

**`.serena/memory/`** (知識庫)
```
←  存儲已發現的重要信息
```

**流程**：
```
用戶問 「gwupdate 怎麼運作」
  ↓ 讀 codebase-search.md 指導
  ↓ 主動用 Task(Explore) 搜索
  ↓ 找到完整流程
  ↓ add memory gwupdate_ct_relationship
  ↓ 下次用戶問類似問題
  ↓ 直接從 memory 讀取 ✅
```

---

## 實戰例子

### 例子 1：用戶問「CT 在哪裡處理？」

```
用戶：「CT 數據在系統中哪裡被處理？」

步驟 1：讀 codebase-search.md → 這是架構問題
步驟 2：主動執行
  Task({
    subagent_type: 'Explore',
    description: '追蹤 CT 數據的完整處理流程',
    prompt: '從 gwupdate MQTT topic 開始，CT 數據...'
  })
步驟 3：找到完整流程（gwupdate → gwStorage → DB）
步驟 4：發現這是重要知識
  add memory ct_data_processing_flow
步驟 5：回答用戶，並提示「已記錄到 Memory」
```

### 例子 2：用戶問「MqttMessageToCacheDispatcher 在哪？」

```
用戶：「MqttMessageToCacheDispatcher 這個類在哪？」

步驟 1：讀 codebase-search.md → 明確的類名
步驟 2：主動執行
  serena find_symbol('MqttMessageToCacheDispatcher', depth=1)
步驟 3：返回：gateway-server/utils/mqtt/MqttMessageToCacheDispatcher.js
步驟 4：這是簡單一次性問題，不需要存 Memory
步驟 5：直接回答用戶
```

### 例子 3：用戶問「modbus 怎麼和 CT 區分？」

```
用戶：「系統中 modbus 和 CT 怎麼區分的？」

步驟 1：讀 codebase-search.md → 架構對比問題
步驟 2：檢查 memory（gwupdate_ct_relationship 已存）
步驟 3：查找 modbus 的類似信息
  Task(Explore) 搜索 modbus 數據流
步驟 4：找到區別，發現值得記錄
  add memory modbus_vs_ct_difference
步驟 5：回答用戶，展示兩套系統的區別
```

---

## 檔案位置參考

**常見搜索位置**：
```
MQTT 處理：
  gateway-server/utils/mqtt/MqttMessageToCacheDispatcher.js

快取管理：
  gateway-server/model/gateWayCache.js

One-Phase/Three-Phase/DC 配置：
  gateway-server/model/create-table-model.js
  gateway-server/routes/onePhase.js
  gateway-server/routes/threePhase.js
  gateway-server/routes/dcSettingNew.js

設備管理：
  gateway-server/model/onePhase.js
  gateway-server/model/threePhase.js
```

---

## 總結：何時主動搜索

| 情況 | 搜索工具 | 存 Memory？ |
|------|--------|----------|
| 用戶問「在哪」+ 明確類名 | find_symbol | ❌ |
| 用戶問「怎麼運作」 | Task(Explore) | ✅ |
| 用戶問「數據流向」 | Task(Explore, very_thorough) | ✅ |
| 用戶問「這個模式在哪」 | search_for_pattern | ❌ |
| 用戶問已知答案（在 Memory 中） | 直接 read_memory | ✅ |

---

**記住：主動搜索是提升效率的關鍵，不要等用戶明確要求！**
