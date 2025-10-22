using System.Text.Json;
using PohLibrary.Exceptions;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
// ReSharper disable ExplicitCallerInfoArgument

namespace PohLibrary.Data;

public class DataLoader(Action<string>? output)
{
    private Action<string>? _output = output;
    public DataErrors Errors { get; } = new();

    public void LoadFromDir(string directoryPath)
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
            InitObjectsFromFile(file);
        }

        Output("Build Relations");
        var buildErrors = Errors.InitGroup("BuildRelations");
        ObjectStore.BuildRelations(buildErrors);

        Output("Fill ObjectRef details");
        var fillErrors = Errors.InitGroup("FillRefDetails");
        ObjectRef<IObject>.FillDetails(fillErrors);
    }

    private void InitObjectsFromFile(string filePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var fileErrors = Errors.InitGroup(fileName);
        
        var doc = PohLib.TryTo(() => ParseFile(filePath), fileErrors);
        if (doc == null) return;
        
        var classType = PohLib.TryTo(() => ResolveClass(fileName), fileErrors);
        if (classType == null) return;

        InitObjects(classType, doc.RootElement, fileErrors);
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

    private void InitObjects(Type objectClass, JsonElement objectsElem, Dictionary<string, List<Exception>> fileErrors)
    {
        if (!typeof(IObject).IsAssignableFrom(objectClass) || objectClass.IsAbstract || objectClass.IsInterface)
        {
            fileErrors["config"] = [new Exception($"invalid object class {objectClass}")];
            return;
        }

        var i = 0;
        foreach (var objectElem in objectsElem.EnumerateArray())
        {
            var name = objectElem.TryGetProperty("name", out var nameElem) ? nameElem.GetString() : null;
            var idForLog = string.IsNullOrWhiteSpace(name) ? i.ToString() : name;
            
            PohLib.TryTo(
                () => IObject.FromJson(objectClass, objectElem),
                fileErrors,
                idForLog
            );
            i++;
        }
    }

    private void Output(string message)
    {
        _output?.Invoke(message);
    }
}