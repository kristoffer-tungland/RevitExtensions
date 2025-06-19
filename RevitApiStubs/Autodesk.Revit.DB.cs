using System;

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
    public class Element : IDisposable
    {
        public ElementId Id { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Dispose"/> has been called.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public Element(ElementId id) => Id = id;

        /// <summary>
        /// Disposes the element. In the stubs this simply sets <see cref="IsDisposed"/>.
        /// </summary>
        public void Dispose() => IsDisposed = true;
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Document.
    /// </summary>
    public class Document { }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.FilteredElementCollector capturing
    /// the document and type filter.
    /// </summary>
    public class FilteredElementCollector : System.Collections.Generic.IEnumerable<Element>
    {
        public Document Document { get; }
        public Type FilterType { get; private set; }
        public BuiltInCategory? Category { get; private set; }
        public System.Collections.Generic.IList<BuiltInCategory> Categories { get; private set; }
        public bool ExcludesElementTypes { get; private set; }
        public bool OnlyElementTypes { get; private set; }

        private readonly System.Collections.Generic.List<Element> _elements = new System.Collections.Generic.List<Element>();

        public FilteredElementCollector(Document document)
        {
            Document = document;
        }

        public void AddElement(Element element) => _elements.Add(element);

        public System.Collections.Generic.IEnumerator<Element> GetEnumerator() => _elements.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public FilteredElementCollector OfClass(Type type)
        {
            FilterType = type;
            return this;
        }

        public FilteredElementCollector WherePasses(ElementCategoryFilter filter)
        {
            Category = filter.Category;
            return this;
        }

        public FilteredElementCollector WherePasses(ElementMulticategoryFilter filter)
        {
            Categories = filter.Categories;
            return this;
        }

        public FilteredElementCollector WhereElementIsNotElementType()
        {
            ExcludesElementTypes = true;
            OnlyElementTypes = false;
            return this;
        }

        public FilteredElementCollector WhereElementIsElementType()
        {
            OnlyElementTypes = true;
            ExcludesElementTypes = false;
            return this;
        }
    }

    /// <summary>
    /// Enumeration of built-in Revit categories. Only the members required for
    /// unit tests are included.
    /// </summary>
    public enum BuiltInCategory
    {
        /// <summary>
        /// Generic model elements.
        /// </summary>
        GenericModel
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Wall used in tests.
    /// </summary>
    public class Wall : Element
    {
        public Wall(ElementId id) : base(id) { }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.ElementCategoryFilter capturing
    /// the category passed to the collector.
    /// </summary>
    public class ElementCategoryFilter
    {
        public BuiltInCategory Category { get; }

        public ElementCategoryFilter(BuiltInCategory category)
        {
            Category = category;
        }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.ElementMulticategoryFilter capturing
    /// the categories passed to the collector.
    /// </summary>
    public class ElementMulticategoryFilter
    {
        public System.Collections.Generic.IList<BuiltInCategory> Categories { get; }

        public ElementMulticategoryFilter(System.Collections.Generic.IList<BuiltInCategory> categories)
        {
            Categories = categories;
        }
    }
}
