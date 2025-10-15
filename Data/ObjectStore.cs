using System.Text.Json;
using System.Xml.Linq;
using PohLibrary.GameObjects;
using PohLibrary.GameObjects.Simple;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.Data;

public static class ObjectStore
{
    private static readonly Dictionary<string, Dictionary<string, AbstractObject>> Store = [];
    public static World? World = null;

    public static void Set(AbstractObject obj)
    {
        if (!Store.TryGetValue(obj.Class, out var classDict))
        {
            classDict = new Dictionary<string, AbstractObject>();
            Store[obj.Class] = classDict;
        }

        classDict[obj.Id] = obj;
    }

    public static AbstractObject Get(ObjectRef objectRef)
    {
        return Store[objectRef.Class][objectRef.Id];
    }

    public static Dictionary<string, AbstractObject> GetAll(string className)
    {
        return Store[className];
    }

    public static AbstractObject? TryToGet(ObjectRef objectRef)
    {
        try
        {
            return Get(objectRef);
        }
        catch (Exception)
        {
            return null;
        }
    }

    // Placeholder for relation building step; implement as needed later
    public static void BuildRelations()
    {
        var errors = new Dictionary<string, List<Exception>>();

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
                var objRef = obj.Ref();

                if (obj is IObjectWithAllowsAndRequires objWithRequires)
                {
                    foreach (IObjectWithAllowsAndRequires require in objWithRequires.Requires.Objects())
                    {
                        PohLib.TryTo(() => require.Allows.Add(objRef), errors, obj.Key);
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
                    foreach (IObjectWithUpgrades upgrade in objWithUpgrades.UpgradesFrom.Objects())
                    {
                        PohLib.TryTo(() => upgrade.UpgradesTo.Add(objRef), errors, obj.Key);
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

        if (errors.Count == 0) return;
        
        var eList = new List<Exception>();
        foreach (var exceptions in errors.Select(keyVal => keyVal.Value))
        {
            eList.AddRange(exceptions);
        }
        throw new AggregateException(eList);
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

    private static void AddRelatesTo(IEnumerable<AbstractObject> objects, ObjectRef relateToRef, Dictionary<string, List<Exception>> errors)
    {
        foreach (IObjectWithRelatesTo relatedObj in objects)
        {
            PohLib.TryTo(() => relatedObj.RelatesTo.Add(relateToRef), errors, relateToRef.Key);
        }
    }
}