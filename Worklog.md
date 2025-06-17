# ShopBoss Development Worklog

This file contains the chronological development log and task prompts for the ShopBoss project.

---

## Initial Repository Setup

**Date:** 2025-06-17  
**Prompt:** Set up initial repository structure with Requirements.md, Collaboration-Guidelines.md, and Worklog.md

**Status:** Completed

---

### 2025-06-17 – Proof-of-Concept SDF Importer

**Goal** – Build a minimal, cross-platform CLI that opens a Microvellum SQL CE (`*.sdf`) work-order file and prints a JSON summary containing **all six** required tables: `Products`, `Parts`, `PlacedSheets`, `Hardware`, `Subassemblies`, `OptimizationResults`.

**Definition of Done**  
1. `dotnet run -- ./sample.sdf` works in **WSL** *and* on **Windows 11** (native CMD or PowerShell).  
2. Output is valid JSON with **exactly six** top-level keys:  
   `products`, `parts`, `placedSheets`, `hardware`, `subassemblies`, `optimizationResults`.  
3. No crashes on the provided `sample.sdf`; unknown columns handled gracefully.  
4. Repo includes a brief `README.md` with one-line build/run commands for both environments and a note on the chosen SQL CE access method (`ErikEJ.SqlCe` or other).  
5. A self-check script or xUnit/NUnit test verifies the JSON root contains those six keys.  
6. Exactly **two commits** for this task:  
   * `task: add POC SDF importer prompt` (this block)  
   * `done: POC SDF importer` (all code + README + "Status: Completed" line appended below)

**Constraints**  
* Use a stack suitable for final deployment: **.NET 8 Console App** preferred.  
* All code must live under `/importer/` (e.g., `Importer.csproj`, `Program.cs`).  
* Do **not** integrate DB writes, web UI, or full architecture yet—pure JSON to stdout.  
* Only the **AI Code Agent** commits; Owner/OA remain read-only.

**Out of Scope** – Schema migrations, Entity Framework, persistent DB, web server, rack logic, assembly/shipping features.

<!-- AI Code Agent will append: **Status: Completed – …** -->