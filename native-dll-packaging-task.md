### 2025-06-17 – Package **all** native SQL CE DLLs

**Goal** – Ensure the published Windows-x64 build of the importer always carries **all six** required SQL CE 4.0 native DLLs so it runs out-of-the-box on Windows 11.

**Definition of Done**  
1. `dotnet publish -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true` produces a `publish\` folder with **Importer.exe** plus these six DLLs:  
   `sqlcecompact40.dll`, `sqlceca40.dll`, `sqlcese40.dll`, `sqlceqp40.dll`, `sqlceme40.dll`, `sqlceer40EN.dll`  
2. Copying that folder to a clean Windows 11 PC (with VC++ 2010 SP1) and executing  
   `Importer.exe sample.sdf` prints valid JSON—no initializer errors.  
3. `README.md` lists the DLL names and links to the VC++ 2010 redistributable.  
4. Exactly **two commits** for this task:  
   * `task: add native DLL packaging prompt` ← this block  
   * `done: native DLL packaging` (project file change + README update + "Status: Completed" line below)

**Implementation hints**  
* Add all six DLLs under `importer/native/` and include:  
  ```xml
  <ItemGroup>
    <None Include="native\*.dll" CopyToPublishDirectory="PreserveNewest" />
  </ItemGroup>
  ```  
* Remove current DLL references that point to NuGet paths.  
* Update README dependency note.

**Out of Scope** – Schema migrations, Entity Framework, persistent DB, web server, rack logic, assembly/shipping features.

<!-- AI Code Agent will append: **Status: Completed – …** -->