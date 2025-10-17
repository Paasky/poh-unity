using PohLibrary.TypeObjects.Simple;

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

    public Yields ApplyMods()
    {
        var lumpPerType = new Dictionary<string, double>();
        var percentPerType = new Dictionary<string, double>();
        var setPerType = new Dictionary<string, double>();

        foreach (var yield in _yields)
        {
            switch (yield.Method)
            {
                case YieldEffect.YieldMethod.Lump:
                    lumpPerType[yield.TypeRef.Id] = lumpPerType.GetValueOrDefault(yield.TypeRef.Id, 0) + yield.Amount;
                    break;
                case YieldEffect.YieldMethod.Percent:
                    percentPerType[yield.TypeRef.Id] = percentPerType.GetValueOrDefault(yield.TypeRef.Id, 0) + yield.Amount;
                    break;
                case YieldEffect.YieldMethod.Set:
                    // "Set" doesn't cumulate
                    setPerType[yield.TypeRef.Id] = yield.Amount;
                    break;
            }
        }
        
        var totalAmountPerType = new Dictionary<string, double>();
        
        foreach (var keyValue in setPerType)
        {
            totalAmountPerType[keyValue.Key] = Math.Round(keyValue.Value, 1);
        }

        foreach (var keyValue in lumpPerType)
        {
            // "Set" means it overrides lump & percent
            if (setPerType.ContainsKey(keyValue.Key))
            {
                continue;
            }
            
            // Add "Lump" while applying "Percent" multiplier
            var multiplier = 1 + percentPerType.GetValueOrDefault(keyValue.Key, 0) / 100;
            totalAmountPerType[keyValue.Key] = Math.Round(keyValue.Value * multiplier, 1);
        }

        var yieldEffects = totalAmountPerType.Select(keyValue =>
            new YieldEffect(
                // ReSharper disable once AccessToStaticMemberViaDerivedType
                ObjectRef.Get(YieldType.FormatClass(), keyValue.Key), keyValue.Value)
        );

        return new Yields(yieldEffects);
    }

    public Yields Direct(List<string>? yieldTypeIds = null)
    {
        if (yieldTypeIds is null)
        {
            return new Yields(_yields.Where(yieldEffect => !yieldEffect.IsMod()));
        }
        
        return new Yields(_yields.Where(yieldEffect =>
            !yieldEffect.IsMod()
            && yieldTypeIds.Contains(yieldEffect.TypeRef.Id)
        ));
    }

    public Yields Mods(List<string>? yieldTypeIds = null)
    {
        if (yieldTypeIds is null)
        {
            return new Yields(_yields.Where(yieldEffect => yieldEffect.IsMod()));
        }

        return new Yields(_yields.Where(yieldEffect =>
            yieldEffect.IsMod()
            && yieldTypeIds.Contains(yieldEffect.TypeRef.Id)
        ));
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