using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects;

public class CategorizedTypeObject : ComplexTypeObject,
    IObjectWithCategory
{
    public Category Category { get; private set; } = null!;

    protected override TypeObject LoadFromJson(JsonElement data)
    {
        base.LoadFromJson(data);
        Category = new Category(data.GetProperty("category").GetString()!);
        
        return this;
    }
}