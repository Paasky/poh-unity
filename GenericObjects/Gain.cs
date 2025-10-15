namespace PohLibrary.GenericObjects;

public class Gain(string className, string id, string name, int amount = 1)
    : ObjectRef(className, id, name)
{
    public int Amount { get; } = amount;
}