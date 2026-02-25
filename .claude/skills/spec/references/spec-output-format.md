# Spec 輸出格式範例

以下是 spec.md 的標準輸出格式：

---

# {Feature Name} Spec

## Overview

**Feature**: {feature 名稱}
**Component**: {frontend component 名稱}
**Date**: {建立日期}

---

## Frontend Analysis

### 1. Intention（目的）

{component} 的主要功能是：
- {功能描述 1}
- {功能描述 2}

### 2. Request Paths（API 呼叫）

| Service 檔案 | Request Path | 用途 |
|-------------|--------------|------|
| `fe/src/app/services/xxx.service.ts:42` | GET `/api/xxx` | 取得 xxx 資料 |
| `fe/src/app/services/xxx.service.ts:56` | POST `/api/xxx` | 新增 xxx |

### 3. UI Elements（介面元素）

| Element | 位置 | 顯示內容 |
|---------|------|----------|
| `<app-energy-display>` | `fe/src/app/components/xxx.component.html:24` | 顯示 kW 數值 |
| `<mat-table>` | `fe/src/app/components/xxx.component.html:48` | 列表資料 |

---

## Backend Analysis

### 1. API Endpoints

| Route 檔案 | Handler 檔案 | HTTP Method + Path |
|-----------|-------------|-------------------|
| `be/routes/api.js:128` | `be/controller/xxxController.js:45` | GET `/api/xxx` |
| `be/routes/api.js:135` | `be/controller/xxxController.js:89` | POST `/api/xxx` |

### 2. DB Schema

#### Table: `energy_meter_data`

| Column | Type | Description |
|--------|------|-------------|
| id | INT | Primary Key |
| device_id | VARCHAR(50) | 設備 ID |
| kw_value | DECIMAL(10,2) | 即時功率 |
| timestamp | DATETIME | 時間戳記 |

### 3. Data Workflow（完整追蹤）

```
1. MQTT 取值
   └─ be/mqtt.js:156 - 訂閱 topic "/device/+/energy"
         ↓
2. dispatch 處理
   └─ be/mqtt.js:178 - dispatchMessage() 解析 payload
         ↓
3. cache 存取
   └─ be/cache/energyCache.js:34 - setDeviceData() 暫存最新值
         ↓
4. storage 計算
   └─ be/storage/energyStorage.js:67 - calculateAggregation() 計算彙總
         ↓
5. IPC 回傳
   └─ be/ipc/ipcHandler.js:89 - sendToMain() 傳送至主進程
         ↓
6. handler 處理
   └─ be/handler/energyHandler.js:112 - processEnergyData() 格式化
         ↓
7. DB insert
   └─ be/model/energyModel.js:45 - insertEnergyData() 寫入資料庫
```

---

## Requirement Phases

### Phase 1: {階段名稱}

**Requirement**:
{具體需求描述}

**Modification Context**:
- **為什麼改**: {改動原因}
- **怎麼改**: {改動方式}
- **影響範圍**: {受影響的檔案/功能}

**Implementation Steps**:
- [ ] 修改 `be/handler/energyHandler.js:112` - 新增 energy meter 資料處理
- [ ] 修改 `be/model/energyModel.js:45` - 新增 insert 欄位
- [ ] 修改 `fe/src/app/components/xxx.component.ts:34` - 新增顯示邏輯
- [ ] 修改 `fe/src/app/components/xxx.component.html:24` - 新增 UI element

---

### Phase 2: {階段名稱}

**Requirement**:
{具體需求描述}

**Modification Context**:
- **為什麼改**: {改動原因}
- **怎麼改**: {改動方式}

**Implementation Steps**:
- [ ] {修改項目 1}（含 file:line）
- [ ] {修改項目 2}（含 file:line）

---

## Notes

- {注意事項 1}
- {注意事項 2}
