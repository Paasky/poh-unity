using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class YieldEffect
{
    public enum YieldMethod
    {
        Lump,      // "lump"
        Percent,   // "percent"
        Set        // "set"
    }

    public ObjectRef TypeRef { get; }
    public double Amount;
    public YieldMethod Method { get; }
    public ObjectRefList<ObjectRef> For { get; }
    public ObjectRefList<ObjectRef> Vs  { get; }

    public YieldEffect(
        ObjectRef typeRef,
        double amount,
        YieldMethod method = YieldMethod.Lump,
        ObjectRefList<ObjectRef>? @for = null,
        ObjectRefList<ObjectRef>? vs = null
    ) {
        TypeRef = typeRef;
        Amount = amount;
        Method = method;
        For = @for ?? new ObjectRefList<ObjectRef>();
        Vs   = vs ?? new ObjectRefList<ObjectRef>();
    }
    
    public bool IsFor(AbstractObject obj)
    {
        return For.All().Any(forRef => obj.Is(forRef));
    }

    public bool IsVs(AbstractObject obj)
    {
        return Vs.All().Any(vsRef => obj.Is(vsRef));
    }

    public bool IsMod()
    {
        return For.All().Count > 0 || Vs.All().Count > 0;
    }
    
    public static double GetTotal(ObjectRef yieldTypeRef, IEnumerable<YieldEffect> yields)
    {
        var total = yields.Where(
            y => yieldTypeRef.Is(y.TypeRef)
        ).Sum(
            y => y.Amount
        );
        return Math.Round(total, 1);
    }
}