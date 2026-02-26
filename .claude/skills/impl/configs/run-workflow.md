# Workflow: Run

**Auto-detect:** If user has impl file open in IDE, automatically use that path. Otherwise, ask the user which impl to run.

## Execution Workflow

### Step 1: Read Impl File

```
Read {impl-folder}/impl.md
```

### Step 2: Parse Implementation Steps

Locate the `## Implementation Steps` section and collect all checkbox items:

```markdown
- [ ] Modify `{file:line}` - {Brief description}
```

Identify which items are unchecked (`- [ ]`) vs already completed (`- [x]`).

If all items are already checked, inform the user and stop.

### Step 3: Execute Each Unchecked Step

For each unchecked `- [ ]` item, **in order from top to bottom**:

#### 3.1 Read Target File

Use Read tool to open the file referenced in the step. Verify the file and line exist.

#### 3.2 Apply Modification

Edit the code as described in the step. Reference the corresponding Phase section in the impl for detailed context (code changes, modification context) if needed.

**Rules:**
- Follow project conventions (Vue 3 Composition API, .NET PascalCase)
- Only make changes described in the impl — do not add extra modifications
- If a step references a file that doesn't exist yet, create it
- If a step is ambiguous, check the corresponding Phase section for clarification

#### 3.3 Check Off the Checkbox

Immediately after completing each step, update the impl file:

```
- [ ] Modify `{file:line}` - {description}
```
becomes:
```
- [x] Modify `{file:line}` - {description}
```

**Important:** Check off each item right after it is completed, not in a batch at the end. This ensures progress is tracked even if execution is interrupted.

### Step 4: Verify

After all steps are executed:

1. Re-read the impl file to confirm all checkboxes are checked
2. If any items could not be completed, leave them unchecked and note the reason

## Completion Output

```
Impl run complete.

Execution Summary:
- Completed: {N}/{Total} steps
- Skipped: {list of skipped items with reasons, if any}

Impl file: {path}/impl.md
```
