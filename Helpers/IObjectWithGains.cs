using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithGains : IObject
{
    public Gains Gains { get; }
}