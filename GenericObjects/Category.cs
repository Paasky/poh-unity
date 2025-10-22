using PohLibrary.Data;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class Category(string className, string name)
    : IObject, IObjectWithRelatesTo
{
    public string Id { get; } = FormatId(className, name);
    public string Class { get; } = "Category";
    public string Name { get; } = name;
    public string? Description { get; } = null;
    
    public ObjectRefList<IObject> RelatesTo { get; } = new();

    public static Category Get(string className, string name)
    {
        if (ObjectStore.TryToGet<Category>(className, name, out var category)) return category;
        category = new Category(className, name);
        ObjectStore.Set(category);
        return category;
    }

    private static string FormatId(string className, string name)
    {
        return PohLib.FormatId($"{className} {name}");
    }
}