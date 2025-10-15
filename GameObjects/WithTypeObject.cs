using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
using PohLibrary.TypeObjects;

namespace PohLibrary.GameObjects;

public abstract class WithTypeObject : GameObject
{
    public ObjectRef TypeObjectRef { get; }

    public TypeObject Type()
    {
        return (TypeObject)TypeObjectRef.Object();
    }
}