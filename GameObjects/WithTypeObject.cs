using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects;

namespace PohLibrary.GameObjects;

public abstract class WithTypeObject(ObjectRef typeObjectRef) : GameObject
{
    public ObjectRef TypeObjectRef { get; } = typeObjectRef;

    public TypeObject Type()
    {
        return (TypeObject)TypeObjectRef.Object();
    }
}