---
name: rfc-best-practices
description: RFC writing best practices and guidelines
---

# RFC Best Practices

This skill provides best practices for writing RFC (Request for Comments) documents.

## What is an RFC?

An RFC is used to propose and review significant engineering changes **before writing code**. It creates a record of the reasoning, not just the outcome.

RFC is used by companies like Google, Meta, Rust, and Sourcegraph to ensure engineering decisions are well-thought-out and reviewed before implementation.

## When to Write an RFC

Write an RFC when:
- You're proposing a new feature or significant change
- The implementation approach needs team input
- There are multiple valid solutions to consider
- The decision will have long-term architectural impact

## RFC Section Guidance

### 1. Status

**Purpose:** Track RFC lifecycle state so readers immediately know if the proposal is still authoritative.

**Format:**
```markdown
- [ ] Draft           # Initial writing, not ready for review
- [ ] Under Review    # Ready for team feedback
- [ ] Accepted        # Approved, ready to implement
- [ ] Rejected        # Not moving forward
```

**Best Practices:**
- At least one status must be checked
- Draft stage can have incomplete sections
- Ensure all sections are complete before moving to Under Review

### 2. Problem

**Purpose:** Clearly describe the problem being solved and why this RFC is needed.

**Best Practices:**
- Describe specific pain points or requirements
- Explain shortcomings of current state
- Quantify the problem when possible (performance data, user feedback)

**Example:**
```markdown
## Problem

The current system collects data at a frequency of one record every 3.5 seconds.
The raw data volume is large, and directly charting it on the frontend has two problems:
1. Excessive data transfer volume affects performance
2. Data points are too dense, making trends difficult to read

An aggregation layer is needed to verify whether "electricity usage trends grouped
by minute/hour/day/month can reveal meaningful patterns."
```

### 3. Goals

**Purpose:** Clearly define what the solution must achieve.

**Best Practices:**
- Use verifiable statements
- Prioritize the most important goals
- Keep goals specific and measurable

**Example:**
```markdown
## Goals

- Add API endpoint to support time aggregation queries
- Support four granularities: `minute` / `hour` / `day` / `month`
- Return avgWatt, maxWatt, minWatt, sumWatt, and count for each time period
- Support filtering by device type
```

### 4. Non-Goals

**Purpose:** Clearly define what is **not** in scope to avoid scope creep.

**Best Practices:**
- **Should rarely be empty** - think carefully about scope boundaries
- Explain why related features are not being handled in this phase
- Can point to future expansion directions

**Example:**
```markdown
## Non-Goals

- Pre-calculation or caching of aggregated results (current data volume doesn't require it)
- Frontend chart implementation (handled separately)
- kWh conversion (frontend converts as needed)
```

**Anti-pattern to avoid:**
```markdown
## Non-Goals

None
```
(Too vague, hasn't thought through scope boundaries)

### 5. Proposed Solution

**Purpose:** Describe in detail how to solve the problem.

**Best Practices:**
- Provide enough detail for the team to evaluate
- Include API design, data flow, architecture diagrams (if needed)
- Code examples to illustrate key implementation points
- Consider performance, security, maintainability

**Example:**
```markdown
## Proposed Solution

### Endpoint Design

\`\`\`
GET /api/sensor-data/aggregate?granularity=hour&deviceType=EnergyMeter
\`\`\`

### Implementation Approach: EF Core GroupBy

\`\`\`csharp
var grouped = granularity switch
{
    "hour" => query.GroupBy(x => new {
        x.Timestamp.Year, x.Timestamp.Month,
        x.Timestamp.Day, x.Timestamp.Hour
    }),
    // ...
};
\`\`\`
```

### 6. Alternatives Considered

**Purpose:** Prove the chosen approach was evaluated, prevent team from repeatedly evaluating same options.

**Best Practices:**
- List rejected options
- Explain why rejected (pros/cons comparison)
- Use tables for clear comparison

**Example:**
```markdown
## Alternatives Considered

| Option | Pros | Cons |
|------|------|------|
| EF Core GroupBy (Recommended) | Type-safe, maintainable | Complex GroupBy may not fully translate to SQL |
| Raw SQL | SQL behavior explicit and controllable | Requires parameterized queries; poor cross-database portability |
| Frontend aggregation | No backend changes needed | Excessive data transfer, not viable |
```

### 7. Open Questions

**Purpose:** Record items that are still uncertain or need verification.

**Best Practices:**
- Use checkboxes to track resolution status
- Specifically describe what needs verification
- Update or remove when questions are resolved
- Use "None" when all resolved

**Example:**
```markdown
## Open Questions

- [ ] Can EF Core `DateTime.GroupBy` correctly translate to SQL Server `DATEPART`? (Requires testing)
- [ ] Do aggregation results need pagination? (Month granularity has at most 12 records, likely not needed)
```

**When resolved:**
```markdown
## Open Questions

None
```

### 8. References

**Purpose:** Link related documents for complete context.

**Best Practices:**
- Link related ADR, PRD, other RFCs
- Use relative paths
- Use "None" if no related documents

**Example:**
```markdown
## References

- [PRD — F3 Time Aggregation Trend Analysis](../PRD.md)
- [ADR-002 — ORM Selection](../adr/ADR-002-orm-selection.md)
- [api/openapi.yaml — aggregate endpoint definition](../api/openapi.yaml)
```

## RFC to ADR Conversion Workflow

When an RFC is accepted:

1. **Keep RFC file** in `docs/rfc/` as proposal history
2. **Write new ADR** in `docs/adr/`
3. ADR content:
   - Context: Mention "This implements RFC-{number}"
   - Decision: Summarize RFC's final decision
   - Consequences: Extract from Proposed Solution
   - Implementation: Record actual execution steps

**Why keep them separate?**
- RFC records **proposal process**: discussions, alternatives, open questions
- ADR records **final decision**: what was chosen, why, consequences
- Separate preservation has higher historical value

## Common Mistakes and Improvements

### Mistake 1: Empty Non-Goals

**Problem:** Hasn't thought through scope boundaries, prone to scope creep.

**Improvement:** Explicitly list related features that won't be handled.

### Mistake 2: Alternatives Considered lacks rationale

**Problem:** Only lists options, doesn't explain why rejected.

**Improvement:** Use table to compare pros/cons, explain decision basis.

### Mistake 3: Proposed Solution too brief

**Problem:** Team cannot evaluate feasibility.

**Improvement:** Provide API design, code examples, architecture diagrams.

### Mistake 4: Open Questions never updated

**Problem:** Questions resolved but not recorded, readers think still unresolved.

**Improvement:** Regularly update, check or remove when resolved.

## Summary

A good RFC should:
- **Clearly define problem and scope** (Problem, Goals, Non-Goals)
- **Provide sufficient detail for evaluation** (Proposed Solution)
- **Prove alternatives were evaluated** (Alternatives Considered)
- **Track unresolved questions** (Open Questions)
- **Link related documents** (References)

The goal is to achieve consensus before writing code and leave complete decision context for future readers.
