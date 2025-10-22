using System.Text.Json;
using PohLibrary.GenericObjects;
using PohLibrary.Helpers;
using PohLibrary.TypeObjects.Simple;

namespace PohLibrary.TypeObjects;

public abstract class TypeObject :
    IObjectWithConcept,
    IObjectWithRelatesTo
{
    public string Id { get; }
    public string Class { get; }
    public string Name { get; }
    public string? Description { get; }

    public ObjectRef<ConceptType> ConceptRef { get; }

    public ObjectRefList<IObject> RelatesTo { get; }

    protected TypeObject(string? id, string name, string? description)
    {
        Class = PohLib.FormatClass(GetType().Name);
        Id = string.IsNullOrWhiteSpace(id) ? PohLib.FormatId(name) : id;
        Name = name;
        Description = description;
        ConceptRef = ObjectRef<ConceptType>.Get("ConceptType", PohLib.FormatId(Class), Class);
        RelatesTo = new ObjectRefList<IObject>();
    }
}