# ShopBoss

Shop Floor Part Tracking System to replace discontinued Production Coach software for millwork manufacturing workflow management.

## Quick Start

### SDF Importer v2 (Self-Contained EXE)

**Build (WSL/Linux):**
```bash
cd importer
dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true
```

**Run (Windows 11 CMD/PowerShell):**
```cmd
# Navigate to the publish directory
cd importer\bin\Release\net8.0\win-x64\publish

# Import SDF file and output JSON
.\Importer.exe path\to\your\file.sdf

# Run self-check test
.\Importer.exe --self-check
```

**SQL CE Requirements:** The SQL Server Compact native DLLs (`sqlcese40.dll`, `sqlceqp40.dll`) must be available in the system PATH or copied next to `Importer.exe`. These are typically installed with SQL Server Compact 4.0 Runtime.

**Output:** JSON with six top-level keys: `products`, `parts`, `placedSheets`, `hardware`, `subassemblies`, `optimizationResults`.

Development tasks are tracked in `Worklog.md`.