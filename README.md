# Electricity

Electricity data analysis project for analyzing power usage data, consumption statistics, and trend visualization.

## Tech Stack

- **Frontend**: Vue 3 + TypeScript + Vite
- **Backend**: .NET Core + C# + Entity Framework Core

## Getting Started

### Prerequisites

- Node.js 18+
- .NET 10 SDK

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

#### Development with Auto-Restart

For development, use `dotnet watch` to automatically restart the server when you save changes:

```bash
dotnet watch run
```

This will:
- Watch for file changes
- Automatically restart the server (1-2 seconds)
- Apply hot reload when possible for instant updates
- Perfect for rapid development iteration

### Collector (Console App)

```bash
cd collector
dotnet run
```

## Project Structure

```
Electricity/
├── README.md
├── CLAUDE.md
├── frontend/          # Vue 3 frontend project
├── backend/           # .NET Core backend project
└── collector/         # .NET Console App (data collection / C# practice)
```

## License

MIT
