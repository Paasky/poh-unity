using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithAllowsAndRequires
{
    public ObjectRefList<ObjectRef> Allows { get; }
    public ObjectRefList<ObjectRef> Requires { get; }
}