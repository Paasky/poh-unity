using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects;

public class ComplexTypeObject : TypeObject,
    IObjectWithAllowsAndRequires,
    IObjectWithGains,
    IObjectWithSpecials,
    IObjectWithYields
{
    public ObjectRefList<ObjectRef> Allows { get; } = new ObjectRefList<ObjectRef>();
    public RequireRefList Requires { get; } = new RequireRefList([]);
    public ObjectRefList<Gain> Gains { get; } =  new ObjectRefList<Gain>();
    public ObjectRefList<ObjectRef> Specials { get; } =   new ObjectRefList<ObjectRef>();
    
    public Yields Yields { get; } = new Yields();

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

    protected override TypeObject LoadFromJson(JsonElement data)
    {
        if (data.TryGetProperty("gains", out var gains))
        {
            foreach (var gain in gains.EnumerateArray())
            {
                Gains.AddFromJson(gain);   
            }
        }
        if (data.TryGetProperty("requires", out var requires))
        {
            foreach (var require in requires.EnumerateArray())
            {
                Requires.AddFromJson(require);   
            }
        }
        // ReSharper disable once InvertIf
        if (data.TryGetProperty("specials", out var specials))
        {
            foreach (var special in specials.EnumerateArray())
            {
                Specials.AddFromJson(special);   
            }
        }
        
        return this;
    }
}