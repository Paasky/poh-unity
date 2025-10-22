using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects;

namespace PohLibrary.Helpers;

public interface IObject
{
    public string Id { get; }
    public string Class { get; }
    string Key => $"{Class}:{Id}";
    public string? Name { get; }
    public string? Description { get; }

    public bool Is(string key) => Key == key;
    public bool Is(ObjectRef<IObject> obj) => Key ==  obj.Key;
    
    public ObjectRef<IObject> Ref() => ObjectRef<IObject>.Get(Class, Id);
    public ObjectRef<TObj> Ref<TObj>() where TObj : IObject => ObjectRef<TObj>.Get(Class, Id);

    public static IObject FromJson(Type objectClass, JsonElement data)
    {
        // Get name (required)
        if (!data.TryGetProperty("name", out var nameElem))
        {
            throw new JsonException("name is required");
        }
        var name = nameElem.GetString();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new JsonException("name is empty");
        }
        
        var instance = (IObject)Activator.CreateInstance(
            objectClass,
            data.TryGetProperty("id", out var idElem) ? idElem.GetString() : null,
            name,
            data.TryGetProperty("description", out var d) ? d.GetString() : null
        )!;

        instance.LoadExtrasFromJson(data);
        return instance;
    }

    // ReSharper disable once UnusedParameter.Global
    protected void LoadExtrasFromJson(JsonElement data)
    {
    }
}