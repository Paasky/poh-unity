namespace PohLibrary.GenericObjects;

public class YieldStorage
{
    private Dictionary<string, YieldEffect> _storage = [];

    public YieldStorage Add(ObjectRef yieldTypeRef, double amount)
    {
        if (!_storage.ContainsKey(yieldTypeRef.Key))
        {
            _storage[yieldTypeRef.Key] = new YieldEffect(yieldTypeRef, 0);
        }

        _storage[yieldTypeRef.Key].Amount = Math.Round(
            _storage[yieldTypeRef.Key].Amount + amount,
            1
        );

        return this;
    }

    public YieldStorage Subtract(ObjectRef yieldTypeRef, double amount, bool failOnOverflow = true)
    {
        YieldEffect effect = _storage.GetValueOrDefault(yieldTypeRef.Key, new YieldEffect(yieldTypeRef, 0));
        double newYield = Math.Round(effect.Amount - amount, 1);

        if (newYield < 0)
        {
            if (failOnOverflow) throw new Exception($"Cannot subtract {amount} {yieldTypeRef.Name} (only has {effect.Amount})");
            newYield = 0;
        }
        
        effect.Amount = newYield;
        _storage[yieldTypeRef.Key] = effect;

        return this;
    }

    public bool Has(ObjectRef yieldTypeRef, double amount = 0)
    {
        YieldEffect? effect = _storage!.GetValueOrDefault(yieldTypeRef.Key, null);
        
        return effect?.Amount > 0
               && effect.Amount >= amount;
    }
}