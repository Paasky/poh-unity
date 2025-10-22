using System.Text.Json;
using PohLibrary.Helpers;

namespace PohLibrary.GenericObjects;

public class Gains : List<Gain>
{
    public void Add(JsonElement element)
    {
        Add(Gain.Get(element));
    }
    
    public IEnumerable<IObjectWithGains> Objects()
    {
        return this.Select(gain => gain.Ref.Object());
    }
}