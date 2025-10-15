using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.Helpers;

public interface IObjectWithConcept
{
    public ObjectRef ConceptRef { get; }
    public ConceptType Concept();
}