# ShopBoss

Shop Floor Part Tracking System to replace discontinued Production Coach software for millwork manufacturing workflow management.

## Quick Start

### SDF Importer v3 (SQLite Converter Wrapper)

**Build (WSL/Linux):**
```bash
cd importer
dotnet publish -c Release -r win-x64
```

**Setup (Windows 11):**
1. Download ExportSqlCE40.exe from [Erik EJ's SqlCeToolbox releases](https://github.com/ErikEJ/SqlCeToolbox/releases)
2. Extract and place `ExportSqlCE40.exe` in the `/tools/` directory

**Run (Windows 11 CMD/PowerShell):**
```cmd
# Navigate to the publish directory
cd importer\bin\Release\net8.0\win-x64\publish

# Import SDF file (automatic conversion to SQLite)
.\Importer.exe path\to\your\file.sdf

# Or import pre-converted SQLite file
.\Importer.exe path\to\your\file.sqlite

# Run self-check test
.\Importer.exe --self-check
```

**Conversion Process:**
1. `ExportSqlCE40.exe` converts SDF → SQL script
2. SQL script is imported into SQLite database  
3. SQLite database is queried for JSON output

**Requirements:** 
- No native SQL CE DLLs required
- Works in WSL when using pre-converted .sqlite files
- ExportSqlCE40.exe tool from [Erik EJ's SqlCeToolbox](https://github.com/ErikEJ/SqlCeToolbox)

**Output:** JSON with six top-level keys: `products`, `parts`, `placedSheets`, `hardware`, `subassemblies`, `optimizationResults`.

Development tasks are tracked in `Worklog.md`.