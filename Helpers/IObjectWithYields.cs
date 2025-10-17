using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithYields
{
    public Yields Yields { get; }
    public Yields YieldOutput(Yields? yieldMods = null, List<string>? yieldTypeIds = null);
    public double YieldTypeOutput(string yieldTypeId, Yields? yieldMods = null);
}