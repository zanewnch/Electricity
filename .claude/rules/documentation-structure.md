# Documentation Structure Rules

## Overview

This project uses a structured approach to documentation. Each type of document has a specific location and purpose.

## Directory Structure

```
docs/
├── PRD.md                          # Product Requirements Document
├── DOCUMENTATION-GUIDE.md          # Documentation standards for this project
├── test-plan.md                    # Test strategy and plan
├── README.md                       # Docs overview
│
├── adr/                            # Architecture Decision Records
│   ├── README.md                   # ADR index and conventions
│   ├── ADR-001-tech-stack.md
│   ├── ADR-002-orm-selection.md
│   └── ... (numbering: ADR-001, ADR-002, ...)
│
├── rfc/                            # Request for Comments (design proposals)
│   ├── README.md                   # RFC index and conventions
│   ├── RFC-001-data-collection-strategy.md
│   ├── RFC-002-time-aggregation-query.md
│   └── ... (numbering: RFC-001, RFC-002, ...)
│
└── architecture/                   # System architecture & diagrams
    ├── c4-context.md               # C4 Context diagram (system overview & purpose)
    ├── c4-container.md             # C4 Container diagram
    └── c4-component.md             # C4 Component diagram
```

## Document Types & Rules

### RFC (Request for Comments)
- **Location**: `docs/rfc/`
- **Numbering**: RFC-001, RFC-002, RFC-003, ... (start from 001)
- **Purpose**: Design proposals & technical solutions BEFORE implementation
- **When to write**: Before deciding on implementation approach
- **Example topics**:
  - How should we aggregate time-series data?
  - What data collection strategy should we use?
  - How should we handle real-time updates?

### ADR (Architecture Decision Record)
- **Location**: `docs/adr/`
- **Numbering**: ADR-001, ADR-002, ADR-003, ... (start from 001)
- **Purpose**: Record WHY a decision was made and what were the alternatives
- **When to write**: AFTER a decision is accepted
- **Example topics**:
  - Why we chose Vue 3 (not React/Svelte)
  - Why we chose Entity Framework Core (not Dapper)
  - Why we chose SQL Server (not PostgreSQL)

### C4 Diagrams
- **Location**: `docs/architecture/c4-*.md`
- **Purpose**: Visual representation of system structure at different levels

**C4-Context** (`c4-context.md`):
- Shows the system in context of external systems/users
- Explains overall system purpose and validation goals
- Describes why each component exists (Collector, Backend, Frontend)
- Includes high-level data flow
- References relevant RFCs and ADRs

**C4-Container** (`c4-container.md`):
- Major containers/services within the system

**C4-Component** (`c4-component.md`):
- Components within a container

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

1. ✅ RFCs explain the "how" (technical design)
2. ✅ ADRs explain the "why" (decision rationale)
3. ✅ Architecture docs explain the "what" (overall system)
4. ❌ Do NOT put architecture overview in CLAUDE.md
5. ❌ Do NOT start numbering from 0 (use 001)
6. ❌ Do NOT create RFC-002 before RFC-001 is complete

## References

- [Claude Code Rules Documentation](https://code.claude.com/docs/en/overview)
- [Claude Code Rules Directory Guide](https://claudefa.st/blog/guide/mechanics/rules-directory)
- [What is .claude/rules/](https://claudelog.com/faqs/what-are-claude-rules/)
