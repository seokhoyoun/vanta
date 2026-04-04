# \## Behavioral Guidelines 

# 

# \### 1. Think Before Coding

# \- Do not assume; state assumptions explicitly.

# \- If multiple interpretations exist, surface them instead of silently choosing one.

# \- If a simpler approach exists, call it out and prefer it.

# \- If requirements are unclear, stop and ask a focused clarification question.

# 

# \### 2. Simplicity First

# \- Implement the minimum code needed to satisfy the request.

# \- Do not add unrequested features, abstractions, configurability, or speculative handling.

# \- Avoid overengineering; if a smaller solution exists, use it.

# 

# \### 3. Surgical Changes

# \- Modify only lines required for the user request.

# \- Do not refactor or reformat unrelated code.

# \- Match local style, even if different from personal preference.

# \- Remove only unused code/imports introduced by your own change.

# \- If unrelated dead code is found, report it; do not delete unless asked.

# 

# \### 4. Goal-Driven Execution

# \- Define concrete success criteria before implementation.

# \- For multi-step work, provide a brief plan with a verification check per step.

# \- Verify outcomes directly (tests, reproducible checks, or clear manual validation steps).

# \- Do not stop at "seems fine"; confirm with evidence.

# 

# \## Testing Guidelines

# \- No dedicated unit-test projects are present in this snapshot.

# \- Validate changes with manual smoke tests:

# &#x20; launch app, open key forms, verify alarm/status updates, and confirm startup/shutdown paths.

# \- Include reproducible test steps and outcomes in change notes.

# 

# \## Commit \& Pull Request Guidelines

# \- Git history is not available in this directory snapshot; use clear, scoped commit messages.

# \- Recommended format: `feat(gui): ...`, `fix(comm): ...`, `chore(build): ...`.

# \- PRs should include:

# &#x20; affected solution/project, behavior change summary, manual test evidence, and screenshots for UI changes.

# \- Call out DB/config impacts explicitly (especially changes touching `DB/` artifacts or connection settings).

