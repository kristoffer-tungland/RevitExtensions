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
        public Document Document { get; }
        public ElementId TypeId { get; set; }
        public ParameterSet Parameters { get; } = new ParameterSet();

        /// <summary>
        /// Gets a value indicating whether <see cref="Dispose"/> has been called.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public Element(ElementId id) : this(null, id) { }


        public Element(Document document, ElementId id)
        {
            Document = document;
            Id = id;
        }

        public ElementId GetTypeId() => TypeId;

        public Parameter get_Parameter(BuiltInParameter parameter) => new Parameter(parameter);

        public Parameter get_Parameter(ElementId id) => new Parameter(id);

        public Parameter get_Parameter(Guid guid) => new Parameter(guid);

        public Parameter LookupParameter(string name) => new Parameter(name);

        /// <summary>
        /// Disposes the element. In the stubs this simply sets <see cref="IsDisposed"/>.
        /// </summary>
        public void Dispose() => IsDisposed = true;
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Document.
    /// Provides simple worksharing ownership tracking used in tests.
    /// </summary>
    public class Document
    {
        public bool IsWorkshared { get; set; }
        public string CurrentUser { get; set; }

        private readonly System.Collections.Generic.Dictionary<ElementId, string> _owners = new System.Collections.Generic.Dictionary<ElementId, string>();
        private readonly System.Collections.Generic.Dictionary<long, Element> _elements = new System.Collections.Generic.Dictionary<long, Element>();

        public void SetElementOwner(ElementId id, string owner) => _owners[id] = owner;

        public string GetElementOwner(ElementId id)
        {
            return _owners.TryGetValue(id, out var owner) ? owner : null;
        }

        public CheckoutStatus GetCheckoutStatus(ElementId elementId)
        {
            return WorksharingUtils.GetCheckoutStatus(this, elementId);
        }

        private static long GetIdValue(ElementId id)
        {
#if REVIT2024_OR_ABOVE
            return id.Value;
#else
            return id.IntegerValue;
#endif
        }

        public void AddElement(Element element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            _elements[GetIdValue(element.Id)] = element;
        }

        public Element GetElement(ElementId id)
        {
            if (_elements.TryGetValue(GetIdValue(id), out var element))
            {
                return element;
            }
            return new Element(this, id);
        }
    }

    /// <summary>
    /// Provides helper methods for worksharing related functionality.
    /// </summary>
    public static class WorksharingUtils
    {
        public static CheckoutStatus GetCheckoutStatus(Document doc, ElementId elementId)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (elementId == null) throw new ArgumentNullException(nameof(elementId));

            var owner = doc.GetElementOwner(elementId);
            if (string.IsNullOrEmpty(owner)) return CheckoutStatus.NotOwned;
            return owner == doc.CurrentUser ? CheckoutStatus.OwnedByCurrentUser : CheckoutStatus.OwnedByOtherUser;
        }
    }

    public enum CheckoutStatus
    {
        OwnedByCurrentUser,
        OwnedByOtherUser,
        NotOwned,
    }

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

    public enum BuiltInParameter { }

    public enum StorageType
    {
        None,
        Integer,
        Double,
        String,
        ElementId,
    }

    public class Parameter : IDisposable
    {
        public BuiltInParameter? BuiltInParameter { get; }
        public ElementId Id { get; }
        public Guid? Guid { get; }
        public string Name { get; }
        public bool IsDisposed { get; private set; }
        public StorageType StorageType { get; set; }
        public bool IsReadOnly { get; set; }

        private int _intValue;
        private double _doubleValue;
        private string _stringValue;
        private ElementId _elementIdValue;

        public Parameter(BuiltInParameter bip) => BuiltInParameter = bip;
        public Parameter(ElementId id) => Id = id;
        public Parameter(Guid guid) => Guid = guid;
        public Parameter(string name) => Name = name;

        public int AsInteger() => _intValue;
        public double AsDouble() => _doubleValue;
        public string AsString() => _stringValue;
        public ElementId AsElementId() => _elementIdValue;

        public bool Set(int value)
        {
            if (IsReadOnly) return false;
            _intValue = value;
            StorageType = StorageType.Integer;
            return true;
        }

        public bool Set(double value)
        {
            if (IsReadOnly) return false;
            _doubleValue = value;
            StorageType = StorageType.Double;
            return true;
        }

        public bool Set(string value)
        {
            if (IsReadOnly) return false;
            _stringValue = value;
            StorageType = StorageType.String;
            return true;
        }

        public bool Set(ElementId value)
        {
            if (IsReadOnly) return false;
            _elementIdValue = value;
            StorageType = StorageType.ElementId;
            return true;
        }

        public bool SetValueString(string value) => Set(value);
        public string AsValueString()
        {
            return StorageType switch
            {
                StorageType.Integer => _intValue.ToString(),
                StorageType.Double => _doubleValue.ToString(),
                StorageType.String => _stringValue,
#if REVIT2024_OR_ABOVE
                StorageType.ElementId => _elementIdValue?.Value.ToString(),
#else
                StorageType.ElementId => _elementIdValue?.IntegerValue.ToString(),
#endif
                _ => string.Empty,
            };
        }

        public void Dispose() => IsDisposed = true;
    }

    public class ParameterSet : System.Collections.Generic.List<Parameter>
    {
    }

}
