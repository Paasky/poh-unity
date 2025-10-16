using System.Reflection;
using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects;
// ReSharper disable ExplicitCallerInfoArgument

namespace PohLibrary.Data;

public class DataLoader(Action<string>? output)
{
    private Action<string>? _output = output;
    public Dictionary<string, List<Exception>> Errors { get; } = [];

    public void LoadTypes(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var files = Directory.GetFiles(directoryPath, "*.json");
        if (files.Length == 0)
        {
            throw new FileNotFoundException($"No json files in {directoryPath}");
        }
        foreach (var file in files)
        {
            Output($"Loading {file}");
            LoadType(file);
        }

        Output("Build Relations");
        PohLib.TryTo(ObjectStore.BuildRelations, Errors, "BuildRelations");

        Output("Fill ObjectRef details");
        PohLib.TryTo(ObjectRef.FillDetails, Errors, "FillRefDetails");

        if (Errors.Count <= 0) return;
        
        var messages = new List<string>();
        foreach (var (context, exceptions) in Errors)
        {
            messages.AddRange(exceptions.Select(e => $"{context}: {e.Message}"));
        }
        throw new Exception(string.Join("; ", messages));
    }

    private void LoadType(string filePath)
    {
        var doc = PohLib.TryTo(() => ParseFile(filePath), Errors);
        if (doc == null) return;

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        
        var classType = PohLib.TryTo(() => ResolveClass(fileName), Errors);
        if (classType == null) return;

        int i = 0;
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            var idForLog = item.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? i.ToString() : i.ToString();
            InitObject(classType, item, $"{fileName}[{idForLog}] fromJson");
            i++;
        }
    }
    
    private static JsonDocument ParseFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                return doc;
            }
        }
        catch (Exception)
        {
            throw new Exception($"Error parsing JSON file: {filePath}");   
        }
        throw new Exception($"Error parsing JSON file: {filePath}");   
    }

    private static Type ResolveClass(string fileBaseName)
    {
        var overrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MajorCultureType"] = "CultureType",
            ["MajorLeaderType"] = "LeaderType",
            ["MinorCultureType"] = "CultureType",
            ["MinorLeaderType"] = "LeaderType",
            ["ResourceManufacturedType"] = "ResourceType",
            ["ResourceNaturalType"] = "ResourceType",
            ["ResourceStrategicType"] = "ResourceType",
        };
        var className = overrides.GetValueOrDefault(fileBaseName, fileBaseName);
        
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t =>
                t.Namespace != null &&
                t.Namespace.StartsWith("PohLibrary.TypeObjects", StringComparison.Ordinal) &&
                t.Name.Equals(className, StringComparison.Ordinal));
        
        if (type != null) return type;
        
        throw new Exception(fileBaseName == className
            ? $"Type {fileBaseName} not found"
            : $"Type {className} not found (using file {fileBaseName})"
        );
    }

    private void InitObject(Type objectClass, JsonElement data, string context)
    {
        PohLib.TryTo(() =>
        {
            // Build closed generic method TypeObject.FromJson<typeClass>()
            var methodInfo = FromJsonOpenGeneric.MakeGenericMethod(objectClass);
            
            // Run & return TypeObject.FromJson(typeData)
            return (TypeObject)methodInfo.Invoke(null, [data])!;
        }, Errors, context);
    }

    private void Output(string message)
    {
        _output?.Invoke(message);
    }
    
    private static readonly MethodInfo FromJsonOpenGeneric =
        typeof(TypeObject).GetMethod(nameof(TypeObject.FromJson), BindingFlags.Public | BindingFlags.Static)
        ?? throw new InvalidOperationException("TypeObject.FromJson<T>(JsonElement) not found.");
}