# CLAUDE.md

## 專案概述

這是一個電力數據分析專案，前端使用 Vue，後端使用 .NET。

## 技術棧

### 前端
- Vue 3
- TypeScript
- Vite

### 後端
- .NET 10（LTS）
- C#
- Entity Framework Core

## 開發慣例

- 使用繁體中文撰寫面向使用者的文字與註解
- 前端遵循 Vue 3 Composition API 風格
- 後端遵循 .NET 慣例命名規則（PascalCase）
- API 採用 RESTful 設計

## 目錄結構

- `frontend/` - Vue 前端專案
- `backend/` - .NET 後端專案
- `collector/` - .NET Console App（收值 / C# 練習）
- `tests/` - 測試檔案

## 常用指令

### 前端
```bash
cd frontend
npm install
npm run dev
```

### 後端
```bash
cd backend
dotnet restore
dotnet run
```

### Collector
```bash
cd collector
dotnet run
```

## 注意事項

- 敏感設定（連線字串、API 金鑰等）不應提交至版本控制
- 使用 `.gitignore` 排除 `node_modules/`、`bin/`、`obj/` 等產出目錄
