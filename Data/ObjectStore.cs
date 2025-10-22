using PohLibrary.GameObjects;
using PohLibrary.GameObjects.Simple;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.Data;

public static class ObjectStore
{
    private static readonly Dictionary<string, Dictionary<string, IObject>> Store = [];
    public static World? World = null;

    public static void Set(IObject obj)
    {
        if (!Store.TryGetValue(obj.Class, out var classDict))
        {
            classDict = new Dictionary<string, IObject>();
            Store[obj.Class] = classDict;
        }

        classDict[obj.Id] = obj;
    }

    public static TObj Get<TObj>(ObjectRef<TObj> objectRef) where TObj : IObject
    {
        return Get<TObj>(objectRef.Class, objectRef.Id);
    }

    public static TObj Get<TObj>(string className, string id) where TObj : IObject
    {
        return (TObj)Store[className][id];
    }

    public static Dictionary<string, IObject> GetAll(string className)
    {
        return Store[className];
    }
    
    public static bool TryToGet<TObj>(
        ObjectRef<TObj> objectRef,
        out TObj result
    ) where TObj : IObject
    {
        try
        {
            result = Get(objectRef);
            return true;
        }
        catch
        {
            result = default!;
            return false;
        }
    }
    
    public static bool TryToGet<TObj>(
        string className,
        string id,
        out TObj result
    ) where TObj : IObject
    {
        try
        {
            result = Get<TObj>(className, id);
            return true;
        }
        catch
        {
            result = default!;
            return false;
        }
    }

    // Placeholder for relation building step; implement as needed later
    public static void BuildRelations(Dictionary<string, List<Exception>> errors)
    {
        var nonGameObjects = Store.Values.SelectMany(
            objects =>
                objects.Select(
                    keyVal => keyVal.Value
                ).TakeWhile(
                    obj => obj is not GameObject
                )
        );
        
        foreach (var obj in nonGameObjects)
        {
            PohLib.TryTo(() =>
            {
                var objRef = obj.Ref<IObject>();

                if (obj is IObjectWithAllowsAndRequires objWithRequires)
                {
                    var requireRef = obj.Ref<IObjectWithAllowsAndRequires>();
                    foreach (var require in objWithRequires.Requires.Objects())
                    {
                        PohLib.TryTo(() => require.Allows.Add(requireRef), errors, obj.Key);
                    }
                }

                if (obj is IObjectWithCategory objWithCategory)
                {
                    PohLib.TryTo(() => objWithCategory.Category.RelatesTo.Add(objRef), errors, obj.Key);
                }

                if (obj is IObjectWithGains objWithGains)
                {
                    AddRelatesTo(objWithGains.Gains.Objects(), objRef, errors);
                }

                if (obj is IObjectWithSpecials objWithSpecials)
                {
                    AddRelatesTo(objWithSpecials.Specials.Objects(), objRef, errors);
                }

                if (obj is IObjectWithUpgrades objWithUpgrades)
                {
                    var upgradeRef = obj.Ref<IObjectWithUpgrades>();
                    foreach (IObjectWithUpgrades upgrade in objWithUpgrades.UpgradesFrom.Objects())
                    {
                        PohLib.TryTo(() => upgrade.UpgradesTo.Add(upgradeRef), errors, obj.Key);
                    }
                }

                // ReSharper disable once InvertIf
                if (obj is IObjectWithYields objWithYields)
                {
                    foreach (var yieldMod in objWithYields.YieldMods())
                    {
                        AddRelatesTo(yieldMod.For.Objects(), objRef, errors);
                        AddRelatesTo(yieldMod.Vs.Objects(), objRef, errors);
                    }
                }
            }, errors, obj.Key);
        }
    }

    public static Dictionary<string, object?> Save()
    {
        var objects = new List<object?>();
        foreach (var obj in Store.Values.SelectMany(classObjects => classObjects.Values))
        {
            if (obj is GameObject gameObject)
            {
                objects.Add(gameObject.Save());
            }
        }
        
        return new Dictionary<string, object?>
        {
            ["world"] = World?.Save(),
            ["objects"] = objects
        };
    }

    private static void AddRelatesTo(IEnumerable<IObject> objects, ObjectRef<IObject> relateToRef, Dictionary<string, List<Exception>> errors)
    {
        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (IObjectWithRelatesTo relatedObj in objects)
        {
            PohLib.TryTo(() => relatedObj.RelatesTo.Add(relateToRef), errors, relateToRef.Key);
        }
    }
}