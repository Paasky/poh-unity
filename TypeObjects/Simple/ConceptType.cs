using System.Text.Json;
using PohLibrary.GenericObjects;

namespace PohLibrary.TypeObjects.Simple;

public class ConceptType : TypeObject
{
    public List<ObjectRef<YieldType>> YieldTypesFromTile { get; } = [];
    
    protected ConceptType(string id, string name, string? description) : base(id, name, description)
    {
    }
    
    protected void LoadExtrasFromJson(JsonElement data)
    {
        if (!data.TryGetProperty("yieldTypesFromTile", out var yieldTypesFromTile)) return;
        
        foreach (var yieldType in yieldTypesFromTile.EnumerateArray()
            .Select(yieldTypeElem => yieldTypeElem.GetString())
            .Where(yieldType => !string.IsNullOrWhiteSpace(yieldType)))
        {
            YieldTypesFromTile.Add(ObjectRef<YieldType>.Get("YieldType", PohLib.FormatId(yieldType!)));
        }
    }
}