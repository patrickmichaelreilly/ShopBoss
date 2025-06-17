# ShopBoss

Shop Floor Part Tracking System to replace discontinued Production Coach software for millwork manufacturing workflow management.

## Quick Start

### SDF Importer v2 (Self-Contained Distribution)

**Build (WSL/Linux):**
```bash
cd importer
dotnet publish -c Release -r win-x64
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

**SQL CE Requirements:** All required SQL Server Compact 4.0 native DLLs are automatically included in the publish output:
- `sqlcecompact40.dll`
- `sqlceca40.dll` 
- `sqlcese40.dll`
- `sqlceqp40.dll`
- `sqlceme40.dll`
- `sqlceer40EN.dll`

The target Windows system requires [Microsoft Visual C++ 2010 Service Pack 1 Redistributable Package (x64)](https://www.microsoft.com/en-us/download/details.aspx?id=13523).

**Output:** JSON with six top-level keys: `products`, `parts`, `placedSheets`, `hardware`, `subassemblies`, `optimizationResults`.

Development tasks are tracked in `Worklog.md`.