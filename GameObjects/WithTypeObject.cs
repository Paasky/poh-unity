using PohLibrary.GenericObjects;
using PohLibrary.TypeObjects;

namespace PohLibrary.GameObjects;

public abstract class WithTypeObject(ObjectRef<TypeObject> typeObjectRef) : GameObject
{
    public ObjectRef<TypeObject> TypeObjectRef { get; } = typeObjectRef;

    public TypeObject Type()
    {
        return TypeObjectRef.Object();
    }
}