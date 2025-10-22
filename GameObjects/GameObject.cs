using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.GameObjects;

public abstract class GameObject :
    IObjectWithConcept
{

    public string Id { get; }
    public string Class { get; }
    public string? Name { get; } = null;
    public string? Description { get; } = null;
    public ObjectRef<ConceptType> ConceptRef { get; }
    
    public GameObject()
    {
        Class = PohLib.FormatClass(GetType().Name);
        Id = Guid.NewGuid().ToString();
        ConceptRef = ObjectRef<ConceptType>.Get("ConceptType", PohLib.FormatId(Class), Class);
    }

    public GameObject(string? id, string? name = null, string? description = null)
    {
        Class = PohLib.FormatClass(GetType().Name);
        Id = string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        Name = name;
        Description = description;
        ConceptRef = ObjectRef<ConceptType>.Get("ConceptType", PohLib.FormatId(Class), Class);
    }
    
    public Dictionary<string, object?> Save()
    {
        return new Dictionary<string, object?>{
            ["id"] = Id,
            ["name"] = Name,
            ["description"] = Description,
        };
    }

    public static T FromJson<T>(JsonElement data) where T : GameObject
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            data.GetProperty("id").GetString()!,
            data.GetProperty("name").GetString()!,
            data.TryGetProperty("description", out var d) ? d.GetString() : null
        )!;
    }
}