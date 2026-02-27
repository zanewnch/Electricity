# VS Code Launch Configuration 說明

## Properties 速查表

| Property | 類型 | 說明 |
|----------|------|------|
| `type` | string | Debugger 類型（`node` = Node.js debugger） |
| `request` | string | `launch` = 啟動新 process / `attach` = 附加到已運行的 process |
| `name` | string | Debug 下拉選單顯示的名稱 |
| `program` | string | 要執行的入口檔案路徑 |
| `args` | string[] | 傳給 program 的命令列參數 |
| `console` | string | 輸出位置：`integratedTerminal` / `internalConsole` / `externalTerminal` |
| `skipFiles` | string[] | Debugger 跳過的檔案（不進入這些檔案的 breakpoint） |
| `port` | number | Attach 模式：要連接的 debugger port（預設 9229） |
| `continueOnAttach` | boolean | Attach 後自動繼續執行（不暫停在第一行） |

---

## 現有設定解析

### Debug Gateway Server

```jsonc
{
  "name": "Debug Gateway Server",
  "type": "node",
  "request": "launch",
  "program": "${workspaceFolder}/gateway-server/bin/www.js",
  "cwd": "${workspaceFolder}/gateway-server",
  "restart": true,
  "console": "integratedTerminal",
  "skipFiles": ["<node_internals>/**"]
}
```

**用途：** Debug 後端 Express server

**執行等效：**
```bash
cd gateway-server
node --inspect bin/www.js
```

**關鍵設定說明：**

| 設定 | 值 | 原因 |
|------|-----|------|
| `program` | `bin/www.js` | Express 入口（執行 `server.listen()`） |
| `cwd` | `gateway-server` | 確保 `process.cwd()` 能讀到 `setting.json` |
| `restart` | `true` | 檔案變更時自動重啟 |

**Workflow：**
1. `F5` 啟動 debugger
2. 在 routes/controllers 設 breakpoint
3. 前端發送 API 請求 → 觸發 breakpoint

---

## VS Code 變數參考

| 變數 | 展開結果 |
|------|----------|
| `${workspaceFolder}` | 專案根目錄（`D:\GitHub\gateway`） |
| `${relativeFile}` | 目前開啟檔案的相對路徑 |
| `${file}` | 目前開啟檔案的絕對路徑 |
| `${fileBasename}` | 目前開啟檔案的檔名（含副檔名） |

---

## 常用操作

| 操作 | 快捷鍵 |
|------|--------|
| 啟動 Debug | `F5` |
| 停止 Debug | `Shift + F5` |
| 重啟 Debug | `Ctrl + Shift + F5` |
| Step Over | `F10` |
| Step Into | `F11` |
| Step Out | `Shift + F11` |
| Continue | `F5` |

---

## 踩坑紀錄：Debug Gateway Server

### 最終正確設定

```jsonc
{
  "name": "Debug Gateway Server",
  "type": "node",
  "request": "launch",
  "program": "${workspaceFolder}/gateway-server/bin/www.js",  // ✅ 必須是 www.js
  "cwd": "${workspaceFolder}/gateway-server",                 // ✅ 必須設定 cwd
  "restart": true,
  "console": "integratedTerminal",
  "skipFiles": ["<node_internals>/**"]
}
```

---

### 踩坑 1：入口檔案錯誤（app.js vs bin/www.js）

**錯誤設定：**
```jsonc
"program": "${workspaceFolder}/gateway-server/app.js"  // ❌ 錯誤
```

**症狀：**
- Debugger 啟動成功，Redis/MySQL 連線正常
- `netstat -ano | findstr :3000` 沒有輸出（port 沒在監聽）
- 前端 proxy 報錯：`Error occurred while trying to proxy: localhost:4200/api/...`

**原因：**
Express 專案標準結構：
```
app.js     → 定義 Express app、routes、middleware（不啟動 server）
bin/www.js → require app.js + 執行 server.listen(port)
```

`npm run start` 腳本指向 `bin/www.js`，所以正常運行沒問題。
Debugger 直接啟動 `app.js` → 跳過 `server.listen()` → port 沒監聽。

**驗證方式：**
```bash
# 確認後端是否在監聽
netstat -ano | findstr :3000

# 有 LISTENING = 正常
# 空輸出 = 沒有監聽
```

---

### 踩坑 2：cwd 工作目錄錯誤

**錯誤設定：**
```jsonc
{
  "program": "${workspaceFolder}/gateway-server/bin/www.js"
  // 沒有設定 cwd → 預設是 ${workspaceFolder}
}
```

**症狀：**
- Debugger 啟動成功，port 3000 有在監聽
- 前端畫面空白
- DevTools Network：`/api/auth/check` 請求 pending（永遠不回應）
- DevTools 顯示 "Provisional headers are shown"

**原因：**
後端程式碼用 `process.cwd()` 讀取設定檔：

```javascript
// authMiddleware.js
const dbConfig = require(path.join(process.cwd(), "setting.json")).dbConfig;

// bin/www.js
setting = JSON.parse(fs.readFileSync(path.join(process.cwd(), 'setting.json')));
```

| 執行方式 | process.cwd() | setting.json 路徑 |
|----------|---------------|-------------------|
| `npm run start` | `gateway-server/` | `gateway-server/setting.json` ✅ |
| Debugger（無 cwd） | `D:\GitHub\gateway\` | `D:\GitHub\gateway\setting.json` ❌ |

`setting.json` 位於 `gateway-server/` 目錄，Debugger 找不到 → DB config 錯誤 → `queryPool.query()` 卡住 → API 永遠不回應。

**驗證方式：**
```javascript
// 在 app.js 開頭加入檢查
console.log('Current working directory:', process.cwd());
```

---

### 總結：兩個必要設定

| 設定 | 作用 | 缺少的後果 |
|------|------|------------|
| `program: bin/www.js` | 執行 server.listen() | Port 沒監聽 |
| `cwd: gateway-server` | 正確的 process.cwd() | 讀不到 setting.json |
