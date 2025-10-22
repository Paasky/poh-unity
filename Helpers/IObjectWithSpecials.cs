using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithSpecials : IObject
{
    public ObjectRefList<IObject> Specials { get; }
}