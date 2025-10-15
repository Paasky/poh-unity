using System.ComponentModel;

namespace PohLibrary.GenericObjects;

public class Yields
{
    private readonly List<YieldEffect> _yields = [];

    public Yields(IEnumerable<YieldEffect>? yields = null)
    {
        if (yields is null)
        {
            return;
        }
        
        foreach (var yield in yields)
        {
            Add(yield);
        }
    }

    public Yields Add(YieldEffect yield)
    {
        _yields.Add(yield);
        return this;
    }

    public Yields AddYields(Yields yields)
    {
        foreach (var yield in yields.All())
        {
            Add(yield);
        }

        return this;
    }

    public List<YieldEffect> All()
    {
        return _yields;
    }

    public Yields Filter(List<string> typeIds)
    {
        var output = new Yields();

        foreach (var yieldEffect in _yields.Where(yieldEffect => typeIds.Contains(yieldEffect.TypeRef.Id)))
        {
            output.Add(yieldEffect);
        }
        
        return output;
    }

    public YieldStorage ToStorage()
    {
        var storage = new YieldStorage();
        foreach (var yieldEffect in _yields.Where(yieldEffect => yieldEffect.Method == YieldEffect.YieldMethod.Lump))
        {
            storage.Add(yieldEffect.TypeRef, yieldEffect.Amount);
        }
        
        return storage;
    }
}