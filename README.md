# ShopBoss

Shop Floor Part Tracking System to replace discontinued Production Coach software for millwork manufacturing workflow management.

## Quick Start

### SDF Importer (Proof of Concept)

**Build (WSL/Linux):**
```bash
cd importer && dotnet build
```

**Run (Windows 11 CMD/PowerShell):**
```cmd
cd importer
dotnet run -- path\to\your\file.sdf
```

**SQL CE Access Method:** Microsoft.SqlServer.Compact package for .NET compatibility.

**Note:** The importer builds cross-platform but requires Windows runtime due to SQL Server Compact dependencies.

Development tasks are tracked in `Worklog.md`.