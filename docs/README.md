# docs/

This folder contains all specification and design documents for the Electricity project.

> 📖 For detailed explanations of documentation practices, see [DOCUMENTATION-GUIDE.md](DOCUMENTATION-GUIDE.md)

## Structure

```
docs/
├── README.md               # This file
├── DOCUMENTATION-GUIDE.md  # Detailed guide to documentation practices
├── PRD.md                  # Product Requirements Document
├── test-plan.md            # Test strategy and coverage expectations
│
├── rfc/                    # Engineering proposals (How to build it)
│   └── RFC-001-*.md
│
├── adr/                    # Architecture Decision Records (Why we decided X)
│   └── ADR-001-*.md
│
├── architecture/           # System diagrams (What it looks like)
│   ├── c4-context.md       # Level 1: System and external actors
│   ├── c4-container.md     # Level 2: Frontend, Backend, DB, Collector
│   └── c4-component.md     # Level 3: Internal components per container
│
└── api/
    └── openapi.yaml        # API contract (OpenAPI 3.0)
```

## Document Types

| Type | Purpose | When |
|------|---------|------|
| **PRD** | What to build and why | Before development starts |
| **RFC** | How to build a specific feature | Before implementation |
| **ADR** | Why a decision was made | After a decision is finalized |
| **Architecture** | What the system looks like (C4 model) | Alongside system design |
| **OpenAPI** | Formal API contract | Before/during API implementation |
| **Test Plan** | Test strategy and coverage goals | Before testing begins |

## Naming Conventions

- RFC: `RFC-{number}-{short-description}.md` — e.g., `RFC-001-time-aggregation-query.md`
- ADR: `ADR-{number}-{short-description}.md` — e.g., `ADR-001-tech-stack.md`
- Numbers are zero-padded to 3 digits and sequential

## Status Labels

**RFC:**
```
- [ ] Draft
- [ ] Under Review
- [x] Accepted
- [ ] Rejected
```

**ADR (MADR format):**
```
Accepted | Deprecated | Superseded by ADR-XXX
```

## Quick Reference

| | RFC | ADR |
|---|---|---|
| **When** | Before decision | After decision |
| **Goal** | Get feedback | Record history |
| **Question** | "Should we?" | "Why did we?" |
