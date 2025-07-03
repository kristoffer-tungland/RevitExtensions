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
    /// Minimal stand-in for Autodesk.Revit.DB.WorksetId.
    /// </summary>
    public class WorksetId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorksetId"/> class.
        /// </summary>
        /// <param name="id">The integer value.</param>
        public WorksetId(int id) => IntegerValue = id;

        /// <summary>
        /// Gets the value of the workset id as an integer.
        /// </summary>
        public int IntegerValue { get; }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Workset.
    /// </summary>
    public class Workset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Workset"/> class.
        /// </summary>
        /// <param name="id">The workset id.</param>
        public Workset(WorksetId id) => Id = id;

        /// <summary>
        /// Gets the workset id.
        /// </summary>
        public WorksetId Id { get; }
    }

#if REVIT2021_OR_LESS
    public enum UnitType
    {
        UT_Length
    }

    public enum DisplayUnitType
    {
        DUT_DECIMAL_FEET,
        DUT_DECIMAL_INCHES,
        DUT_METERS,
        DUT_CENTIMETERS,
        DUT_MILLIMETERS
    }
#else
    public static class UnitTypeId
    {
        public static ForgeTypeId Feet { get; } = new ForgeTypeId("unit:feet");
        public static ForgeTypeId Inches { get; } = new ForgeTypeId("unit:inch");
        public static ForgeTypeId Meters { get; } = new ForgeTypeId("unit:meter");
        public static ForgeTypeId Centimeters { get; } = new ForgeTypeId("unit:centimeter");
        public static ForgeTypeId Millimeters { get; } = new ForgeTypeId("unit:millimeter");
    }
