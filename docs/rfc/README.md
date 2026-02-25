# RFC (Request for Comments)

## What is an RFC?

An RFC is a document used to propose and review significant engineering changes **before writing code**. It creates a record of the reasoning, not just the outcome.

RFC is used by Google, Meta, Rust, and Sourcegraph to ensure engineering decisions are well-thought-out and reviewed by the team before implementation.

## When to Write an RFC

Write an RFC when:
- You're proposing a new feature or significant change
- The implementation approach needs team input
- There are multiple valid solutions to consider
- The decision will have long-term architectural impact

## RFC Format

There is no single official RFC standard. Different companies use similar sections but vary in format. This project uses the following structure:

```markdown
# RFC-{number}: {Title}

## Status
- [ ] Draft
- [ ] Under Review
- [ ] Accepted
- [ ] Rejected

## Problem
What problem are we trying to solve?

## Goals
What must this solution achieve?

## Non-Goals
What is explicitly out of scope?

## Proposed Solution
How will we solve this problem?

## Alternatives Considered
What other approaches did we evaluate?

## Open Questions
What still needs to be decided or tested?

## References
Links to related documents
```

## Handling Empty Sections

When a section has no content, use the uniform format:

```markdown
## Non-Goals

None

## Open Questions

None

## References

None
```

**Notes:**
- **Non-Goals** should rarely be empty — think carefully about scope boundaries and what you're explicitly NOT doing
- **Alternatives Considered** should list rejected options; if there are none, use "None"
- **Open Questions** use "None" when all questions are resolved
- **References** use "None" if no related documents exist

## Status Labels

Use checkboxes to track RFC lifecycle:

```
- [ ] Draft           # Initial writing, not ready for review
- [ ] Under Review    # Ready for team feedback
- [ ] Accepted        # Approved, ready to implement
- [ ] Rejected        # Not moving forward
```

## RFC vs ADR

| | RFC | ADR |
|---|---|---|
| **When** | Before decision | After decision |
| **Goal** | Get feedback | Record history |
| **Question** | "Should we?" | "Why did we?" |

An RFC often **becomes** an ADR once accepted — the ADR is the final record of what the RFC concluded.

## RFC to ADR Workflow

### Standard Approach (Recommended)

When an RFC is accepted, **keep the RFC file** in the `rfc/` folder and **write a new ADR** in the `adr/` folder.

**Workflow:**

1. Write `RFC-001-time-aggregation-query.md`
   - Status: Draft → Under Review → Accepted

2. After acceptance, write `ADR-003-time-aggregation-implementation.md`
   - Reference the RFC: "This implements RFC-001"
   - Focus on: final decision, consequences, implementation

3. Both files remain in their respective folders

**Why keep them separate?**

- **RFC** captures the **proposal process**: discussions, alternatives, open questions, review feedback
- **ADR** captures the **final decision**: what was chosen, why, and consequences
- RFC may have details about rejected alternatives that don't need to be in the ADR
- ADR is more concise and focused on the outcome
- Historical value: RFC shows "how we got here", ADR shows "what we decided"

### Alternative Approach (Less Common)

Some teams convert the RFC by moving and renaming it to the `adr/` folder, but this loses the proposal history.

## Naming Convention

```
RFC-{number}-{short-description}.md
```

Examples:
- `RFC-001-time-aggregation-query.md`
- `RFC-002-authentication-strategy.md`

Numbers are zero-padded to 3 digits and sequential.
