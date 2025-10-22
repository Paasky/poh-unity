using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects;

public abstract class ComplexTypeObject(string id, string name, string description)
    : TypeObject(id, name, description),
        IObjectWithAllowsAndRequires,
        IObjectWithGains,
        IObjectWithSpecials,
        IObjectWithYields
{
    public ObjectRefList<IObjectWithAllowsAndRequires> Allows { get; } = new();
    public RequireRefList Requires { get; } = new();
    public Gains Gains { get; } = [];
    public ObjectRefList<IObject> Specials { get; } = new();
    public Yields Yields { get; } = new();

    public void LoadExtrasFromJson(JsonElement data)
    {
        if (data.TryGetProperty("gains", out var gains))
        {
            foreach (var gain in gains.EnumerateArray())
            {
                Gains.Add(gain);
            }
        }
        if (data.TryGetProperty("requires", out var requires))
        {
            foreach (var require in requires.EnumerateArray())
            {
                Requires.Add(require);
            }
        }
        // ReSharper disable once InvertIf
        if (data.TryGetProperty("specials", out var specials))
        {
            foreach (var special in specials.EnumerateArray())
            {
                Specials.Add(special);
            }
        }
    }
}