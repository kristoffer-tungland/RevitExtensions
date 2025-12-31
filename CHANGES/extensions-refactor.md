# C# 14 extension member refactor mapping

This file captures the signature changes for all extension members migrated to C# 14 extension syntax, including property conversions.

## RevitExtensions/EditStatus.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static string ToFriendlyString(this EditStatus status)` | `extension (EditStatus status) { public string FriendlyName { get; } }` | Property-like derived label. | None |

## RevitExtensions/ElementExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static long GetElementIdValue(this Element element)` | `extension (Element element) { public long IdValue { get; } }` | Parameterless derived value. | None |
| `public static long GetElementIdValue(this ElementId id)` | `extension (ElementId id) { public long Value { get; } }` | Parameterless derived value. | Declared only under `#if !REVIT2025_OR_ABOVE`; uses `#if REVIT2024_OR_ABOVE`/`#else` inside getter. |
| `public static Element? ToElement(this ElementId id, Document document)` | `extension (ElementId id) { public Element? ToElement(Document document) }` | Parameterized lookup. | None |
| `public static T? ToElement<T>(this ElementId id, Document document) where T : Element` | `extension (ElementId id) { public T? ToElement<T>(Document document) where T : Element }` | Parameterized/generic lookup. | None |
| `public static bool CanEdit(this Element element)` | `extension (Element element) { public bool IsEditable { get; } }` | Boolean check without parameters. | Uses `#if REVIT2020_OR_ABOVE` and `#if REVIT2024_OR_ABOVE` logic via `CanEdit(out ...)`. |
| `public static bool CanEdit(this Element element, out EditStatus status)` | `extension (Element element) { public bool CanEdit(out EditStatus status) }` | Operation with out parameter. | `#if REVIT2020_OR_ABOVE` and `#if REVIT2024_OR_ABOVE` branches preserved. |
| `public static Element? GetElementType(this Element element)` | `extension (Element element) { public Element? ElementType { get; } }` | Parameterless derived value. | None |
| `public static void SetWorkset(this Element element, Workset workset)` | `extension (Element element) { public void SetWorkset(Workset workset) }` | State-changing operation. | None |
| `public static void SetWorkset(this Element element, WorksetId worksetId)` | `extension (Element element) { public void SetWorkset(WorksetId worksetId) }` | State-changing operation. | None |

