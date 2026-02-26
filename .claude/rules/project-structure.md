# Project Structure

## Overview

This is an electricity data analysis project. The repository is organized as follows:

## Directory Structure

```
Electricity/
├── frontend/                       # Vue 3 + TypeScript + Vite
├── backend/                        # .NET 10 Web API (C# + EF Core)
├── collector/                      # .NET Console App (data collection / C# practice)
├── shared/                         # Shared models and DbContext
├── tests/                          # Test files
│
├── docs/
│   ├── PRD.md                      # Product Requirements Document
│   ├── DOCUMENTATION-GUIDE.md      # Documentation standards for this project
│   ├── test-plan.md                # Test strategy and plan
│   ├── README.md                   # Docs overview
│   │
│   ├── adr/                        # Architecture Decision Records
│   │   ├── README.md               # ADR index and conventions
│   │   ├── ADR-001-tech-stack.md
│   │   ├── ADR-002-orm-selection.md
│   │   └── ... (numbering: ADR-001, ADR-002, ...)
│   │
│   ├── rfc/                        # Request for Comments (design proposals)
│   │   ├── README.md               # RFC index and conventions
│   │   ├── RFC-001-data-collection-strategy.md
│   │   ├── RFC-002-time-aggregation-query.md
│   │   └── ... (numbering: RFC-001, RFC-002, ...)
│   │
│   └── architecture/               # System architecture & diagrams
│       ├── c4-context.md           # C4 Context diagram
│       ├── c4-container.md         # C4 Container diagram
│       └── c4-component.md         # C4 Component diagram
│
├── CLAUDE.md                       # Project instructions for Claude Code
└── .claude/
    └── rules/                      # Auto-loaded rules (this directory)
```

## Development Conventions

- Write user-facing text and comments in English
- Frontend follows Vue 3 Composition API style
- Backend follows .NET naming conventions (PascalCase)
- API follows RESTful design

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

## Document Types & Rules

### RFC (Request for Comments)
- **Location**: `docs/rfc/`
- **Numbering**: RFC-001, RFC-002, RFC-003, ... (start from 001)
- **Purpose**: Design proposals & technical solutions BEFORE implementation
- **When to write**: Before deciding on implementation approach

### ADR (Architecture Decision Record)
- **Location**: `docs/adr/`
- **Numbering**: ADR-001, ADR-002, ADR-003, ... (start from 001)
- **Purpose**: Record WHY a decision was made and what were the alternatives
- **When to write**: AFTER a decision is accepted

### C4 Diagrams
- **Location**: `docs/architecture/c4-*.md`
- **Purpose**: Visual representation of system structure at different levels
  - **C4-Context**: System in context of external systems/users
  - **C4-Container**: Major containers/services within the system
  - **C4-Component**: Components within a container

### PRD (Product Requirements Document)
- **Location**: `docs/PRD.md` (single file)
- **Purpose**: Define functional/non-functional requirements, success metrics
- **When to write**: At project start or when adding major features

## Numbering Convention

**RFC and ADR numbering:**
- Start from **001** (not 0)
- Use three digits with zero-padding: RFC-001, RFC-002, etc.
- Number sequentially as new documents are added
- Do NOT skip numbers or reorder

## Key Rules

1. RFCs explain the "how" (technical design)
2. ADRs explain the "why" (decision rationale)
3. Architecture docs explain the "what" (overall system)
4. Do NOT put architecture overview in CLAUDE.md
5. Do NOT start numbering from 0 (use 001)
6. Do NOT create RFC-002 before RFC-001 is complete
