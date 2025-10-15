using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithUpgrades
{
    public ObjectRefList<ObjectRef> UpgradesFrom { get; }
    public ObjectRefList<ObjectRef> UpgradesTo { get; }
}