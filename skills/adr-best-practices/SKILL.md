---
name: adr-best-practices
description: ADR/MADR writing best practices and guidelines
---

# ADR Best Practices

This skill provides best practices for writing ADR (Architecture Decision Records) using the MADR (Markdown Any Decision Records) format.

## What is an ADR?

An ADR documents **why** an architectural decision was made. It answers: "Why did we build it this way?" — something that cannot be determined from code alone.

ADR was introduced by Michael Nygard and is widely adopted at Netflix, Spotify, and ThoughtWorks.

## When to Write an ADR

Write an ADR:
- **After** a significant architectural decision is finalized
- When you need to record the rationale for a choice
- When the decision will impact future development
- When you want to prevent re-evaluating the same options

## MADR Template

This project follows **MADR** (Markdown Any Decision Records) with an additional **Implementation** section.

```markdown
# ADR-{number}: {Title}

## Status

Accepted | Deprecated | Superseded by ADR-XXX

## Context

What is the issue we're facing? What constraints exist?

## Decision

What did we decide? What alternatives were rejected?

## Consequences

What becomes easier? What becomes harder? What trade-offs are accepted?

## Implementation

How was this decision actually realized in code?
```

## MADR Section Guidance

### 1. Status

**Purpose:** Record the current lifecycle state so readers immediately know if the decision is still authoritative.

A decision is not permanent. It may be accepted, later deprecated (approach no longer recommended but still in place), or superseded by a newer ADR. Without explicit status, readers cannot tell if an old ADR still reflects reality or has become obsolete. This prevents "zombie decisions" — rules that look authoritative but have been replaced.

**Examples:**
```
Accepted
Deprecated
Superseded by ADR-003
```

### 2. Context

**Purpose:** Capture the situation *at the time the decision was made* — the problem, constraints, and forces at play — so future readers can judge whether the same reasoning still applies.

Code shows you *what* was built. Context explains *why it had to be solved at all*. This includes the business or technical problem, constraints that could not be changed (team size, existing infrastructure, deadline), and trade-offs that were given. Without context, future engineers cannot know whether the original constraints still hold — and therefore cannot know whether revisiting the decision is worthwhile.

**Best Practices:**
- Record the business or technical problem
- Explain unchangeable constraints (team size, existing infrastructure, deadlines)
- Describe trade-offs considered at the time
- **Don't just record "what was done"** — record "**why it had to be solved**"

**Example:**
```markdown
## Context

We need to build an electricity data collection and visualization system consisting of:
a continuously running data collector, a RESTful API backend, a charting web frontend,
and a relational database.

The developer (personal project) has basic familiarity with the .NET and JavaScript
ecosystems and prefers mature, LTS technologies.
```

**Why this matters:**
- Future readers can judge if original constraints still exist
- If context changes (e.g., team size increases), decision can be re-evaluated

### 3. Decision

**Purpose:** State the concrete choice made and the reasoning behind it, including alternatives that were considered and rejected.

This is the core of the ADR. It must be specific enough that a new team member can read it and know exactly what was decided and what was ruled out. Listing rejected alternatives is critical: it prevents the team from repeatedly re-evaluating the same options, and it signals that the chosen approach was not the only one considered.

**Best Practices:**
- **Specific enough for new members to know what was chosen and what was excluded**
- List rejected alternatives
- Explain why this approach was chosen (technical advantages, fits constraints, reduces risk)
- Can use tables for comparison

**Example:**
```markdown
## Decision

Adopt the following tech stack:

| Layer | Technology | Version |
|-------|-----------|---------|
| Frontend | Vue 3 + TypeScript + Vite | Vue 3.5, TS 5.9, Vite 7 |
| Backend | ASP.NET Core Web API | .NET 10 (LTS) |
| ORM | Entity Framework Core | 10.0.3 |

Alternatives considered:

| Layer | Option | Reason Rejected |
|-------|--------|----------------|
| Frontend | React | More boilerplate; too heavy for this scale |
| Frontend | Svelte | Smaller ecosystem; fewer resources when issues arise |
| Backend | Node.js / Express | C# type system stricter for data processing |
| Database | PostgreSQL | More involved setup on Windows dev environment |
```

**Why this matters:**
- Prevents team from repeatedly re-evaluating same options
- Shows choice was not the only option, but was evaluated

### 4. Consequences

**Purpose:** Honestly account for trade-offs introduced by the decision — what becomes easier, what becomes harder, and what new risks or constraints are accepted.

