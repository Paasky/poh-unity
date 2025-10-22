using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.Helpers;

public interface IObjectWithConcept : IObject
{
    public ObjectRef<ConceptType> ConceptRef { get; }
    
    
    public ConceptType Concept()
    {
        return ConceptRef.Object();
    }
}