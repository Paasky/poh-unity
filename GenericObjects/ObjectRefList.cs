using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class ObjectRefList<T> where T : ObjectRef
{
    private readonly Dictionary<string, T> _refs;
    
    public ObjectRefList()
    {
        _refs = new Dictionary<string, T>();
    }
    
    public ObjectRefList(List<T> refs)
    {
        _refs = new Dictionary<string, T>();
        foreach (T objectRef in refs)
        {
            Add(objectRef);
        }
    }

    public ObjectRefList(List<AbstractObject> abstractObjects)
    {
        _refs = [];
        foreach (AbstractObject abstractObject in abstractObjects)
        {
            Add(abstractObject);
        }
    }

    public ObjectRefList<T> Add(T objectRef)
    {
        _refs[objectRef.Key] = objectRef;
        return this;
    }
    
    public ObjectRefList<T> Add(AbstractObject abstractObject)
    {
        _refs[abstractObject.Key] = (T)abstractObject.Ref();
        return this;
    }

    public bool Contains(T objectRef)
    {
        return _refs.ContainsKey(objectRef.Key);
    }

    public bool Contains(AbstractObject abstractObject)
    {
        return _refs.ContainsKey(abstractObject.Key);
    }

    public bool Remove(T objectRef)
    {
        return _refs.Remove(objectRef.Key);
    }

    public bool Remove(AbstractObject abstractObject)
    {
        return _refs.Remove(abstractObject.Key);
    }

    public IEnumerable<AbstractObject> Objects()
    {
        return _refs.Values.Select(objectRef => objectRef.Object());
    }

    public List<T> All()
    {
        return _refs.Values.ToList();
    }
}