using PohLibrary.Helpers;
using PohLibrary.TypeObjects;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.GenericObjects;

public class YieldEffect
{
    public enum YieldMethod
    {
        Lump,      // "lump"
        Percent,   // "percent"
        Set        // "set"
    }

    public ObjectRef<YieldType> TypeRef { get; }
    public double Amount;
    public YieldMethod Method { get; }
    public ObjectRefList<TypeObject> For { get; }
    public ObjectRefList<TypeObject> Vs  { get; }

    public YieldEffect(
        ObjectRef<YieldType> typeRef,
        double amount,
        YieldMethod method = YieldMethod.Lump,
        ObjectRefList<TypeObject>? @for = null,
        ObjectRefList<TypeObject>? vs = null
    ) {
        TypeRef = typeRef;
        Amount = amount;
        Method = method;
        For = @for ?? new ObjectRefList<TypeObject>();
        Vs   = vs ?? new ObjectRefList<TypeObject>();
    }
    
    public bool IsFor(IObject obj)
    {
        return For.All().Any(@for => @for.Is(obj));
    }

    public bool IsVs(IObject obj)
    {
        return Vs.All().Any(vs => vs.Is(obj));
    }

    public bool IsMod()
    {
        return For.All().Count > 0 || Vs.All().Count > 0;
    }
    
    public static double GetTotal(ObjectRef<YieldType> yieldTypeRef, IEnumerable<YieldEffect> yields)
    {
        var total = yields.Where(
            y => yieldTypeRef.Is(y.TypeRef.Key)
        ).Sum(
            y => y.Amount
        );
        return Math.Round(total, 1);
    }
}