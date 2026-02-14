# Electricity

電力數據分析專案，用於分析電力使用數據、用電量統計與趨勢視覺化。

## 技術棧

- **前端**：Vue 3 + TypeScript + Vite
- **後端**：.NET Core + C# + Entity Framework Core

## Getting Started

### Prerequisites

- Node.js 18+
- .NET 10 SDK

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

### Collector（Console App）

```bash
cd collector
dotnet run
```

## Project Structure

```
Electricity/
├── README.md
├── CLAUDE.md
├── frontend/          # Vue 3 前端專案
├── backend/           # .NET Core 後端專案
└── collector/         # .NET Console App（收值 / C# 練習）
```

## License

MIT
