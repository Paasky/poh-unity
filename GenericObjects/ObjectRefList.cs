using System.Text.Json;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class ObjectRefList<TObj> where TObj : IObject
{
    private readonly Dictionary<string, ObjectRef<TObj>> _refs;
    
    public ObjectRefList()
    {
        _refs = new Dictionary<string, ObjectRef<TObj>>();
    }
    
    public ObjectRefList(List<ObjectRef<TObj>> refs)
    {
        _refs = new Dictionary<string, ObjectRef<TObj>>();
        foreach (var objectRef in refs)
        {
            Add(objectRef);
        }
    }

    public ObjectRefList(List<TObj> objects)
    {
        _refs = [];
        foreach (var obj in objects)
        {
            Add(obj);
        }
    }

    public ObjectRefList<TObj> Add(ObjectRef<TObj> objectRef)
    {
        _refs[objectRef.Key] = objectRef;
        return this;
    }
    
    public ObjectRefList<TObj> Add(IObject obj)
    {
        _refs[obj.Key] = (obj.Ref() as ObjectRef<TObj>)!;
        return this;
    }
    

    public ObjectRefList<TObj> Add(JsonElement element)
    {
        if (!element.TryGetProperty("type", out var typeElem))
        {
            throw new Exception("type is required");
        }

        var type = typeElem.GetString();
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new Exception("type is empty");
        }

        Add(ObjectRef<TObj>.Get(element));
        return this;
    }

    public bool Contains(ObjectRef<TObj> objectRef)
    {
        return _refs.ContainsKey(objectRef.Key);
    }

    public bool Contains(TObj obj)
    {
        return _refs.ContainsKey(obj.Key);
    }

    public bool Remove(ObjectRef<TObj> objectRef)
    {
        return _refs.Remove(objectRef.Key);
    }

    public bool Remove(TObj obj)
    {
        return _refs.Remove(obj.Key);
    }

    public IEnumerable<TObj> Objects()
    {
        return _refs.Values.Select(objectRef => objectRef.Object());
    }

    public List<ObjectRef<TObj>> All()
    {
        return _refs.Values.ToList();
    }
}