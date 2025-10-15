using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.GameObjects;

public abstract class GameObject : AbstractObject,
    IObjectWithConcept
{
    public ObjectRef ConceptRef { get; } = ObjectRef.Get(
        FormatClass("ConceptType"),
        FormatId(FormatClass().Replace("Type", ""))
    );

    public ConceptType Concept()
    {
        return (ConceptType)ConceptRef.Object();
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