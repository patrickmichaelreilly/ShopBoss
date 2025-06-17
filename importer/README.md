# Importer

Converts SQL Server Compact (SDF) files to JSON format via SQLite conversion.

## Usage

```bash
C:\ShopBoss\Importer\Importer.exe C:\ShopBoss\MicrovellumWorkOrder.sdf
```

## How it works

1. **ExportSqlCe40.exe** → converts SDF to SQL script(s)
2. **sqlite3.exe** → imports SQL scripts into SQLite database
3. **System.Data.SQLite** → reads SQLite database and outputs JSON

All native binaries are included in the published output.