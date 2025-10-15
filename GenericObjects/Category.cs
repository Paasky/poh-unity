using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class Category(string name)
    : AbstractObject(id: FormatId(name), name: name),
        IObjectWithRelatesTo
{
    public ObjectRefList<ObjectRef> RelatesTo { get; } = new();

    private static readonly Dictionary<string, Category> Store = [];

    public static Category Get(string name)
    {
        if (Store.TryGetValue(name, out Category? category)) return category;
        
        category = new Category(name);
        Store[name] = category;
        return category;
    }
}