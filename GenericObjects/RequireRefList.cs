using System.Text.Json;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class RequireRefList
{
    private readonly List<ObjectRefList<IObjectWithAllowsAndRequires>> _anyOfRefLists;
    private readonly ObjectRefList<IObjectWithAllowsAndRequires> _allOfRefs;
    private readonly ObjectRefList<IObjectWithAllowsAndRequires> _allRefs;

    public RequireRefList(object[]? requireItems = null)
    {
        _anyOfRefLists = [];
        _allOfRefs = new ObjectRefList<IObjectWithAllowsAndRequires>();
        _allRefs = new ObjectRefList<IObjectWithAllowsAndRequires>();
        foreach (var item in requireItems ?? [])
        {
            Add(item);
        }
    }

    public RequireRefList Add(ObjectRef<IObjectWithAllowsAndRequires> objectRef)
    {
        _allOfRefs.Add(objectRef);
        _allRefs.Add(objectRef);
        
        return this;
    }

    public RequireRefList Add(ObjectRefList<IObjectWithAllowsAndRequires> anyOf)
    {
        _anyOfRefLists.Add(anyOf);
        foreach (var objectRef in anyOf.All())
        {
            _allRefs.Add(objectRef);
        }

        return this;
    }

    public RequireRefList Add(JsonElement element)
    {
        var hasType = element.TryGetProperty("type", out var typeElem);
        var hasCategory = element.TryGetProperty("category", out var categoryElem);
        
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (!hasType && !hasCategory)
        {
            throw new Exception("type or category is required");
        }

        if (hasType)
        {
            var type = typeElem.GetString();
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new Exception("type is empty");
            }

            if (type == "any")
            {
                if (!element.TryGetProperty("items", out var itemsElem))
                {
                    throw new Exception("items is required for type=any");
                }

                var items = new ObjectRefList<IObjectWithAllowsAndRequires>();
                PohLib.TryToForeach(
                    itemsElem.EnumerateArray(),
                    itemElem => items.Add(ObjectRef<IObjectWithAllowsAndRequires>.Get(itemElem)),
                    "any"
                );

                Add(items);
                return this;
            }
        }

        Add(ObjectRef<IObjectWithAllowsAndRequires>.Get(element));
        return this;
    }
    
    private void Add(object item)
    {
        switch (item)
        {
            case ObjectRef<IObjectWithAllowsAndRequires> r:
                Add(r);
                break;

            case ObjectRefList<IObjectWithAllowsAndRequires> rl:
                Add(rl);
                break;

            default:
                throw new ArgumentException(
                    $"Unsupported item type '{item.GetType().Name}'. " +
                    "Expected ObjectRef or ObjectRefList.");
        }
    }

    public List<ObjectRef<IObjectWithAllowsAndRequires>> AllOfRefs()
    {
        return _allOfRefs.All();
    }

    public List<ObjectRefList<IObjectWithAllowsAndRequires>> AnyOfRefLists()
    {
        return _anyOfRefLists;
    }

    public IEnumerable<IObjectWithAllowsAndRequires> Objects()
    {
        return _allRefs.All().Select(item => item.Object());
    }

    public bool Contains(ObjectRef<IObjectWithAllowsAndRequires> objectRef)
    {
        return _allRefs.Contains(objectRef);
    }
}