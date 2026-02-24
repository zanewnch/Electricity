# CLAUDE.md

## Project Overview

This is an electricity data analysis project with a Vue frontend and a .NET backend.

## Tech Stack

### Frontend
- Vue 3
- TypeScript
- Vite

### Backend
- .NET 10 (LTS)
- C#
- Entity Framework Core

## Development Conventions

- Write user-facing text and comments in English
- Frontend follows Vue 3 Composition API style
- Backend follows .NET naming conventions (PascalCase)
- API follows RESTful design

## Directory Structure

- `frontend/` - Vue frontend project
- `backend/` - .NET backend project
- `collector/` - .NET Console App (data collection / C# practice)
- `tests/` - Test files

## Common Commands

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

### Collector
```bash
cd collector
dotnet run
```

## Notes

- Sensitive settings (connection strings, API keys, etc.) should not be committed to version control
- Use `.gitignore` to exclude `node_modules/`, `bin/`, `obj/`, and other build output directories
