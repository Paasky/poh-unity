using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithSpecials
{
    public ObjectRefList<ObjectRef> Specials { get; }
}