using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.TypeObjects;

public abstract class TypeObject : AbstractObject,
    IObjectWithConcept,
    IObjectWithRelatesTo
{
    public ObjectRefList<ObjectRef> RelatesTo { get; } = new();

    public ObjectRef ConceptRef { get; } = ObjectRef.Get(
        FormatClass("ConceptType"),
        FormatId(FormatClass().Replace("Type", ""))
    );

    public ConceptType Concept()
    {
        return (ConceptType)ConceptRef.Object();
    }

    public static T FromJson<T>(JsonElement data) where T : TypeObject
    {
        var name = data.GetProperty("name").GetString()!;
        var id = FormatId(name);
        
        var instance = (T)Activator.CreateInstance(
            typeof(T),
            id,
            name,
            data.TryGetProperty("description", out var d) ? d.GetString() : null
        )!;

        instance.LoadFromJson(data);
        return instance;
    }
    
    protected abstract TypeObject LoadFromJson(JsonElement data);
}