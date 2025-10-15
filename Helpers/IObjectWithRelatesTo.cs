using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithRelatesTo
{
    public ObjectRefList<ObjectRef> RelatesTo { get; }
}