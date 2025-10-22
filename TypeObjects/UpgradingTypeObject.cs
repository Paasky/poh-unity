using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects;

public class UpgradingTypeObject(string id, string name, string? description) :
    CategorizedTypeObject(id, name, description),
    IObjectWithUpgrades
{

    public ObjectRefList<IObjectWithUpgrades> UpgradesFrom { get; } = new();
    public ObjectRefList<IObjectWithUpgrades> UpgradesTo { get; } = new();

    protected new TypeObject LoadExtrasFromJson(JsonElement data)
    {
        base.LoadExtrasFromJson(data);
        
        if (data.TryGetProperty("upgradesFrom", out var upgradesFrom))
        {
            foreach (var fromElem in upgradesFrom.EnumerateArray())
            {
                UpgradesFrom.Add(fromElem);
            }
        }
        
        return this;
    }
}