No decision is free. Consequences make the cost visible so the team is not surprised later. Positive consequences confirm the value of the decision. Negative consequences serve as a maintenance checklist: they identify where technical debt was knowingly incurred and may point to future ADRs needed to resolve it.

**Best Practices:**
- **Divide into Positive and Negative**
- Positive consequences confirm decision value
- Negative consequences are a maintenance checklist: record technical debt, future problems that may need addressing
- **Be honest about costs**

**Example:**
```markdown
## Consequences

**Positive:**
- .NET + EF Core provides strongly-typed models, reducing runtime errors
- Vue 3 Composition API has clear structure, easy to split into components
- LocalDB requires zero configuration in development, enabling fast startup
- Collector and Backend can share models via `shared/` project

**Negative:**
- SQL Server LocalDB not suitable for production; migration needed later
- .NET 10 relatively new; some package ecosystems still catching up
- Frontend/backend separation requires handling CORS configuration
```

**Why this matters:**
- No decision is free, consequences make cost visible
- Negative consequences point to future ADRs (e.g., "ADR-XXX: Migrate to Production SQL Server")

### 5. Implementation (Project Extension)

**Purpose:** Bridge the gap between the decision (the "why") and the actual code (the "what") by recording exactly how the decision was put into practice.

The standard MADR template ends at Consequences. This project adds an Implementation section to answer: *how was this decision actually realized?* This includes specific files created or modified, commands run, configuration applied, and any non-obvious steps taken. This section makes it easy to verify the decision was implemented correctly, to trace a piece of code back to its originating ADR, and to understand the scope of change when revisiting the decision later.

**Best Practices:**
- Record specific files created or modified
- List commands executed
- Explain configuration applied
- Record non-obvious steps

**Example:**
```markdown
## Implementation

- Created `backend/` project: `dotnet new webapi`
- Installed EF Core: `dotnet add package Microsoft.EntityFrameworkCore`
- Created `frontend/` project: `npm create vite@latest frontend -- --template vue-ts`
- Configured CORS: modified `Program.cs` to add `builder.Services.AddCors()`
- Created `shared/` project to share models
```

**Why this matters:**
- Verify decision was correctly implemented
- Trace code fragments back to their source ADR
- Understand scope of change when re-evaluating decision later

## RFC to ADR Workflow

When an RFC is accepted, write an ADR:

1. **Reference RFC in Context**
```markdown
## Context

This implements RFC-001-time-aggregation-query.md, which proposed...
```

2. **Decision summarizes RFC's final decision**
- No need to repeat all RFC details
- Focus on final choice and reasoning

3. **Consequences extracted from Proposed Solution**
- Positive: Value achieved
- Negative: Technical debt accepted

4. **Implementation records actual execution**
- Created endpoints, services, tests
- Actual differences from RFC proposal (if any)

## Common Mistakes and Improvements

### Mistake 1: Context only records "what was done"

**Problem:** Future readers cannot judge if original problem still exists.

**Bad example:**
```markdown
## Context

We chose Vue 3 as frontend framework.
```

**Better example:**
```markdown
## Context

We need to build a data visualization frontend. Developer has basic Vue familiarity
and prefers mature frameworks with abundant resources. React has too much boilerplate,
Svelte has smaller ecosystem.
```

### Mistake 2: Decision lacks alternatives

**Problem:** Looks like other options weren't evaluated.

**Improvement:** List rejected options with rationale.

### Mistake 3: Consequences only lists positives

**Problem:** Not honest about technical debt.

**Improvement:** List negative impacts as future maintenance checklist.

### Mistake 4: Missing Implementation

**Problem:** Cannot trace code to decision.

**Improvement:** Record actual execution steps and files.

## ADR vs RFC

| | RFC | ADR |
|---|---|---|
| **When** | Before decision | After decision |
| **Goal** | Get feedback | Record history |
| **Question** | "Should we do this?" | "Why did we do this?" |
| **Sections** | Problem, Goals, Proposed Solution, Alternatives, Open Questions | Context, Decision, Consequences, Implementation |

**Workflow:**
1. Write RFC proposal
2. Team discussion and review
3. After RFC acceptance, write ADR
4. RFC preserved as history, ADR as final decision record

## Summary

A good ADR should:
- **Record context at decision time** (Context)
- **Clearly state chosen and rejected options** (Decision)
- **Be honest about trade-offs** (Consequences)
- **Bridge decision and code** (Implementation)

The goal is to let future self or team members understand "why this decision was made at that time" and judge whether re-evaluation is needed.
