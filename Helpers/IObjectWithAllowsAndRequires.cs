using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithAllowsAndRequires : IObject
{
    public ObjectRefList<IObjectWithAllowsAndRequires> Allows { get; }
    public RequireRefList Requires { get; }
}