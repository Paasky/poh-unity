using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects;

public class CategorizedTypeObject(string id, string name, string description) :
    ComplexTypeObject(id, name, description),
    IObjectWithCategory
{
    public Category Category { get; private set; } = null!;

    protected new TypeObject LoadExtrasFromJson(JsonElement data)
    {
        base.LoadExtrasFromJson(data);
        if (!data.TryGetProperty("category", out JsonElement categoryElem))
        {
            throw new JsonException("category is required");
        }

        var category = categoryElem.GetString();
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new JsonException("category is empty");
        }
        Category = new Category(PohLib.FormatClass(category), category);
        
        return this;
    }
}