#endif

    public class FormatOptions
    {
#if REVIT2021_OR_LESS
        public DisplayUnitType DisplayUnits { get; set; } = DisplayUnitType.Feet;
#else
        public ForgeTypeId UnitTypeId { get; set; } = Autodesk.Revit.DB.UnitTypeId.Feet;
        public ForgeTypeId GetUnitTypeId() => UnitTypeId;
        public void SetUnitTypeId(ForgeTypeId id) => UnitTypeId = id;
#endif
    }

    public class Units
    {
#if REVIT2021_OR_LESS
        private readonly System.Collections.Generic.Dictionary<UnitType, FormatOptions> _map = new System.Collections.Generic.Dictionary<UnitType, FormatOptions>();
        public FormatOptions GetFormatOptions(UnitType unitType)
        {
            _map.TryGetValue(unitType, out var fo);
            return fo ?? new FormatOptions();
        }
        public void SetFormatOptions(UnitType unitType, FormatOptions options) => _map[unitType] = options;
#else
        private readonly System.Collections.Generic.Dictionary<ForgeTypeId, FormatOptions> _map = new System.Collections.Generic.Dictionary<ForgeTypeId, FormatOptions>();
        public FormatOptions GetFormatOptions(ForgeTypeId spec)
        {
            _map.TryGetValue(spec, out var fo);
            return fo ?? new FormatOptions();
        }
        public System.Collections.Generic.IEnumerable<ForgeTypeId> GetFormatOptionsSpecTypes() => _map.Keys;
        public void SetFormatOptions(ForgeTypeId spec, FormatOptions options) => _map[spec] = options;
#endif
    }

    public static class UnitUtils
    {
#if REVIT2021_OR_LESS
        public static double ConvertToInternalUnits(double value, DisplayUnitType units)
        {
            return units switch
            {
                DisplayUnitType.Feet => value,
                DisplayUnitType.Inches => value / 12.0,
                DisplayUnitType.Meters => value * 3.28083989501312,
                DisplayUnitType.DUT_CENTIMETERS => value * 0.0328083989501312,
                DisplayUnitType.DUT_MILLIMETERS => value * 0.00328083989501312,
                _ => value
            };
        }

        public static double ConvertFromInternalUnits(double value, DisplayUnitType units)
        {
            return units switch
            {
                DisplayUnitType.Feet => value,
                DisplayUnitType.Inches => value * 12.0,
                DisplayUnitType.Meters => value / 3.28083989501312,
                DisplayUnitType.DUT_CENTIMETERS => value / 0.0328083989501312,
                DisplayUnitType.DUT_MILLIMETERS => value / 0.00328083989501312,
                _ => value
            };
        }

        public static double Convert(double value, DisplayUnitType current, DisplayUnitType desired)
        {
            var internalValue = ConvertToInternalUnits(value, current);
            return ConvertFromInternalUnits(internalValue, desired);
        }
#else
        public static double ConvertToInternalUnits(double value, ForgeTypeId unitTypeId)
        {
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Feet) return value;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Inches) return value / 12.0;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Meters) return value * 3.28083989501312;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Centimeters) return value * 0.0328083989501312;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Millimeters) return value * 0.00328083989501312;
            return value;
        }

        public static double ConvertFromInternalUnits(double value, ForgeTypeId unitTypeId)
        {
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Feet) return value;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Inches) return value * 12.0;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Meters) return value / 3.28083989501312;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Centimeters) return value / 0.0328083989501312;
            if (unitTypeId == Autodesk.Revit.DB.UnitTypeId.Millimeters) return value / 0.00328083989501312;
            return value;
        }

        public static double Convert(double value, ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)
        {
            var internalValue = ConvertToInternalUnits(value, currentUnitTypeId);
            return ConvertFromInternalUnits(internalValue, desiredUnitTypeId);
        }
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
        public ParameterSet Parameters { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the element can be modified.
        /// </summary>
        public bool IsModifiable { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether <see cref="Dispose"/> has been called.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public Element(ElementId id) : this(null, id) { }


        public Element(Document document, ElementId id)
        {
            Document = document;
            Id = id;
            Parameters = new ParameterSet(this);
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
        public bool IsModifiable { get; set; } = true;
        public bool IsLinked { get; set; }

        /// <summary>
        /// Gets or sets the application associated with the document.
        /// </summary>
        public Autodesk.Revit.ApplicationServices.Application Application { get; set; } = new Autodesk.Revit.ApplicationServices.Application();

        private readonly System.Collections.Generic.Dictionary<ElementId, string> _owners = new System.Collections.Generic.Dictionary<ElementId, string>();
        private readonly System.Collections.Generic.Dictionary<long, Element> _elements = new System.Collections.Generic.Dictionary<long, Element>();
        private readonly System.Collections.Generic.Dictionary<XYZ, Autodesk.Revit.DB.Architecture.Room> _rooms = new System.Collections.Generic.Dictionary<XYZ, Autodesk.Revit.DB.Architecture.Room>();
        private readonly System.Collections.Generic.Dictionary<XYZ, Autodesk.Revit.DB.Mechanical.Space> _spaces = new System.Collections.Generic.Dictionary<XYZ, Autodesk.Revit.DB.Mechanical.Space>();

        /// <summary>
        /// Gets the parameter bindings in the document keyed by parameter name.
        /// </summary>
        public BindingMap ParameterBindings { get; } = new BindingMap();

        public Settings Settings { get; } = new Settings();

        public Units Units { get; } = new Units();

        public Units GetUnits() => Units;

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

        internal System.Collections.Generic.IEnumerable<Element> GetElements()
        {
            return _elements.Values;
        }

        /// <summary>
        /// Retrieves a room located at the given point.
        /// </summary>
        /// <param name="point">Point to look up.</param>
        /// <returns>The room at that point or <c>null</c>.</returns>
        public Autodesk.Revit.DB.Architecture.Room GetRoomAtPoint(XYZ point)
        {
            _rooms.TryGetValue(point, out var room);
            return room;
        }

        /// <summary>
        /// Retrieves a space located at the given point.
        /// </summary>
        /// <param name="point">Point to look up.</param>
        /// <returns>The space at that point or <c>null</c>.</returns>
        public Autodesk.Revit.DB.Mechanical.Space GetSpaceAtPoint(XYZ point)
        {
            _spaces.TryGetValue(point, out var space);
            return space;
        }

        /// <summary>
        /// Adds a room at the specified point for tests that perform lookups.
        /// </summary>
        /// <param name="room">The room to register.</param>
        /// <param name="point">The lookup location.</param>
        public void AddRoom(Autodesk.Revit.DB.Architecture.Room room, XYZ point)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            _rooms[point] = room;
        }

        /// <summary>
        /// Adds a space at the specified point for tests that perform lookups.
        /// </summary>
        /// <param name="space">The space to register.</param>
        /// <param name="point">The lookup location.</param>
        public void AddSpace(Autodesk.Revit.DB.Mechanical.Space space, XYZ point)
        {
            if (space == null) throw new ArgumentNullException(nameof(space));
            _spaces[point] = space;
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
            if (document != null)
            {
                _elements.AddRange(document.GetElements());
            }
        }

        public void AddElement(Element element) => _elements.Add(element);

        public System.Collections.Generic.IEnumerator<Element> GetEnumerator() => _elements.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public FilteredElementCollector OfClass(Type type)
        {
            FilterType = type;
            _elements.RemoveAll(e => e != null && !type.IsAssignableFrom(e.GetType()));
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

    public enum CategoryType
    {
        Model,
        Annotation,
        Tags,
        Internal,
        AnalyticalModel,
    }

    public class Category
    {
        public ElementId Id { get; }
        public BuiltInCategory BuiltInCategory { get; }
        public string Name { get; set; }
        public CategoryType CategoryType { get; set; }
        public bool IsVisibleInUI { get; set; } = true;

        public Category(BuiltInCategory id)
        {
            BuiltInCategory = id;
            Id = new ElementId((int)id);
            Name = id.ToString();
            CategoryType = CategoryType.Model;
        }
    }

    public class Categories : System.Collections.Generic.List<Category>
    {
        public Category get_Item(BuiltInCategory id) =>
            this.Find(c => c.BuiltInCategory == id);
    }

    public class Settings
    {
        public Categories Categories { get; } = new Categories();
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

    public enum BuiltInParameter
    {
        /// <summary>
        /// Identifies the workset parameter on an element.
        /// </summary>
        ELEM_PARTITION_PARAM = -201
    }

    public enum StorageType
    {
        None,
        Integer,
        Double,
        String,
        ElementId,
    }

#if REVIT2021_OR_LESS
    /// <summary>
    /// Simplified list of parameter data types used in older Revit versions.
    /// Only members required by tests are included.
    /// </summary>
    public enum ParameterType
    {
        Text,
        MultilineText,
        URL,
        Integer,
        YesNo,
        Material,
        Number,
        FixtureUnit,
        Length,
        Area,
        Volume,
        Angle,
        HVACDensity,
        HVACPower,
        HVACTemperature,
        HVACAirflow,
        ElectricalCurrent,
        ElectricalPower,
        Custom
    }
#endif

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.Definition.
    /// </summary>
    public class Definition
    {
        public string Name { get; set; }
#if REVIT2021_OR_LESS
        public ParameterType ParameterType { get; set; }
#else
        public ForgeTypeId DataType { get; set; } = new ForgeTypeId(string.Empty);

        public ForgeTypeId GetDataType() => DataType;
#endif
    }

    public class Parameter : IDisposable
    {
        public BuiltInParameter? BuiltInParameter { get; }
        public ElementId Id { get; }
        public Guid? Guid { get; }
        public Guid? GUID => Guid;
        public string Name { get; }
        public Definition Definition { get; }
        public Element Element { get; internal set; }
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
        private readonly Element? _owner;

        internal ParameterSet() { }

        internal ParameterSet(Element owner)
        {
            _owner = owner;
        }

        public new void Add(Parameter item)
        {
            if (_owner != null && item != null)
                item.Element = _owner;
            base.Add(item);
        }

        public new void AddRange(System.Collections.Generic.IEnumerable<Parameter> collection)
        {
            if (_owner != null)
            {
                foreach (var p in collection)
                    if (p != null) p.Element = _owner;
            }
            base.AddRange(collection);
        }
    }

    /// <summary>
    /// Simple coordinate representation used by the stubs.
    /// </summary>
    public class XYZ : System.IEquatable<XYZ>
    {
        /// <summary>Gets the X value.</summary>
        public double X { get; }
        /// <summary>Gets the Y value.</summary>
        public double Y { get; }
        /// <summary>Gets the Z value.</summary>
        public double Z { get; }

        /// <summary>Creates a new coordinate.</summary>
        public XYZ(double x, double y, double z)
        {
            X = x; Y = y; Z = z;
        }

        public bool Equals(XYZ other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj) => obj is XYZ xyz && Equals(xyz);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        public override int GetHashCode() => System.HashCode.Combine(X, Y, Z);
#else
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }
#endif
    }

    /// <summary>
    /// Simple phase placeholder used by tests.
    /// </summary>
    public class Phase : Element
    {
        /// <summary>Initializes the phase with an id.</summary>
        public Phase(ElementId id) : base(id) { }
    }

    /// <summary>
    /// Base class for room and space placeholders.
    /// </summary>
    public abstract class SpatialElement : Element
    {
        /// <summary>Gets or sets the phase of the spatial element.</summary>
        public Phase Phase { get; set; }

        /// <summary>Creates a spatial element with the given id.</summary>
        protected SpatialElement(ElementId id) : base(id) { }
    }

    namespace Architecture
    {
        /// <summary>
        /// Minimal room placeholder for tests.
        /// </summary>
        public class Room : SpatialElement
        {
            /// <summary>Creates a room with the given id.</summary>
            public Room(ElementId id) : base(id) { }
        }
    }

    namespace Mechanical
    {
        /// <summary>
        /// Minimal space placeholder for tests.
        /// </summary>
        public class Space : SpatialElement
        {
            /// <summary>Creates a space with the given id.</summary>
            public Space(ElementId id) : base(id) { }
        }
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.DB.ParameterElement.
    /// </summary>
    public class ParameterElement : Element
    {
        public ParameterElement(ElementId id) : base(id) { }

        public Definition Definition { get; set; } = new Definition();

        public Definition GetDefinition() => Definition;

        public bool IsInstance { get; set; }

        public System.Collections.Generic.HashSet<BuiltInCategory> Categories { get; } = new System.Collections.Generic.HashSet<BuiltInCategory>();
    }

    public class SharedParameterElement : ParameterElement
    {
        public SharedParameterElement(ElementId id) : base(id) { }

        public System.Guid? GuidValue { get; set; }
    }

    /// <summary>
    /// Simple map of parameter names to ids used to simulate Document.ParameterBindings.
    /// </summary>
    public class BindingMap : System.Collections.Generic.Dictionary<string, ElementId>
    {
        public BindingMap() : base(System.StringComparer.OrdinalIgnoreCase) { }

        private readonly System.Collections.Generic.Dictionary<Definition, ElementBinding> _bindings =
            new System.Collections.Generic.Dictionary<Definition, ElementBinding>();

        public ElementBinding get_Item(Definition def)
        {
            _bindings.TryGetValue(def, out var binding);
            return binding;
        }

        public void set_Item(Definition def, ElementBinding binding)
        {
            _bindings[def] = binding;
        }
    }

    public class CategorySet : System.Collections.Generic.HashSet<Category>
    {
    }

    public abstract class ElementBinding
    {
        public CategorySet Categories { get; } = new CategorySet();
    }

    public class InstanceBinding : ElementBinding
    {
    }

    public class TypeBinding : ElementBinding
    {
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

        public TransactionStatus RollBack()
        {
            if (!IsStarted)
                return TransactionStatus.Error;

            IsStarted = false;
            return TransactionStatus.RolledBack;
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

        public TransactionStatus RollBack()
        {
            if (!IsStarted)
                return TransactionStatus.Error;

            IsStarted = false;
            return TransactionStatus.RolledBack;
        }

        public void Dispose() => IsDisposed = true;
    }

}

namespace Autodesk.Revit.ApplicationServices
{
    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.ApplicationServices.ControlledApplication.
    /// </summary>
    public class ControlledApplication
    {
        /// <summary>
        /// Gets or sets the primary version number of the application.
        /// </summary>
        public string VersionNumber { get; set; } = "0";
    }

    /// <summary>
    /// Minimal stand-in for Autodesk.Revit.ApplicationServices.Application.
    /// In Revit, this derives from <see cref="ControlledApplication"/>.
    /// </summary>
    public class Application : ControlledApplication
    {
    }
}