## RevitExtensions/ParameterExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static Parameter? GetParameter(this Element element, string identifier)` | `extension (Element element) { public Parameter? GetParameter(string identifier) }` | Parameterized lookup. | None |
| `public static Parameter? GetParameter(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public Parameter? GetParameter(ParameterIdentifier identifier) }` | Parameterized lookup. | None |
| `public static Parameter? LookupParameter(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public Parameter? LookupParameter(ParameterIdentifier identifier) }` | Parameterized lookup. | None |
| `public static Parameter? LookupParameter(this Element element, string identifier)` | `extension (Element element) { public Parameter? LookupParameter(string identifier) }` | Parameterized lookup. | None |
| `public static object? GetParameterValue(this Element element, string identifier)` | `extension (Element element) { public object? GetParameterValue(string identifier) }` | Parameterized query. | None |
| `public static object? GetParameterValue(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public object? GetParameterValue(ParameterIdentifier identifier) }` | Parameterized query. | None |
| `public static ParameterValueDetailed? GetValueDetailed(this Element element, string identifier)` | `extension (Element element) { public ParameterValueDetailed? GetValueDetailed(string identifier) }` | Parameterized query. | None |
| `public static ParameterValueDetailed? GetValueDetailed(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public ParameterValueDetailed? GetValueDetailed(ParameterIdentifier identifier) }` | Parameterized query. | None |
| `public static T GetParameterValue<T>(this Element element, string identifier)` | `extension (Element element) { public T GetParameterValue<T>(string identifier) }` | Parameterized/generic query. | None |
| `public static T GetParameterValue<T>(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public T GetParameterValue<T>(ParameterIdentifier identifier) }` | Parameterized/generic query. | None |
| `public static object? LookupParameterValue(this Element element, string identifier)` | `extension (Element element) { public object? LookupParameterValue(string identifier) }` | Parameterized query. | None |
| `public static object? LookupParameterValue(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public object? LookupParameterValue(ParameterIdentifier identifier) }` | Parameterized query. | None |
| `public static T LookupParameterValue<T>(this Element element, string identifier)` | `extension (Element element) { public T LookupParameterValue<T>(string identifier) }` | Parameterized/generic query. | None |
| `public static T LookupParameterValue<T>(this Element element, ParameterIdentifier identifier)` | `extension (Element element) { public T LookupParameterValue<T>(ParameterIdentifier identifier) }` | Parameterized/generic query. | None |
| `public static void SetParameterValue(this Element element, string identifier, object value)` | `extension (Element element) { public void SetParameterValue(string identifier, object value) }` | State-changing operation. | None |
| `public static void SetParameterValue(this Element element, ParameterIdentifier identifier, object value)` | `extension (Element element) { public void SetParameterValue(ParameterIdentifier identifier, object value) }` | State-changing operation. | None |
| `public static bool TrySetParameterValue(this Element element, string identifier, object value, out string? reason)` | `extension (Element element) { public bool TrySetParameterValue(string identifier, object value, out string? reason) }` | State-changing operation with out parameter. | None |
| `public static bool TrySetParameterValue(this Element element, ParameterIdentifier identifier, object value, out string? reason)` | `extension (Element element) { public bool TrySetParameterValue(ParameterIdentifier identifier, object value, out string? reason) }` | State-changing operation with out parameter. | None |
| `public static object? GetParameterValue(this Parameter parameter)` | `extension (Parameter parameter) { public object? Value { get; } }` | Parameterless derived value. | None |
| `public static ParameterValueDetailed GetValueDetailed(this Parameter parameter)` | `extension (Parameter parameter) { public ParameterValueDetailed ValueDetailed { get; } }` | Parameterless derived value. | None |
| `public static T GetParameterValue<T>(this Parameter parameter)` | `extension (Parameter parameter) { public T GetParameterValue<T>() }` | Generic conversion. | None |
| `public static void SetParameterValue(this Parameter parameter, object value)` | `extension (Parameter parameter) { public void SetParameterValue(object value) }` | State-changing operation. | None |
| `public static bool TrySetParameterValue(this Parameter parameter, object value, out string? reason)` | `extension (Parameter parameter) { public bool TrySetParameterValue(object value, out string? reason) }` | State-changing operation with out parameter. | None |
| `public static ParameterIdentifier ToIdentifier(this Parameter parameter)` | `extension (Parameter parameter) { public ParameterIdentifier Identifier { get; } }` | Parameterless derived identifier. | None |

