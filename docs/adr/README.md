# ADR (Architecture Decision Records)

## What is an ADR?

An ADR documents **why** an architectural decision was made. It answers the question: "Why did we build it this way?" — something that cannot be determined from code alone.

ADR was introduced by Michael Nygard and is widely adopted at Netflix, Spotify, and ThoughtWorks.

## When to Write an ADR

Write an ADR:
- **After** a significant architectural decision is finalized
- When you need to record the rationale for a choice
- When the decision will impact future development
- When you want to prevent re-evaluating the same options

## MADR Template

This project follows **MADR** (Markdown Any Decision Records), which defines a canonical structure. We extend it with an additional **Implementation** section.

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

## MADR Section Guide

### 1. Status

**Intention:** Record the current lifecycle state of the decision so readers immediately know whether it is still authoritative.

A decision is not permanent. It may be accepted, later deprecated (the approach is no longer recommended but is still in place), or superseded by a newer ADR that overrides it. Without an explicit status, a reader cannot tell whether an old ADR still reflects reality or has quietly become obsolete. This single line prevents "zombie decisions" — rules that look authoritative but have already been replaced.

**Examples:**
```
Accepted
Deprecated
Superseded by ADR-003
```

### 2. Context

**Intention:** Capture the situation *at the time the decision was made* — the problem, constraints, and forces at play — so future readers can judge whether the same reasoning still applies.

Code shows you *what* was built. Context explains *why it had to be solved at all*. This includes the business or technical problem, the constraints that could not be changed (team size, existing infrastructure, deadline), and any trade-offs that were already given. Without context, a future engineer has no way to know whether the original constraints still hold — and therefore no way to know whether revisiting the decision is worth the effort.

### 3. Decision

**Intention:** State the concrete choice made and the reasoning behind it, including alternatives that were considered and rejected.

This is the core of the ADR. It must be specific enough that a new team member can read it and know exactly what was decided and what was ruled out. Listing rejected alternatives is critical: it prevents the team from repeatedly re-evaluating the same options, and it signals that the chosen approach was not the only one considered.

### 4. Consequences

**Intention:** Honestly account for the trade-offs introduced by the decision — what becomes easier, what becomes harder, and what new risks or constraints are now accepted.

No decision is free. Consequences make the cost visible so the team is not surprised later. Positive consequences confirm the value of the decision. Negative consequences serve as a maintenance checklist: they identify where technical debt was knowingly incurred and may point to future ADRs that will be needed to resolve it.

### 5. Implementation *(project extension)*

**Intention:** Bridge the gap between the decision (the "why") and the actual code (the "what") by recording exactly how the decision was put into practice.

The standard MADR template ends at Consequences. This project adds an Implementation section to answer: *how was this decision actually realized?* This includes the specific files created or modified, commands run, configuration applied, and any non-obvious steps taken. This section makes it easy to verify the decision was implemented correctly, to trace a piece of code back to its originating ADR, and to understand the scope of change when revisiting the decision later.

## RFC vs ADR

| | RFC | ADR |
|---|---|---|
| **When** | Before decision | After decision |
| **Goal** | Get feedback | Record history |
| **Question** | "Should we?" | "Why did we?" |

An RFC often **becomes** an ADR once the team agrees — the ADR is the final record of what the RFC concluded.

## Creating an ADR from an RFC

When an RFC is accepted, write a **new ADR** that references it:

**Example:**

```markdown
# ADR-003: Time Aggregation Implementation

## Status

Accepted

## Context

This implements RFC-001-time-aggregation-query.md, which proposed...

## Decision

We decided to use EF Core GroupBy for time aggregation because...
(Summarize the final decision from the RFC)

## Consequences

Positive:
- Type-safe queries
- Maintainable code

Negative:
- May need raw SQL fallback for complex queries

## Implementation

- Created `SensorDataController.GetAggregated()` endpoint
- Added time grouping logic in `SensorDataService`
- Tests added in `SensorDataControllerTests.cs`
```

**Key points:**
- ADR is more concise than the RFC
- Focus on the final decision and its consequences
- Reference the RFC for full proposal history
- The RFC stays in `rfc/` folder as historical context

## Naming Convention

```
ADR-{number}-{short-description}.md
```

Examples:
- `ADR-001-tech-stack.md`
- `ADR-002-orm-selection.md`

Numbers are zero-padded to 3 digits and sequential.
