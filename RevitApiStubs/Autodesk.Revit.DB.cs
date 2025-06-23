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

        public override bool Equals(object? obj)
        {
            if (obj is ElementId other)
            {
#if REVIT2024_OR_ABOVE
                return Value == other.Value;
#else
                return IntegerValue == other.IntegerValue;
#endif
            }
            return false;
        }

        public override int GetHashCode()
        {
#if REVIT2024_OR_ABOVE
            return Value.GetHashCode();
#else
            return IntegerValue.GetHashCode();
#endif
        }
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

        private static long GetIdValue(ElementId id)
        {
#if REVIT2024_OR_ABOVE
            return id?.Value ?? 0;
#else
            return id?.IntegerValue ?? 0;
#endif
        }

        public Parameter get_Parameter(BuiltInParameter parameter)
        {
            foreach (var p in Parameters)
            {
                if (p.BuiltInParameter == parameter)
                    return p;
            }
            return null;
        }

        public Parameter get_Parameter(ElementId id)
        {
            if (id == null) return null;
            var value = GetIdValue(id);
            foreach (var p in Parameters)
            {
                if (p.Id != null && GetIdValue(p.Id) == value)
                    return p;
            }
            return null;
        }

        public Parameter get_Parameter(Guid guid)
        {
            foreach (var p in Parameters)
            {
                if (p.Guid.HasValue && p.Guid.Value == guid)
                    return p;
            }
            return null;
        }

        public Parameter LookupParameter(string name)
        {
            foreach (var p in Parameters)
            {
                if (p.Definition?.Name == name || p.Name == name)
                    return p;
            }
            return null;
        }

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

        /// <summary>
        /// Gets the parameter bindings in the document keyed by parameter name.
        /// </summary>
        public BindingMap ParameterBindings { get; } = new BindingMap();

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

        public FilteredElementCollector WherePasses(ElementParameterFilter filter)
        {
            return WherePasses((ElementFilter)filter);
        }

        public FilteredElementCollector WherePasses(ElementFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            _elements.RemoveAll(e => !filter.PassesFilter(e));
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

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Definition.
    /// </summary>
    public class Definition
    {
        public string Name { get; set; }
    }

    public class Parameter : IDisposable
    {
        public BuiltInParameter? BuiltInParameter { get; }
        public ElementId Id { get; }
        public Guid? Guid { get; }
        public Guid? GUID => Guid;
        public string Name { get; }
        public Definition Definition { get; }
        public bool IsDisposed { get; private set; }
        public StorageType StorageType { get; set; }
        public bool IsReadOnly { get; set; }

        private int _intValue;
        private double _doubleValue;
        private string _stringValue;
        private ElementId _elementIdValue;

        public Parameter(BuiltInParameter bip)
        {
            BuiltInParameter = bip;
            Id = new ElementId((int)bip);
            Definition = new Definition { Name = bip.ToString() };
        }

        public Parameter(ElementId id)
        {
            Id = id;
            Definition = new Definition();
        }

        public Parameter(Guid guid)
        {
            Guid = guid;
            Definition = new Definition();
        }

        public Parameter(string name)
        {
            Name = name;
            Definition = new Definition { Name = name };
        }

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

    /// <summary>
    /// Simple map of parameter names to ids used to simulate Document.ParameterBindings.
    /// </summary>
    public class BindingMap : System.Collections.Generic.Dictionary<string, ElementId>
    {
        public BindingMap() : base(System.StringComparer.OrdinalIgnoreCase) { }
    }

    /// <summary>
    /// Base class for element filters used by <see cref="FilteredElementCollector"/>.
    /// </summary>
    public abstract class ElementFilter
    {
        public abstract bool PassesFilter(Element element);
    }

    /// <summary>
    /// Base class for parameter filter rules.
    /// </summary>
    public abstract class FilterRule
    {
        public ElementId ParameterId { get; }

        protected FilterRule(ElementId parameterId)
        {
            ParameterId = parameterId;
        }

        public abstract bool Evaluate(Element element);
    }

    /// <summary>
    /// Factory creating parameter filter rules.
    /// </summary>
    public static class ParameterFilterRuleFactory
    {
        private abstract class StringRule : FilterRule
        {
            protected readonly string Value;

            protected StringRule(ElementId id, string value) : base(id)
            {
                Value = value;
            }

            protected string? Get(Element element)
            {
                using var param = element.get_Parameter(ParameterId);
                return param?.AsString();
            }
        }

        private class EqualsRule : StringRule
        {
            public EqualsRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return string.Equals(s, Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class NotEqualsRule : StringRule
        {
            public NotEqualsRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return !string.Equals(s, Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class ContainsRule : StringRule
        {
            public ContainsRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s != null && s.IndexOf(Value, System.StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private class NotContainsRule : StringRule
        {
            public NotContainsRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s == null || s.IndexOf(Value, System.StringComparison.OrdinalIgnoreCase) < 0;
            }
        }

        private class BeginsWithRule : StringRule
        {
            public BeginsWithRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s != null && s.StartsWith(Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class NotBeginsWithRule : StringRule
        {
            public NotBeginsWithRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s == null || !s.StartsWith(Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class EndsWithRule : StringRule
        {
            public EndsWithRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s != null && s.EndsWith(Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class NotEndsWithRule : StringRule
        {
            public NotEndsWithRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return s == null || !s.EndsWith(Value, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        private class GreaterRule : StringRule
        {
            public GreaterRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return string.Compare(s, Value, System.StringComparison.OrdinalIgnoreCase) > 0;
            }
        }

        private class GreaterOrEqualRule : StringRule
        {
            public GreaterOrEqualRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return string.Compare(s, Value, System.StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private class LessRule : StringRule
        {
            public LessRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return string.Compare(s, Value, System.StringComparison.OrdinalIgnoreCase) < 0;
            }
        }

        private class LessOrEqualRule : StringRule
        {
            public LessOrEqualRule(ElementId id, string value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var s = Get(element);
                return string.Compare(s, Value, System.StringComparison.OrdinalIgnoreCase) <= 0;
            }
        }

        public static FilterRule CreateEqualsRule(ElementId parameter, string value) => new EqualsRule(parameter, value);
        public static FilterRule CreateEqualsRule(ElementId parameter, string value, bool caseSensitive) => new EqualsRule(parameter, value);
        public static FilterRule CreateNotEqualsRule(ElementId parameter, string value) => new NotEqualsRule(parameter, value);
        public static FilterRule CreateNotEqualsRule(ElementId parameter, string value, bool caseSensitive) => new NotEqualsRule(parameter, value);
        public static FilterRule CreateContainsRule(ElementId parameter, string value) => new ContainsRule(parameter, value);
        public static FilterRule CreateContainsRule(ElementId parameter, string value, bool caseSensitive) => new ContainsRule(parameter, value);
        public static FilterRule CreateNotContainsRule(ElementId parameter, string value) => new NotContainsRule(parameter, value);
        public static FilterRule CreateNotContainsRule(ElementId parameter, string value, bool caseSensitive) => new NotContainsRule(parameter, value);
        public static FilterRule CreateBeginsWithRule(ElementId parameter, string value) => new BeginsWithRule(parameter, value);
        public static FilterRule CreateBeginsWithRule(ElementId parameter, string value, bool caseSensitive) => new BeginsWithRule(parameter, value);
        public static FilterRule CreateNotBeginsWithRule(ElementId parameter, string value) => new NotBeginsWithRule(parameter, value);
        public static FilterRule CreateNotBeginsWithRule(ElementId parameter, string value, bool caseSensitive) => new NotBeginsWithRule(parameter, value);
        public static FilterRule CreateEndsWithRule(ElementId parameter, string value) => new EndsWithRule(parameter, value);
        public static FilterRule CreateEndsWithRule(ElementId parameter, string value, bool caseSensitive) => new EndsWithRule(parameter, value);
        public static FilterRule CreateNotEndsWithRule(ElementId parameter, string value) => new NotEndsWithRule(parameter, value);
        public static FilterRule CreateNotEndsWithRule(ElementId parameter, string value, bool caseSensitive) => new NotEndsWithRule(parameter, value);
        public static FilterRule CreateGreaterRule(ElementId parameter, string value) => new GreaterRule(parameter, value);
        public static FilterRule CreateGreaterRule(ElementId parameter, string value, bool caseSensitive) => new GreaterRule(parameter, value);
        public static FilterRule CreateGreaterOrEqualRule(ElementId parameter, string value) => new GreaterOrEqualRule(parameter, value);
        public static FilterRule CreateGreaterOrEqualRule(ElementId parameter, string value, bool caseSensitive) => new GreaterOrEqualRule(parameter, value);
        public static FilterRule CreateLessRule(ElementId parameter, string value) => new LessRule(parameter, value);
        public static FilterRule CreateLessRule(ElementId parameter, string value, bool caseSensitive) => new LessRule(parameter, value);
        public static FilterRule CreateLessOrEqualRule(ElementId parameter, string value) => new LessOrEqualRule(parameter, value);
        public static FilterRule CreateLessOrEqualRule(ElementId parameter, string value, bool caseSensitive) => new LessOrEqualRule(parameter, value);

        private abstract class IntRule : FilterRule
        {
            protected readonly int Value;

            protected IntRule(ElementId id, int value) : base(id)
            {
                Value = value;
            }

            protected int? Get(Element element)
            {
                using var param = element.get_Parameter(ParameterId);
                return param?.AsInteger();
            }
        }

        private class IntEqualsRule : IntRule
        {
            public IntEqualsRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                return Get(element) == Value;
            }
        }

        private class IntNotEqualsRule : IntRule
        {
            public IntNotEqualsRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                return Get(element) != Value;
            }
        }

        private class IntGreaterRule : IntRule
        {
            public IntGreaterRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value > Value;
            }
        }

        private class IntGreaterOrEqualRule : IntRule
        {
            public IntGreaterOrEqualRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value >= Value;
            }
        }

        private class IntLessRule : IntRule
        {
            public IntLessRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value < Value;
            }
        }

        private class IntLessOrEqualRule : IntRule
        {
            public IntLessOrEqualRule(ElementId id, int value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value <= Value;
            }
        }

        public static FilterRule CreateEqualsRule(ElementId parameter, int value) => new IntEqualsRule(parameter, value);
        public static FilterRule CreateNotEqualsRule(ElementId parameter, int value) => new IntNotEqualsRule(parameter, value);
        public static FilterRule CreateGreaterRule(ElementId parameter, int value) => new IntGreaterRule(parameter, value);
        public static FilterRule CreateGreaterOrEqualRule(ElementId parameter, int value) => new IntGreaterOrEqualRule(parameter, value);
        public static FilterRule CreateLessRule(ElementId parameter, int value) => new IntLessRule(parameter, value);
        public static FilterRule CreateLessOrEqualRule(ElementId parameter, int value) => new IntLessOrEqualRule(parameter, value);

        private abstract class DoubleRule : FilterRule
        {
            protected readonly double Value;
            protected readonly double Tolerance;

            protected DoubleRule(ElementId id, double value, double tolerance) : base(id)
            {
                Value = value;
                Tolerance = tolerance;
            }

            protected double? Get(Element element)
            {
                using var param = element.get_Parameter(ParameterId);
                return param?.AsDouble();
            }
        }

        private class DoubleEqualsRule : DoubleRule
        {
            public DoubleEqualsRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && System.Math.Abs(v.Value - Value) <= Tolerance;
            }
        }

        private class DoubleNotEqualsRule : DoubleRule
        {
            public DoubleNotEqualsRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return !v.HasValue || System.Math.Abs(v.Value - Value) > Tolerance;
            }
        }

        private class DoubleGreaterRule : DoubleRule
        {
            public DoubleGreaterRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value > Value;
            }
        }

        private class DoubleGreaterOrEqualRule : DoubleRule
        {
            public DoubleGreaterOrEqualRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value >= Value;
            }
        }

        private class DoubleLessRule : DoubleRule
        {
            public DoubleLessRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value < Value;
            }
        }

        private class DoubleLessOrEqualRule : DoubleRule
        {
            public DoubleLessOrEqualRule(ElementId id, double value, double tol) : base(id, value, tol) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v.HasValue && v.Value <= Value;
            }
        }

        public static FilterRule CreateEqualsRule(ElementId parameter, double value, double tolerance) => new DoubleEqualsRule(parameter, value, tolerance);
        public static FilterRule CreateNotEqualsRule(ElementId parameter, double value, double tolerance) => new DoubleNotEqualsRule(parameter, value, tolerance);
        public static FilterRule CreateGreaterRule(ElementId parameter, double value, double tolerance) => new DoubleGreaterRule(parameter, value, tolerance);
        public static FilterRule CreateGreaterOrEqualRule(ElementId parameter, double value, double tolerance) => new DoubleGreaterOrEqualRule(parameter, value, tolerance);
        public static FilterRule CreateLessRule(ElementId parameter, double value, double tolerance) => new DoubleLessRule(parameter, value, tolerance);
        public static FilterRule CreateLessOrEqualRule(ElementId parameter, double value, double tolerance) => new DoubleLessOrEqualRule(parameter, value, tolerance);

        private abstract class ElementIdRule : FilterRule
        {
            protected readonly ElementId Value;

            protected ElementIdRule(ElementId id, ElementId valueId) : base(id)
            {
                Value = valueId;
            }

            protected ElementId? Get(Element element)
            {
                using var param = element.get_Parameter(ParameterId);
                return param?.AsElementId();
            }
        }

        private class IdEqualsRule : ElementIdRule
        {
            public IdEqualsRule(ElementId id, ElementId value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v != null && v.Equals(Value);
            }
        }

        private class IdNotEqualsRule : ElementIdRule
        {
            public IdNotEqualsRule(ElementId id, ElementId value) : base(id, value) { }

            public override bool Evaluate(Element element)
            {
                var v = Get(element);
                return v == null || !v.Equals(Value);
            }
        }

        public static FilterRule CreateEqualsRule(ElementId parameter, ElementId value) => new IdEqualsRule(parameter, value);
        public static FilterRule CreateNotEqualsRule(ElementId parameter, ElementId value) => new IdNotEqualsRule(parameter, value);
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.ElementParameterFilter.
    /// </summary>
    public class ElementParameterFilter : ElementFilter
    {
        private readonly System.Collections.Generic.IList<FilterRule> _rules;

        public ElementParameterFilter(FilterRule rule)
        {
            _rules = new[] { rule };
        }

        public ElementParameterFilter(System.Collections.Generic.IList<FilterRule> rules)
        {
            _rules = rules;
        }

        public override bool PassesFilter(Element element)
        {
            foreach (var rule in _rules)
            {
                if (!rule.Evaluate(element))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.LogicalOrFilter.
    /// </summary>
    public class LogicalOrFilter : ElementFilter
    {
        public System.Collections.Generic.IList<ElementFilter> Filters { get; }

        public LogicalOrFilter(System.Collections.Generic.IList<ElementFilter> filters)
        {
            Filters = filters;
        }

        public override bool PassesFilter(Element element)
        {
            foreach (var filter in Filters)
            {
                if (filter.PassesFilter(element))
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.LogicalAndFilter.
    /// </summary>
    public class LogicalAndFilter : ElementFilter
    {
        public System.Collections.Generic.IList<ElementFilter> Filters { get; }

        public LogicalAndFilter(System.Collections.Generic.IList<ElementFilter> filters)
        {
            Filters = filters;
        }

        public override bool PassesFilter(Element element)
        {
            foreach (var filter in Filters)
            {
                if (!filter.PassesFilter(element))
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Transaction.
    /// </summary>
    public class Transaction : IDisposable
    {
        public Document Document { get; }
        public string Name { get; }

        public bool IsDisposed { get; private set; }
        public bool IsStarted { get; private set; }

        public Transaction(Document document, string name)
        {
            Document = document;
            Name = name;
        }

        public TransactionStatus Start()
        {
            IsStarted = true;
            return TransactionStatus.Started;
        }

        public TransactionStatus Commit()
        {
            if (!IsStarted)
                return TransactionStatus.Error;

            IsStarted = false;
            return TransactionStatus.Committed;
        }

        public void Dispose() => IsDisposed = true;
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.TransactionStatus.
    /// </summary>
    public enum TransactionStatus
    {
        Uninitialized,
        Started,
        Committed,
        RolledBack,
        Error
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.TransactionGroup.
    /// </summary>
    public class TransactionGroup : IDisposable
    {
        public Document Document { get; }
        public string Name { get; }

        public bool IsDisposed { get; private set; }
        public bool IsStarted { get; private set; }

        public TransactionGroup(Document document, string name)
        {
            Document = document;
            Name = name;
        }

        public TransactionStatus Start()
        {
            IsStarted = true;
            return TransactionStatus.Started;
        }

        public TransactionStatus Assimilate()
        {
            if (!IsStarted)
                return TransactionStatus.Error;

            IsStarted = false;
            return TransactionStatus.Committed;
        }

        public void Dispose() => IsDisposed = true;
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.SubTransaction.
    /// </summary>
    public class SubTransaction : IDisposable
    {
        public Document Document { get; }

        public bool IsDisposed { get; private set; }
        public bool IsStarted { get; private set; }

        public SubTransaction(Document document)
        {
            Document = document;
        }

        public TransactionStatus Start()
        {
            IsStarted = true;
            return TransactionStatus.Started;
        }

        public TransactionStatus Commit()
        {
            if (!IsStarted)
                return TransactionStatus.Error;

            IsStarted = false;
            return TransactionStatus.Committed;
        }

        public void Dispose() => IsDisposed = true;
    }

}
