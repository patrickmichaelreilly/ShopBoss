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

        var sdfDirectory = Path.GetDirectoryName(sdfPath);
        
        if (string.IsNullOrEmpty(sdfDirectory))
        {
            // If no directory in path, use current directory
            sdfDirectory = Directory.GetCurrentDirectory();
        }

        var sqliteFilePath = Path.Combine(sdfDirectory, "work.sqlite");
        var tempSqlPath = Path.Combine(sdfDirectory, "temp.sql");
        

        try
        {
            // Step 1: Convert SDF to SQL script using ExportSqlCE40.exe
            await ConvertSdfToSqlScript(sdfPath, tempSqlPath, sdfDirectory);

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

    private async Task ConvertSdfToSqlScript(string sdfPath, string sqlPath, string workingDirectory)
    {
        var exportTool = Path.Combine(_toolsDirectory, "ExportSqlCe40.exe");
        
        if (!File.Exists(exportTool))
        {
            throw new FileNotFoundException($"ExportSqlCe40.exe not found at: {exportTool}. Please ensure native binaries are included.");
        }

        var connectionString = $"Data Source={sdfPath};";
        var baseWorkPath = Path.Combine(workingDirectory, "work");
        
        
        using var process = new Process();
        process.StartInfo.FileName = exportTool;
        process.StartInfo.Arguments = $"\"{connectionString}\" \"{baseWorkPath}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        
        // Add timeout to prevent hanging
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Kill the process if it's hanging
            if (!process.HasExited)
            {
                process.Kill();
                await process.WaitForExitAsync(); // Wait for kill to complete
            }
            throw new InvalidOperationException("ExportSqlCe40.exe process timed out after 60 seconds");
        }
        
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"ExportSqlCe40.exe failed with exit code {process.ExitCode}. Error: {error}");
        }

        // ExportSqlCe40.exe creates files like work_0000.sql, work_0001.sql, etc.
        // We need to combine them into a single temp.sql file
        await CombineSqlFiles(workingDirectory, sqlPath);
    }

    private async Task CombineSqlFiles(string directory, string outputPath)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentException("Directory path cannot be null or empty", nameof(directory));
        }
        
        if (string.IsNullOrEmpty(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));
        }


        var workFiles = Directory.GetFiles(directory, "work_*")
                                .Where(f => !Path.HasExtension(f) || Path.GetExtension(f) == ".sql")
                                .OrderBy(f => f)
                                .ToArray();


        if (workFiles.Length == 0)
        {
            throw new InvalidOperationException($"No files found with pattern work_* in directory {directory}");
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
        
        await outputWriter.FlushAsync();
        
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
        process.StartInfo.Arguments = $"\"{sqliteFilePath}\" \".read {sqlPath}\" \".quit\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        
        // Close stdin immediately to prevent hanging
        process.StandardInput.Close();
        
        // Add timeout to prevent hanging
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Kill the process if it's hanging
            if (!process.HasExited)
            {
                process.Kill();
                await process.WaitForExitAsync(); // Wait for kill to complete
            }
            throw new InvalidOperationException("sqlite3 process timed out after 10 seconds");
        }

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();
            throw new InvalidOperationException($"sqlite3 failed with exit code {process.ExitCode}. Error: {error}");
        }
    }

}