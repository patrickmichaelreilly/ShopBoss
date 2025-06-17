using System.Data;
using System.Data.SqlServerCe;
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
                Console.Error.WriteLine("Usage: dotnet run -- <path-to-sdf-file>");
                Console.Error.WriteLine("       dotnet run -- --self-check");
                return 1;
            }

            string sdfPath = args[0];
            
            if (!File.Exists(sdfPath))
            {
                Console.Error.WriteLine($"File not found: {sdfPath}");
                return 1;
            }

            var importer = new SdfImporter();
            var result = await importer.ImportAsync(sdfPath);
            
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
    public async Task<ImportResult> ImportAsync(string sdfPath)
    {
        var connectionString = $"Data Source={sdfPath};";
        
        using var connection = new SqlCeConnection(connectionString);
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
    
    private async Task<List<Dictionary<string, object?>>> ReadTableAsync(SqlCeConnection connection, string tableName)
    {
        var results = new List<Dictionary<string, object?>>();
        
        try
        {
            using var command = new SqlCeCommand($"SELECT * FROM [{tableName}]", connection);
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
        catch (SqlCeException ex)
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