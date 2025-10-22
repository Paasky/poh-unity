using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithYields : IObject
{
    public Yields Yields { get; }
    
    public Yields YieldOutput(Yields? yieldMods = null, List<string>? yieldTypeIds = null)
    {
        var yields = Yields.Direct(yieldTypeIds);

        if (yieldMods == null) return yields;
            
        foreach (var yieldEffect in yieldMods.All().Where(yieldEffect =>
                     yieldEffect.IsFor(this)
                     && (
                         yieldTypeIds == null
                         || yieldTypeIds.Contains(yieldEffect.TypeRef.Id)
                     )
                 )) {
            yields.Add(yieldEffect);
        }

        return yields;
    }

    public double YieldTypeOutput(string yieldTypeId, Yields? yieldMods = null)
    {
        var yield = YieldOutput(yieldMods, [yieldTypeId]).ApplyMods().All().FirstOrDefault();

        if (yield == null) return 0;

        return yield.Amount;
    }

    public List<YieldEffect> YieldMods()
    {
        return Yields.Mods().All();
    }
}