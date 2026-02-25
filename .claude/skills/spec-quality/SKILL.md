---
name: spec-quality
description: General specification document quality guidelines
---

# Spec Quality Guidelines

This skill provides general quality guidelines for all types of specification documents.

## Core Principles

Good specification documents should be:
1. **Clear** - Readers can quickly understand purpose and content
2. **Complete** - Cover all necessary information
3. **Consistent** - Format and terminology remain consistent
4. **Traceable** - Clear links to related documents
5. **Maintainable** - Easy to update and modify

## Universal Quality Checklist

### 1. Structural Integrity

- [ ] Contains all required sections
- [ ] Section order follows logic
- [ ] Heading hierarchy is correct (H1 > H2 > H3)
- [ ] No empty or placeholder sections (unless explicitly using "None")

### 2. Content Clarity

- [ ] Purpose and scope clearly defined
- [ ] Avoid vague terms ("maybe", "probably", "should")
- [ ] Technical terminology used consistently
- [ ] Examples are concrete and representative
- [ ] Complex concepts supported by diagrams or code

### 3. Language and Format

- [ ] Follows project language conventions (English for user text, English for code)
- [ ] Code blocks use correct syntax highlighting
- [ ] Tables formatted correctly and aligned
- [ ] Lists use consistent symbols (- or 1.)
- [ ] Links are valid and use relative paths

### 4. Traceability

- [ ] References related documents (RFC, ADR, PRD)
- [ ] Version information clear (if applicable)
- [ ] Change history recorded (if applicable)
- [ ] Author or responsible party clear (if applicable)

### 5. Completeness

- [ ] Covers all necessary information for implementation
- [ ] Edge cases and error handling described
- [ ] Assumptions and limitations clearly stated
- [ ] Security considerations (if applicable)

## Document Type-Specific Guidance

### RFC Documents

**Focus:**
- Problem description is concrete
- Non-Goals should not be empty
- Alternatives Considered includes rationale for rejected options
- Open Questions regularly updated

**Common Issues:**
- Problem too brief, doesn't quantify impact
- Non-Goals empty, haven't thought through scope boundaries
- Proposed Solution lacks code examples or architecture diagrams

### ADR Documents

**Focus:**
- Context records situation at decision time
- Decision includes alternative comparison
- Consequences honestly lists both positive and negative impacts
- Implementation records actual execution steps

**Common Issues:**
- Context only records "what was done" instead of "why it was done"
- Consequences only lists positives, doesn't record technical debt
- Missing Implementation section

### OpenAPI Specifications

**Focus:**
- All endpoints have summary and description
- All parameters and fields have description
- Provide example values
- Define common error responses
- Use $ref to reuse schemas

**Common Issues:**
- Missing or too-brief descriptions
- Missing example values
- Duplicate schema definitions instead of using $ref
- Undefined error responses

### Test Plans

**Focus:**
- Test cases are specific and verifiable
- Cover happy path and error cases
- Organized by test level (Unit / Integration / E2E)
- Coverage goals are clear

**Common Issues:**
- Test cases too brief, cannot implement
- Missing error cases
- Undefined coverage goals
- Test tools not documented

## Writing Techniques

### 1. Use Active Voice

**Avoid:** "Data will be collected"
**Use:** "System collects data"

### 2. Be Specific, Not Abstract

**Avoid:** "Improve performance"
**Use:** "Reduce API response time from 2s to 500ms"

### 3. Consistent Terminology

**Create a glossary:**
- SensorData (consistent use, don't mix with sensor_data, sensordata)
- granularity (time granularity, consistently use this English term)
- BleAddress (consistent casing)

### 4. Appropriate Detail Level

**Too brief:**
```markdown
Add API endpoint to support aggregation queries.
```

**Appropriate:**
```markdown
Add `GET /api/sensor-data/aggregate` endpoint supporting `minute/hour/day/month`
time granularities, returning statistics for each period (avg, max, min, sum, count).
```

**Too detailed:**
```markdown
(Full code implementation with line-by-line comments)
```
(This belongs in code, not specification documents)

## Review Process

### Self-Review

After writing:
1. Re-read from reader's perspective
2. Check if it answers "why" not just "what"
3. Verify all links are valid
4. Run spell and grammar check

### Peer Review

Ask team members to check:
1. Is purpose and scope clearly understood
2. Is there enough information for implementation
3. Are there omissions or ambiguities
4. Are terminology and format consistent

### Regular Maintenance

- Regularly review old documents, update Status
- Fix broken links
- Update Context when situation changes
- Add References when new information available

## Common Errors Summary

1. **Empty sections not using "None"**: Causes reading confusion
2. **Missing References**: Cannot trace related decisions
3. **Inconsistent terminology**: Mix different names for same concept
4. **Missing examples**: Abstract descriptions hard to understand
5. **Status not updated**: Old documents appear valid but are outdated
6. **Too brief**: Cannot use for implementation reference
7. **Too detailed**: Buries key information

## Tool Recommendations

- **Markdown Linter**: Check format consistency
- **Vale**: Check writing style
- **markdownlint-cli**: Automated Markdown format checking
- **PlantUML / Mermaid**: Draw architecture diagrams

## Summary

Specification document quality directly affects:
- **Communication efficiency**: Team quickly reaches consensus
- **Implementation correctness**: Developers understand requirements
- **Maintenance cost**: Future modifications understand context

Investing time in high-quality specification documents will save significant debugging and refactoring time long-term.

Remember: **Code tells you how. Documentation tells you why.**
