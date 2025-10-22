using PohLibrary.GenericObjects;

namespace PohLibrary.Helpers;

public interface IObjectWithCategory : IObject
{
    public Category Category { get; }
}