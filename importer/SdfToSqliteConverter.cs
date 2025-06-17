using System.Diagnostics;

namespace SdfImporter;

public class SdfToSqliteConverter
{
    private readonly string _toolsDirectory;

    public SdfToSqliteConverter(string? toolsDirectory = null)
    {
        _toolsDirectory = toolsDirectory ?? Path.Combine(AppContext.BaseDirectory, "native");
    }

    public async Task<string> ConvertAsync(string sdfPath)
    {
        if (!File.Exists(sdfPath))
        {
            throw new FileNotFoundException($"SDF file not found: {sdfPath}");
        }

        var sdfDirectory = Path.GetDirectoryName(sdfPath) ?? throw new InvalidOperationException("Cannot determine SDF directory");
        var sqliteFilePath = Path.Combine(sdfDirectory, "work.sqlite");
        var tempSqlPath = Path.Combine(sdfDirectory, "temp.sql");

        try
        {
            // Step 1: Convert SDF to SQL script using ExportSqlCE40.exe
            await ConvertSdfToSqlScript(sdfPath, tempSqlPath);

            // Step 2: Create SQLite database from SQL script
            await CreateSqliteFromScript(tempSqlPath, sqliteFilePath);

            return sqliteFilePath;
        }
        finally
        {
            // Clean up temporary SQL file
            if (File.Exists(tempSqlPath))
            {
                File.Delete(tempSqlPath);
            }
        }
    }

    private async Task ConvertSdfToSqlScript(string sdfPath, string sqlPath)
    {
        var exportTool = Path.Combine(_toolsDirectory, "ExportSqlCe40.exe");
        
        if (!File.Exists(exportTool))
        {
            throw new FileNotFoundException($"ExportSqlCe40.exe not found at: {exportTool}. Please ensure native binaries are included.");
        }

        var connectionString = $"Data Source={sdfPath};";
        var sdfDirectory = Path.GetDirectoryName(sdfPath) ?? throw new InvalidOperationException("Cannot determine SDF directory");
        var baseWorkPath = Path.Combine(sdfDirectory, "work");
        
        using var process = new Process();
        process.StartInfo.FileName = exportTool;
        process.StartInfo.Arguments = $"\"{connectionString}\" \"{baseWorkPath}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"ExportSqlCe40.exe failed with exit code {process.ExitCode}. Error: {error}");
        }

        // ExportSqlCe40.exe creates files like work_0000.sql, work_0001.sql, etc.
        // We need to combine them into a single temp.sql file
        await CombineSqlFiles(sdfDirectory, sqlPath);
    }

    private async Task CombineSqlFiles(string directory, string outputPath)
    {
        var workFiles = Directory.GetFiles(directory, "work_*.sql")
                                .OrderBy(f => f)
                                .ToArray();

        if (workFiles.Length == 0)
        {
            throw new InvalidOperationException($"No SQL files found with pattern work_*.sql in directory {directory}");
        }

        using var outputWriter = new StreamWriter(outputPath);
        
        foreach (var workFile in workFiles)
        {
            var content = await File.ReadAllTextAsync(workFile);
            await outputWriter.WriteAsync(content);
            await outputWriter.WriteLineAsync(); // Add newline between files
            
            // Clean up the work file
            File.Delete(workFile);
        }
    }

    private async Task CreateSqliteFromScript(string sqlPath, string sqliteFilePath)
    {
        // Delete existing SQLite file if it exists
        if (File.Exists(sqliteFilePath))
        {
            File.Delete(sqliteFilePath);
        }

        var sqlite3Path = Path.Combine(_toolsDirectory, "sqlite3.exe");
        
        if (!File.Exists(sqlite3Path))
        {
            throw new FileNotFoundException($"sqlite3.exe not found at: {sqlite3Path}. Please ensure native binaries are included.");
        }

        using var process = new Process();
        process.StartInfo.FileName = sqlite3Path;
        process.StartInfo.Arguments = $"\"{sqliteFilePath}\" \".read {sqlPath}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"sqlite3 failed with exit code {process.ExitCode}. Error: {error}");
        }
    }

}