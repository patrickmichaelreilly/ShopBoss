# Importer

Converts SQL Server Compact (SDF) files to JSON format via SQLite conversion.

## Build & Run

```bash
# Build and publish
dotnet publish -c Release -r win-x86 --self-contained

# Run
C:\ShopBoss\Importer\Importer.exe C:\ShopBoss\MicrovellumWorkOrder.sdf
```

## How it works

1. **native\ExportSqlCe40.exe** → converts SDF to single temp.sql file
2. **native\sqlite3.exe** → creates work.sqlite from temp.sql 
3. **System.Data.SQLite** → reads work.sqlite and outputs JSON (six keys)

All native binaries are included in the published output. Creates only work.sqlite (no chunk files).