## RevitExtensions/DocumentExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static FilteredElementCollector Instances(this Document document)` | `extension (Document document) { public FilteredElementCollector Instances() }` | Factory/operation. | None |
| `public static FilteredElementCollector Types(this Document document)` | `extension (Document document) { public FilteredElementCollector Types() }` | Factory/operation. | None |
| `public static FilteredElementCollector InstancesOf<T>(this Document document) where T : Element` | `extension (Document document) { public FilteredElementCollector InstancesOf<T>() where T : Element }` | Parameterized/generic query. | None |
| `public static FilteredElementCollector TypesOf<T>(this Document document) where T : Element` | `extension (Document document) { public FilteredElementCollector TypesOf<T>() where T : Element }` | Parameterized/generic query. | None |
| `public static FilteredElementCollector InstancesOf(this Document document, BuiltInCategory category)` | `extension (Document document) { public FilteredElementCollector InstancesOf(BuiltInCategory category) }` | Parameterized query. | None |
| `public static FilteredElementCollector TypesOf(this Document document, BuiltInCategory category)` | `extension (Document document) { public FilteredElementCollector TypesOf(BuiltInCategory category) }` | Parameterized query. | None |
| `public static FilteredElementCollector InstancesOf(this Document document, IEnumerable<BuiltInCategory> categories)` | `extension (Document document) { public FilteredElementCollector InstancesOf(IEnumerable<BuiltInCategory> categories) }` | Parameterized query. | None |
| `public static FilteredElementCollector TypesOf(this Document document, IEnumerable<BuiltInCategory> categories)` | `extension (Document document) { public FilteredElementCollector TypesOf(IEnumerable<BuiltInCategory> categories) }` | Parameterized query. | None |
| `public static Element? GetElement(this Document document, ElementId id)` | `extension (Document document) { public Element? GetElement(ElementId id) }` | Parameterized lookup. | None |
| `public static Element? GetElement(this Document document, int id)` | `extension (Document document) { public Element? GetElement(int id) }` | Parameterized lookup. | None |
| `public static Element? GetElement(this Document document, long id)` | `extension (Document document) { public Element? GetElement(long id) }` | Parameterized lookup. | None |
| `public static T? GetElement<T>(this Document document, ElementId id) where T : Element` | `extension (Document document) { public T? GetElement<T>(ElementId id) where T : Element }` | Parameterized/generic lookup. | None |
| `public static T? GetElement<T>(this Document document, int id) where T : Element` | `extension (Document document) { public T? GetElement<T>(int id) where T : Element }` | Parameterized/generic lookup. | None |
| `public static T? GetElement<T>(this Document document, long id) where T : Element` | `extension (Document document) { public T? GetElement<T>(long id) where T : Element }` | Parameterized/generic lookup. | None |
| `public static Transaction StartTransaction(this Document document, string name)` | `extension (Document document) { public Transaction StartTransaction(string name) }` | State-changing operation. | None |
| `public static TransactionGroup StartTransactionGroup(this Document document, string name)` | `extension (Document document) { public TransactionGroup StartTransactionGroup(string name) }` | State-changing operation. | None |
| `public static SubTransaction StartSubTransaction(this Document document)` | `extension (Document document) { public SubTransaction StartSubTransaction() }` | State-changing operation. | None |
| `public static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document)` | `extension (Document document) { public IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> AvailableParameters { get; } }` | Parameterless derived data. | None |
| `public static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document, BuiltInCategory category)` | `extension (Document document) { public IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(BuiltInCategory category) }` | Parameterized query. | None |
| `public static IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(this Document document, IEnumerable<BuiltInCategory> categories)` | `extension (Document document) { public IReadOnlyDictionary<ParameterIdentifier, ParameterMetadata> GetAvailableParameters(IEnumerable<BuiltInCategory> categories) }` | Parameterized query. | None |
| `public static ParameterIdentifier? LookupParameterId(this Document document, string identifier)` | `extension (Document document) { public ParameterIdentifier? LookupParameterId(string identifier) }` | Parameterized query. | None |
| `public static ParameterIdentifier? LookupParameterId(this Document document, ParameterIdentifier identifier)` | `extension (Document document) { public ParameterIdentifier? LookupParameterId(ParameterIdentifier identifier) }` | Parameterized query. | None |
| `public static ParameterIdentifier? LookupParameterId(this Document document, string identifier, BuiltInCategory category)` | `extension (Document document) { public ParameterIdentifier? LookupParameterId(string identifier, BuiltInCategory category) }` | Parameterized query. | None |
| `public static ParameterIdentifier? LookupParameterId(this Document document, ParameterIdentifier identifier, BuiltInCategory category)` | `extension (Document document) { public ParameterIdentifier? LookupParameterId(ParameterIdentifier identifier, BuiltInCategory category) }` | Parameterized query. | None |
| `public static List<ParameterMetadata> GetParametersByName(this Document document, string name)` | `extension (Document document) { public List<ParameterMetadata> GetParametersByName(string name) }` | Parameterized query. | None |

## RevitExtensions/DocumentUnitExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static void SetTestLengthUnitScale(this Document document, double scale)` | `extension (Document document) { public void SetTestLengthUnitScale(double scale) }` | State-changing operation. | Uses `#if REVIT2021_OR_LESS` branches for unit APIs. |

## RevitExtensions/ForgeTypeIdExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static DisplayUnitType ToDisplayUnitType(this ForgeTypeId id)` | `extension (ForgeTypeId id) { public DisplayUnitType ToDisplayUnitType() }` | Conversion operation. | Entire file under `#if REVIT2021_OR_LESS`. |
| `public static UnitType ToUnitType(this ForgeTypeId id)` | `extension (ForgeTypeId id) { public UnitType ToUnitType() }` | Conversion operation. | Entire file under `#if REVIT2021_OR_LESS`. |

