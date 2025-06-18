# ShopBoss

Shop Floor Part Tracking System to replace discontinued Production Coach software for millwork manufacturing workflow management.

## SDF Data Importer

Production-ready tool for extracting data from Microvellum SDF (SQL Server Compact) files into JSON or SQLite format.

### Features
- **Fast Binary Data Filtering** - Automatically removes problematic BLOB columns and large hex literals
- **Three Output Modes** - JSON to stdout, JSON to file, or direct SQLite database  
- **Clean Progress Reporting** - Minimal output showing file sizes, timing, and row counts
- **Robust Error Handling** - Graceful degradation for missing tables and SQL compatibility issues

### Build Requirements

**Development Environment:**
- .NET 8 SDK
- Windows Subsystem for Linux (WSL) or Windows
- Git for version control

**Target Platform:** Windows (win-x86)
- Visual C++ 2010 SP1 Redistributable Package (x86)
- Native SQL CE DLLs (included)

### Build Instructions

```bash
# From WSL or Linux (cross-compiling for Windows)
cd importer
dotnet publish -c Release

# Output will be in: bin/Release/net8.0/win-x86/publish/
```

### Usage

**Basic JSON Output:**
```powershell
# Output JSON to stdout
.\Importer.exe MicrovellumWorkOrder.sdf

# Save JSON to file  
.\Importer.exe MicrovellumWorkOrder.sdf --output data.json

# Create SQLite database directly
.\Importer.exe MicrovellumWorkOrder.sdf --output database.sqlite

# Self-check test
.\Importer.exe --self-check
```

**Expected Output:**
```
Cleaning SQL file (29.6 MB)...
Cleaned 897 binary data columns
SQL cleanup completed (reduced by 17.8 MB)
Extracting data from 6 required tables...
Processed 6 tables, 1,234 total rows in 2.3s
JSON written to: data.json (1.2 MB)
Import completed successfully
```

### Data Structure

The importer extracts exactly **6 required tables** into JSON with these top-level keys:
- `products` - Product definitions and specifications
- `parts` - Individual part records with dimensions
- `placedSheets` - Sheet placement and nesting information  
- `hardware` - Hardware components and specifications
- `subassemblies` - Assembly groupings and relationships
- `optimizationResults` - Nesting optimization data

### How It Works

1. **SDF → SQL Conversion** - ExportSqlCE40.exe converts SDF to SQL script
2. **SQL Cleanup** - Remove binary data, fix SQLite compatibility issues  
3. **SQLite Import** - sqlite3.exe creates database from cleaned SQL
4. **Data Extraction** - Query SQLite for required tables with column filtering
5. **JSON Output** - Serialize to clean JSON structure

### Integration Notes

**For ShopBoss v2 Development:**
- Tool outputs clean, structured JSON perfect for further processing
- All binary/BLOB columns filtered out to reduce size and complexity
- Error messages go to stderr, data/success messages to stdout
- Exit code 0 for success, 1 for failure
- Can be called programmatically or via command line

**File Management:**
- Creates temporary `work.sqlite` and `temp.sql` files in working directory
- SQLite database can be reused (faster than re-importing SDF)
- Original SDF files remain unchanged

### Development

Development workflow and detailed task history are tracked in `Worklog.md`.
Project follows collaboration guidelines defined in `Collaboration-Guidelines.md`.

### Requirements

See `Requirements.md` for detailed functional and technical requirements.