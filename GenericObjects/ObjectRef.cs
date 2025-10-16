using PohLibrary.Data;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class ObjectRef(string className, string id, string? name = null)
{
    private static readonly Dictionary<string, Dictionary<string, ObjectRef>> Store = [];
    public string Class { get; } = className;
    public string Id { get; } = id;
    public string Key { get; } = AbstractObject.KeyFromClassAndId(className, id);
    public string? Name { get; set; } = name;

    public AbstractObject Object()
    {
        return ObjectStore.Get(this);
    }

    public static ObjectRef Get(AbstractObject abstractObject)
    {
        if (!Store.TryGetValue(abstractObject.Class, out Dictionary<string, ObjectRef>? classDict))
        {
            classDict = new Dictionary<string, ObjectRef>();
            Store[abstractObject.Class] = classDict;
        }

        if (!classDict.TryGetValue(abstractObject.Id, out ObjectRef? objectRef))
        {
            objectRef = new ObjectRef(abstractObject.Class, abstractObject.Id, abstractObject.Name);
            classDict[abstractObject.Id] = objectRef;
        }
        
        return objectRef;
    }

    public static ObjectRef Get(string className, string id)
    {
        if (!Store.TryGetValue(className, out Dictionary<string, ObjectRef>? classDict))
        {
            classDict = new Dictionary<string, ObjectRef>();
            Store[className] = classDict;
        }

        if (!classDict.TryGetValue(id, out ObjectRef? objectRef))
        {
            objectRef = new ObjectRef(className, id);
            classDict[id] = objectRef;
        }
        
        return objectRef;
    }
    
    public bool Is(ObjectRef obj)
    {
        return Key == obj.Key;
    }

    public bool Is(AbstractObject obj)
    {
        return Key == obj.Key;
    }

    public static void FillDetails()
    {
        var errors = new Dictionary<string, List<Exception>>();
        foreach (var classRefs in Store.Values)
        {
            foreach (var classRef in classRefs.Values)
            {
                if (classRef.Name == null)
                {
                    PohLib.TryTo(() => classRef.Name = classRef.Object().Name, errors);
                }
            }
        }

        if (errors.Count == 0) return;
        
        var eList = new List<Exception>();
        foreach (var exceptions in errors.Select(keyVal => keyVal.Value))
        {
            eList.AddRange(exceptions);
        }
        throw new AggregateException(eList);
    }
}