## RevitExtensions/ParameterElementExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static Definition GetDefinitionSafe(this ParameterElement element)` | `extension (ParameterElement element) { public Definition SafeDefinition { get; } }` | Parameterless derived value. | None |
| `public static Guid? GetGuidSafe(this ParameterElement element)` | `extension (ParameterElement element) { public Guid? SafeGuid { get; } }` | Parameterless derived value. | None |
| `public static bool GetIsInstanceSafe(this ParameterElement element, Document doc)` | `extension (ParameterElement element) { public bool GetIsInstanceSafe(Document doc) }` | Parameterized query. | None |
| `public static IEnumerable<BuiltInCategory> GetCategoriesSafe(this ParameterElement element, Document doc)` | `extension (ParameterElement element) { public IEnumerable<BuiltInCategory> GetCategoriesSafe(Document doc) }` | Parameterized query. | None |

## RevitExtensions/FilteredElementCollectorExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static FilteredElementCollector Instances(this FilteredElementCollector collector)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Instances() }` | Fluent filtering operation. | None |
| `public static FilteredElementCollector Types(this FilteredElementCollector collector)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Types() }` | Fluent filtering operation. | None |
| `public static FilteredElementCollector InstancesOf<T>(this FilteredElementCollector collector) where T : Element` | `extension (FilteredElementCollector collector) { public FilteredElementCollector InstancesOf<T>() where T : Element }` | Parameterized/generic filtering. | None |
| `public static FilteredElementCollector TypesOf<T>(this FilteredElementCollector collector) where T : Element` | `extension (FilteredElementCollector collector) { public FilteredElementCollector TypesOf<T>() where T : Element }` | Parameterized/generic filtering. | None |
| `public static FilteredElementCollector InstancesOf(this FilteredElementCollector collector, BuiltInCategory category)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector InstancesOf(BuiltInCategory category) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector TypesOf(this FilteredElementCollector collector, BuiltInCategory category)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector TypesOf(BuiltInCategory category) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector InstancesOf(this FilteredElementCollector collector, IEnumerable<BuiltInCategory> categories)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector InstancesOf(IEnumerable<BuiltInCategory> categories) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector TypesOf(this FilteredElementCollector collector, IEnumerable<BuiltInCategory> categories)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector TypesOf(IEnumerable<BuiltInCategory> categories) }` | Parameterized filtering. | None |
| `public static void ForEach(this FilteredElementCollector collector, Action<Element> action)` | `extension (FilteredElementCollector collector) { public void ForEach(Action<Element> action) }` | Iteration/action. | None |
| `public static void ForEach<T>(this FilteredElementCollector collector, Action<T> action) where T : Element` | `extension (FilteredElementCollector collector) { public void ForEach<T>(Action<T> action) where T : Element }` | Iteration/action. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, ElementId parameterId, StringComparison comparison, string value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(ElementId parameterId, StringComparison comparison, string value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, BuiltInParameter parameter, StringComparison comparison, string value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(BuiltInParameter parameter, StringComparison comparison, string value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, int value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(ElementId parameterId, Comparison comparison, int value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, BuiltInParameter parameter, Comparison comparison, int value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(BuiltInParameter parameter, Comparison comparison, int value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, double value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(ElementId parameterId, Comparison comparison, double value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, BuiltInParameter parameter, Comparison comparison, double value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(BuiltInParameter parameter, Comparison comparison, double value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, ElementId value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(ElementId parameterId, Comparison comparison, ElementId value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, BuiltInParameter parameter, Comparison comparison, ElementId value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(BuiltInParameter parameter, Comparison comparison, ElementId value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, Document document, string parameterName, StringComparison comparison, string value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(Document document, string parameterName, StringComparison comparison, string value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, Document document, string parameterName, Comparison comparison, int value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(Document document, string parameterName, Comparison comparison, int value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, Document document, string parameterName, Comparison comparison, double value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(Document document, string parameterName, Comparison comparison, double value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, Document document, string parameterName, Comparison comparison, ElementId value)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(Document document, string parameterName, Comparison comparison, ElementId value) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereOr(this FilteredElementCollector collector, params (ElementId parameterId, StringComparison comparison, string value)[] conditions)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereOr(params (ElementId parameterId, StringComparison comparison, string value)[] conditions) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereOr(this FilteredElementCollector collector, ElementId parameterId, StringComparison comparison, IEnumerable<string> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereOr(ElementId parameterId, StringComparison comparison, IEnumerable<string> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereAnd(this FilteredElementCollector collector, params (ElementId parameterId, StringComparison comparison, string value)[] conditions)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereAnd(params (ElementId parameterId, StringComparison comparison, string value)[] conditions) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereAnd(this FilteredElementCollector collector, ElementId parameterId, StringComparison comparison, IEnumerable<string> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereAnd(ElementId parameterId, StringComparison comparison, IEnumerable<string> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereOr(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<int> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereOr(ElementId parameterId, Comparison comparison, IEnumerable<int> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereAnd(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<int> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereAnd(ElementId parameterId, Comparison comparison, IEnumerable<int> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereOr(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<double> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereOr(ElementId parameterId, Comparison comparison, IEnumerable<double> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereAnd(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<double> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereAnd(ElementId parameterId, Comparison comparison, IEnumerable<double> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereOr(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereOr(ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WhereAnd(this FilteredElementCollector collector, ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WhereAnd(ElementId parameterId, Comparison comparison, IEnumerable<ElementId> values) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector WherePasses(this FilteredElementCollector collector, ParameterFilterSet filterSet)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector WherePasses(ParameterFilterSet filterSet) }` | Parameterized filtering. | None |
| `public static FilteredElementCollector Where(this FilteredElementCollector collector, Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder> configure)` | `extension (FilteredElementCollector collector) { public FilteredElementCollector Where(Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder> configure) }` | Parameterized filtering. | None |

## RevitExtensions/ParameterTypeExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ForgeTypeId ToForgeTypeId(this ParameterType pt)` | `extension (ParameterType pt) { public ForgeTypeId ToForgeTypeId() }` | Conversion operation. | Entire file under `#if REVIT2021_OR_LESS`. |

## RevitExtensions/NumericExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ElementId ToElementId(this int value)` | `extension (int value) { public ElementId ToElementId() }` | Conversion operation. | None |
| `public static ElementId ToElementId(this long value)` | `extension (long value) { public ElementId ToElementId() }` | Conversion operation. | Uses `#if REVIT2024_OR_ABOVE` for constructor overload. |
| `public static double ToInternalUnits(this double value, ForgeTypeId unitId)` | `extension (double value) { public double ToInternalUnits(ForgeTypeId unitId) }` | Parameterized conversion. | Uses `#if REVIT2021_OR_LESS` unit API branch. |
| `public static double ToInternalUnits(this int value, ForgeTypeId unitId)` | `extension (int value) { public double ToInternalUnits(ForgeTypeId unitId) }` | Parameterized conversion. | Uses `#if REVIT2021_OR_LESS` unit API branch. |
| `public static double Convert(this double value, ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)` | `extension (double value) { public double Convert(ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId) }` | Parameterized conversion. | Uses `#if REVIT2021_OR_LESS` unit API branch. |
| `public static double Convert(this int value, ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId)` | `extension (int value) { public double Convert(ForgeTypeId currentUnitTypeId, ForgeTypeId desiredUnitTypeId) }` | Parameterized conversion. | None |
| `public static double ConvertFromInternalUnits(this double value, ForgeTypeId unitTypeId)` | `extension (double value) { public double ConvertFromInternalUnits(ForgeTypeId unitTypeId) }` | Parameterized conversion. | Uses `#if REVIT2021_OR_LESS` unit API branch. |
| `public static double ConvertFromInternalUnits(this int value, ForgeTypeId unitTypeId)` | `extension (int value) { public double ConvertFromInternalUnits(ForgeTypeId unitTypeId) }` | Parameterized conversion. | None |

## RevitExtensions/ParameterIdentifierExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ElementId? ToElementId(this ParameterIdentifier identifier)` | `extension (ParameterIdentifier identifier) { public ElementId? ToElementId() }` | Conversion operation. | None |

## RevitExtensions/TransactionExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static void CommitAndEnsure(this Transaction transaction)` | `extension (Transaction transaction) { public void CommitAndEnsure() }` | Operation. | None |
| `public static void AssimilateAndEnsure(this TransactionGroup group)` | `extension (TransactionGroup group) { public void AssimilateAndEnsure() }` | Operation. | None |
| `public static void CommitAndEnsure(this SubTransaction subTransaction)` | `extension (SubTransaction subTransaction) { public void CommitAndEnsure() }` | Operation. | None |

## RevitExtensions/DisplayUnitTypeExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ForgeTypeId ToForgeTypeId(this DisplayUnitType unit)` | `extension (DisplayUnitType unit) { public ForgeTypeId ToForgeTypeId() }` | Conversion operation. | Entire file under `#if REVIT2021_OR_LESS`. |

## RevitExtensions/ParameterDefinitionExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ForgeTypeId GetDataType(this Definition definition)` | `extension (Definition definition) { public ForgeTypeId DataType { get; } }` | Parameterless derived value. | Uses `#if REVIT2021_OR_LESS` to map legacy `ParameterType`. |

## RevitExtensions/SpatialElementExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static void SetRoom(this Element element, Room room, Phase? phase = null)` | `extension (Element element) { public void SetRoom(Room room, Phase? phase = null) }` | State-changing operation. | None |
| `public static void SetRoom(this Element element, XYZ point, Phase? phase = null)` | `extension (Element element) { public void SetRoom(XYZ point, Phase? phase = null) }` | State-changing operation. | None |
| `public static void SetSpace(this Element element, Space space, Phase? phase = null)` | `extension (Element element) { public void SetSpace(Space space, Phase? phase = null) }` | State-changing operation. | None |
| `public static void SetSpace(this Element element, XYZ point, Phase? phase = null)` | `extension (Element element) { public void SetSpace(XYZ point, Phase? phase = null) }` | State-changing operation. | None |
| `public static Room? GetRoom(this Element element, Phase? phase = null)` | `extension (Element element) { public Room? GetRoom(Phase? phase = null) }` | Parameterized query. | None |
| `public static Space? GetSpace(this Element element, Phase? phase = null)` | `extension (Element element) { public Space? GetSpace(Phase? phase = null) }` | Parameterized query. | None |
| `public static SpatialElement? GetRoomOrSpace(this Element element, Phase? phase = null)` | `extension (Element element) { public SpatialElement? GetRoomOrSpace(Phase? phase = null) }` | Parameterized query. | None |
| `public static IReadOnlyDictionary<Phase, ElementId> GetStoredRoomIds(this Element element)` | `extension (Element element) { public IReadOnlyDictionary<Phase, ElementId> StoredRoomIds { get; } }` | Parameterless derived value. | None |
| `public static IReadOnlyDictionary<Phase, ElementId> GetStoredSpaceIds(this Element element)` | `extension (Element element) { public IReadOnlyDictionary<Phase, ElementId> StoredSpaceIds { get; } }` | Parameterless derived value. | None |
| `public static IReadOnlyDictionary<Phase, XYZ> GetStoredRoomPoints(this Element element)` | `extension (Element element) { public IReadOnlyDictionary<Phase, XYZ> StoredRoomPoints { get; } }` | Parameterless derived value. | None |
| `public static IReadOnlyDictionary<Phase, XYZ> GetStoredSpacePoints(this Element element)` | `extension (Element element) { public IReadOnlyDictionary<Phase, XYZ> StoredSpacePoints { get; } }` | Parameterless derived value. | None |
| `public static bool HasStoredRoomOrSpace(this Element element)` | `extension (Element element) { public bool HasStoredRoomOrSpace { get; } }` | Parameterless derived boolean. | None |

## RevitExtensions/BuiltInParameterExtensions.cs

| Old signature | New signature | Reason (property vs method) | Conditional compilation notes |
| --- | --- | --- | --- |
| `public static ElementId ToElementId(this BuiltInParameter parameter)` | `extension (BuiltInParameter parameter) { public ElementId ToElementId() }` | Conversion operation. | None |
