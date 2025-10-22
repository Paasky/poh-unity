using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithRelatesTo : IObject
{
    public ObjectRefList<IObject> RelatesTo { get; }
}