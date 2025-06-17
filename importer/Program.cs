using System.Data;
using System.Data.SQLite;
using System.Text.Json;

namespace SdfImporter;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "--self-check")
            {
                JsonStructureTest.RunSelfCheck();
                return 0;
            }
            
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: dotnet run -- <path-to-sdf-or-sqlite-file>");
                Console.Error.WriteLine("       dotnet run -- --self-check");
                return 1;
            }

            string filePath = args[0];
            
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File not found: {filePath}");
                return 1;
            }

            var importer = new SdfImporter();
            var result = await importer.ImportAsync(filePath);
            
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            Console.WriteLine(json);
            
            // Verify the output structure
            if (!JsonStructureTest.VerifyJsonStructure(json))
            {
                Console.Error.WriteLine("JSON structure validation failed");
                return 1;
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}

public class SdfImporter
{
    private readonly SdfToSqliteConverter _converter = new();

    public async Task<ImportResult> ImportAsync(string filePath)
    {
        string sqliteFilePath;
        
        // Check if input is SDF or SQLite
        if (Path.GetExtension(filePath).ToLowerInvariant() == ".sdf")
        {
            // Convert SDF to SQLite
            sqliteFilePath = await _converter.ConvertAsync(filePath);
        }
        else
        {
            // Assume it's already SQLite
            sqliteFilePath = filePath;
        }
        
        var connectionString = $"Data Source={sqliteFilePath};Version=3;";
        
        using var connection = new SQLiteConnection(connectionString);
        await connection.OpenAsync();
        
        return new ImportResult
        {
            Products = await ReadTableAsync(connection, "Products"),
            Parts = await ReadTableAsync(connection, "Parts"),
            PlacedSheets = await ReadTableAsync(connection, "PlacedSheets"),
            Hardware = await ReadTableAsync(connection, "Hardware"),
            Subassemblies = await ReadTableAsync(connection, "Subassemblies"),
            OptimizationResults = await ReadTableAsync(connection, "OptimizationResults")
        };
    }
    
    private async Task<List<Dictionary<string, object?>>> ReadTableAsync(SQLiteConnection connection, string tableName)
    {
        var results = new List<Dictionary<string, object?>>();
        
        try
        {
            using var command = new SQLiteCommand($"SELECT * FROM [{tableName}]", connection);
            using var reader = await command.ExecuteReaderAsync();
            
            var columnNames = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnNames[i] = reader.GetName(i);
            }
            
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnNames[i]] = value;
                }
                results.Add(row);
            }
        }
        catch (SQLiteException ex)
        {
            // Handle table not found gracefully
            Console.Error.WriteLine($"Warning: Table '{tableName}' not found or error reading: {ex.Message}");
        }
        
        return results;
    }
}

public class ImportResult
{
    public List<Dictionary<string, object?>> Products { get; set; } = new();
    public List<Dictionary<string, object?>> Parts { get; set; } = new();
    public List<Dictionary<string, object?>> PlacedSheets { get; set; } = new();
    public List<Dictionary<string, object?>> Hardware { get; set; } = new();
    public List<Dictionary<string, object?>> Subassemblies { get; set; } = new();
    public List<Dictionary<string, object?>> OptimizationResults { get; set; } = new();
}