# Collaboration Guidelines

## 1 · Roles & Responsibilities&#x20;

| Role                  | Primary Interface      | Responsibilities                                          |
| **Product Owner**     | Chat / Canvas          | Describe tasks, supply sample data, accept finished work. |
| **Owner’s Assistant** | Chat / Canvas          | Review codebase, turn discussion into actionable prompts  |
| **AI Code Agent**     | Terminal / Git commits | Implement code; log progress.                             |
                                                        |

> ## 2 · Minimal Repo Layout
```
<project>/
├── Requirements.md            # domain‑specific requirements
├── Collaboration‑Guidelines.md# ← this file
├── Worklog.md                 # chronological log + prompts
└── README.md                  # 2‑line project purpose + quickstart
```

## 3 · Task Workflow

1. **Owner / OA (chat only)** – draft the next-task prompt.
2. **AI Code Agent** – pull repo, append prompt to `Worklog.md`, commit → `task:`.
3. **AI Code Agent** – write code/tests.
4. **AI Code Agent** – add “Status: Completed” in the same prompt block, commit →`done:`.

---

## 4 · Resilience & Restart Rules

1. **Everything committed** — prompt, code, completion notes—all in `main`.
2. **Linear history** — commit straight to `main`; revert bad commits with `git revert`.
3. **Single‑file log** — `Worklog.md` is the canonical timeline; read bottom‑up to catch up.
4. **One‑command demo** — after fresh clone, the command in the latest prompt must succeed.
5. **Rollback protocol** — if main breaks, revert and add a “Rollback” entry in `Worklog.md` explaining why.

---

## 5 · Quick‑Start Checklist (new repo)

1. Add `Collaboration‑Guidelines.md` (this file).
2. Add a stub `Requirements.md`.
3. Create the **first prompt block** at the end of `Worklog.md` (even if just a header).
4. Commit all three files directly to `main`.
5. Hand off prompt to AI Code Agent; let it commit code.
6. Iterate: next prompt → append to `Worklog.md` → commit.