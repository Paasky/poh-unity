using PohLibrary.GameObjects;
using PohLibrary.Helpers;

namespace PohLibrary.TypeObjects.Simple;

public class ConceptType : TypeObject
{
    private List<string> _typeKeys = [];
    
    public bool Is(AbstractObject obj)
    {
        if (obj is IObjectWithConcept withConceptObj && Is(withConceptObj))
        {
            return true;
        }
        
        if (obj is WithTypeObject withTypeObj && _typeKeys.Contains(withTypeObj.Type().Key))
        {
            return true;
        }
        
        return _typeKeys.Contains(obj.Key);
    }
    
    public bool Is(IObjectWithConcept obj)
    {
        return obj.ConceptRef.Is(this);
    }
}