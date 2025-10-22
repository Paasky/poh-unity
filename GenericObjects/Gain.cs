using System.Text.Json;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class Gain(ObjectRef<IObjectWithGains> objRef, int amount)
{
    public ObjectRef<IObjectWithGains> Ref { get; } = objRef;
    public int Amount { get; } = amount;

    public static Gain Get(JsonElement element)
    {
        return new Gain(
            ObjectRef<IObjectWithGains>.Get(element),
            element.GetProperty("amount").GetInt16()
        );
    }
}