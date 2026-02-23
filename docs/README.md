# docs/

This folder contains all specification and design documents for the Electricity project.

## Why This Structure

This documentation structure is modelled after how large engineering organizations manage their specs and decisions.

- **RFC** is used by Google, Meta, Rust, and Sourcegraph to propose and review significant engineering changes before writing code. It creates a record of the reasoning, not just the outcome. RFC has no single official standard — different companies converge on similar sections (Problem, Goals, Proposed Solution, Alternatives, Open Questions) but the format varies.
- **ADR** (Architecture Decision Records) was introduced by Michael Nygard and is widely adopted at Netflix, Spotify, and ThoughtWorks. It answers "why did we build it this way?" — a question that is impossible to answer from code alone. ADR has the most standardized format through **MADR** (Markdown Any Decision Records), which defines a canonical structure of `Status / Context / Decision / Consequences`.
- **C4 Model** was created by Simon Brown and is used across many enterprise teams as a lightweight, structured way to describe system architecture at four levels of detail without requiring a specific tool. There are two common ways to write C4 diagrams — **Structurizr DSL** and **Mermaid**. Structurizr DSL is the professional choice: it is purpose-built for C4, supports all four levels, and can generate multiple views from a single model file. However, it requires a dedicated renderer (Structurizr CLI or a local server), which adds tooling overhead. Mermaid, while not C4-native, is widely adopted, renders natively in GitHub and VS Code without any extra setup, and is sufficient for the Context / Container / Component levels used here. This project uses **Mermaid** for its zero-friction rendering.
- **OpenAPI** is an industry-standard spec (backed by Linux Foundation) used by Stripe, Twilio, and virtually every major API provider to formally define and document APIs in a machine-readable format.

The goal of adopting these formats — even for a personal project — is to practice the same habits used in professional engineering teams, where documentation is treated as a first-class deliverable alongside code.

## Structure

```
docs/
├── README.md               # This file
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

| Type | Purpose | Who writes it | When |
|------|---------|---------------|------|
| **PRD** | What to build and why | Product / developer | Before development starts |
| **RFC** | How to build a specific feature | Engineer | Before implementation, reviewed by team |
| **ADR** | Why a decision was made | Engineer | After a significant decision is finalized |
| **Architecture** | What the system looks like (C4 model) | Engineer | Alongside system design |
| **OpenAPI** | Formal API contract | Engineer | Before or during API implementation |
| **Test Plan** | Test strategy and coverage goals | Engineer | Before testing begins |

## Naming Conventions

- RFC: `RFC-{number}-{short-description}.md` — e.g. `RFC-001-time-aggregation-query.md`
- ADR: `ADR-{number}-{short-description}.md` — e.g. `ADR-001-tech-stack.md`
- Numbers are zero-padded to 3 digits and sequential

## Status Labels

### RFC
```
- [ ] Draft
- [ ] Under Review
- [x] Accepted
- [ ] Rejected
```

### ADR (MADR format)
```
Accepted | Deprecated | Superseded by ADR-XXX
```
