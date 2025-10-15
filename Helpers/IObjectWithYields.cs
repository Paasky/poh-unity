using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithYields
{
    public Yields Yields { get; }
    public List<YieldEffect> YieldMods();
}