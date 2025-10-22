using System.Text.Json;
using PohLibrary.Data;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class ObjectRef<TObj> where TObj : IObject
{
    private static readonly Dictionary<string, Dictionary<string, ObjectRef<TObj>>> Store = [];
    public string Class { get; }
    public string Id { get; }
    public string Key { get; }
    public string? Name;

    private ObjectRef(string className, string id, string? name = null)
    {
        Class = className;
        Id = id;
        Key = PohLib.KeyFromClassAndId(className, id);
        Name = name;
    }

    public TObj Object()
    {
        return (ObjectStore.Get(this));
    }

    public static ObjectRef<TObj> Get(TObj obj)
    {
        return Get(obj.Class, obj.Id, obj.Name);
    }

    public static ObjectRef<TObj> Get(JsonElement element)
    {
        try
        {
            var name = element.TryGetProperty("name", out var nameElem) ? nameElem.ToString() : null;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("name is required");
            }
            
            var categoryClass = element.TryGetProperty("category", out var categoryElem) ? categoryElem.ToString() : null;
            if (!string.IsNullOrWhiteSpace(categoryClass))
            {
                var category = Category.Get(categoryClass, name);
                return Get("Category", category.Id, category.Name);
            }

            var maybeId = element.TryGetProperty("id", out var idElem) ? idElem.GetString() : null;
            var id = string.IsNullOrWhiteSpace(maybeId)
                ? PohLib.FormatId(name)
                : maybeId;
            
            var className = GetClassFromJson(element);

            return Get(className, id, name);
        }
        catch (Exception e)
        {
            var inData = element.GetRawText();
            throw new Exception($"{e.Message} in {inData}", e);
        }
    }

    public static ObjectRef<TObj> Get(string className, string id, string? name = null)
    {
        if (!Store.TryGetValue(className, out var classDict))
        {
            classDict = new Dictionary<string, ObjectRef<TObj>>();
            Store[className] = classDict;
        }

        if (classDict.TryGetValue(id, out var objectRef)) return objectRef;
        
        objectRef = new ObjectRef<TObj>(className, id);
        classDict[id] = objectRef;

        return objectRef;
    }
    
    public bool Is(string key) => Key == key;
    public bool Is(ObjectRef<IObject> obj) => Key == obj.Key;

    public bool Is(IObject obj) => Key == obj.Key;

    private static string GetClassFromJson(JsonElement element)
    {
        var maybeClass = element.TryGetProperty("class", out var classElem) ? classElem.ToString() : null;
        var maybeType = element.TryGetProperty("type", out var typeElem) ? typeElem.ToString() : null;
        
        if (!string.IsNullOrWhiteSpace(maybeClass))
        {
            return PohLib.FormatClass(maybeClass);
        }
        
        if (string.IsNullOrWhiteSpace(maybeType))
        {
            throw new Exception("class or type is required");
        }
        
        var typeClass = maybeType.EndsWith("Type")
            ? maybeType
            : maybeType + "Type";
        
        return PohLib.FormatClass(typeClass);
    }

    public static void FillDetails(Dictionary<string, List<Exception>> errors)
    {
        // For each Ref with null name, set name from Object
        foreach (var classRef in Store.Values.SelectMany(
                     classRefs => classRefs.Values.Where(
                         classRef => classRef.Name == null
                    )
        ))
        {
            PohLib.TryTo(() => classRef.Name = classRef.Object().Name, errors, classRef.Key);
        }
    }
}