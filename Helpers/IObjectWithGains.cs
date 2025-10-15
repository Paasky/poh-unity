using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithGains
{
    public ObjectRefList<Gain> Gains { get; }
}