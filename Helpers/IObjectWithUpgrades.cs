using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithUpgrades : IObject
{
    public ObjectRefList<IObjectWithUpgrades> UpgradesFrom { get; }
    public ObjectRefList<IObjectWithUpgrades> UpgradesTo { get; }
}