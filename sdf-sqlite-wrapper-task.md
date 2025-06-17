### 2025-06-18 – PoC: SDF ➜ SQLite Converter Wrapper

**Goal** – Replace brittle direct-CE access with an external conversion step:

1. Run Erik EJ's **ExportSqlCE40.exe** to convert `sample.sdf` → `sample.sqlite`.
2. Load the resulting SQLite with `System.Data.SQLite` and emit the JSON summary of these six tables:
   `products`, `parts`, `placedSheets`, `hardware`, `subassemblies`, `optimizationResults`.

**Definition of Done**
1. Repository includes `/tools/ExportSqlCE40.exe` (x86) and a tiny wrapper script/class that shells out:
   ```
   converter.exe sample.sdf ➜ produces sample.sqlite (same folder)
   importer.exe sample.sqlite ➜ prints JSON with six top-level keys
   ```
2. Works end-to-end on Windows 11 **and** inside WSL (because the wrapper can be skipped in WSL by supplying an already-converted `sample.sqlite` for unit tests).
3. README has exact one-liner build & run commands, plus link to Erik EJ tool source.
4. Exactly **two commits** for this task:
   * `task: add SDF→SQLite wrapper prompt`  ← this block
   * `done: SDF→SQLite wrapper` (tool drop-in, wrapper code, README, "Status: Completed" line below)

**Constraints**
* Wrapper and JSON logic live under `/importer/`.
* No direct reference to `System.Data.SqlServerCe`; the EXE never P/Invokes native CE DLLs.
* Only AI Code Agent commits (`task:` then `done:`).

**Out of Scope** – Web UI, racks, assembly/shipping, CE native-DLL debugging.

<!-- AI Code Agent will append: **Status: Completed – …** -->