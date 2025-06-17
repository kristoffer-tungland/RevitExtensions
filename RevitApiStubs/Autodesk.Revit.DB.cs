namespace Autodesk.Revit.DB
{
    /// <summary>
    /// Represents a stable persistent identifier for elements.
    /// Only the relevant properties for extension methods are modeled.
    /// </summary>
    public class ElementId
    {
#if REVIT2024_OR_ABOVE
        public long Value { get; }
        public ElementId(long value) => Value = value;
#else
        public int IntegerValue { get; }
        public ElementId(int value) => IntegerValue = value;
#endif
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Element exposing only the Id property.
    /// </summary>
    public class Element
    {
        public ElementId Id { get; }
        public Element(ElementId id) => Id = id;
    }
}
