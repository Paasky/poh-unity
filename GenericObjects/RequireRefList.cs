namespace PohLibrary.GenericObjects;

public class RequireRefList
{
    private List<ObjectRefList<ObjectRef>> _anyOfRefLists;
    private ObjectRefList<ObjectRef> _allOfRefs;
    private ObjectRefList<ObjectRef> _allRefs;

    public RequireRefList(object[] requireItems)
    {
        _anyOfRefLists = [];
        _allOfRefs = new ObjectRefList<ObjectRef>();
        _allRefs = new ObjectRefList<ObjectRef>();
        foreach (var item in requireItems)
        {
            Add(item);
        }
    }

    public RequireRefList Add(ObjectRef objectRef)
    {
        _allOfRefs.Add(objectRef);
        _allRefs.Add(objectRef);
        
        return this;
    }

    public RequireRefList Add(ObjectRefList<ObjectRef> anyOf)
    {
        _anyOfRefLists.Add(anyOf);
        foreach (ObjectRef objectRef in anyOf.All())
        {
            _allRefs.Add(objectRef);
        }

        return this;
    }
    
    private void Add(object item)
    {
        switch (item)
        {
            case ObjectRef r:
                Add(r);
                break;

            case ObjectRefList<ObjectRef> rl:
                Add(rl);
                break;

            default:
                throw new ArgumentException(
                    $"Unsupported item type '{item.GetType().Name}'. " +
                    "Expected ObjectRef or ObjectRefList.");
        }
    }

    public List<ObjectRef> AllOfRefs()
    {
        return _allOfRefs.All();
    }

    public List<ObjectRefList<ObjectRef>> AnyOfRefLists()
    {
        return _anyOfRefLists;
    }

    public bool Contains(ObjectRef objectRef)
    {
        return _allRefs.Contains(objectRef);
    }
}