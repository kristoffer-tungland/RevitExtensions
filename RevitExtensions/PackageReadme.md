# RevitExtensions

Helper extensions for the Autodesk Revit API. The library smooths over differences between Revit versions so code can use a consistent set of helpers.

## Available extensions

All extension methods live in the `RevitExtensions` namespace. The library exposes helpers for common Revit API patterns.

### DocumentExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – create a `FilteredElementCollector` for element instances or types.
- `Instances()` / `Types()` – create a collector for all element instances or types in the document.
  - Overloads allow filtering by a `BuiltInCategory` or multiple categories.
  - `GetElement()` overloads retrieve an element by id (`ElementId`, `int` or `long`).
    Generic versions cast the result to the specified element type and may return
    `null` if the id does not exist.
- `StartTransaction`, `StartTransactionGroup` and `StartSubTransaction` start the respective transaction object and ensure it began successfully.
- `GetAvailableParameters()` – list project and built‑in parameters available in
  the document. Overloads restrict the search to specific categories.
- `LookupParameterId()` – resolve a parameter identifier by name, id or built‑in value.
- `GetParametersByName()` – find all parameters with a given name.

### FilteredElementCollectorExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – filter an existing collector by type.
- `Instances()` / `Types()` – limit an existing collector to element instances or types.
- Overloads filter by a category or multiple categories.
- `ForEach(Action<Element>)` – enumerates the collector and disposes each element after the action executes.
- `ForEach<T>(Action<T>)` – enumerates the collector and invokes the action for
  elements of type `T`, disposing non-matching elements.
 - `Where(ElementId, StringComparison, string)` – filter by a string parameter value.
 - `Where(BuiltInParameter, StringComparison, string)` – filter by a string parameter using a built-in id.
 - `Where(ElementId, Comparison, int)` – filter by an integer parameter value.
 - `Where(BuiltInParameter, Comparison, int)` – filter an integer parameter using a built-in id.
 - `Where(ElementId, Comparison, double)` – filter by a double parameter value.
 - `Where(BuiltInParameter, Comparison, double)` – filter a double parameter using a built-in id.
 - `Where(ElementId, Comparison, ElementId)` – filter by an element id parameter value.
 - `Where(BuiltInParameter, Comparison, ElementId)` – filter an element id parameter using a built-in id.
- `WhereOr((ElementId, StringComparison, string)[])` – combine rules with logical OR.
- `WhereAnd((ElementId, StringComparison, string)[])` – combine rules with logical AND.
- `WhereOr(ElementId, StringComparison, IEnumerable<string>)` – combine values with OR for a single parameter.
- `WhereAnd(ElementId, StringComparison, IEnumerable<string>)` – combine values with AND for a single parameter.
- `WherePasses(ParameterFilterSet)` – apply a nested set of parameter rules with OR or AND logic.
- `Where(Func<ParameterFilterSetBuilder, ParameterFilterSetBuilder>)` – build a complex parameter filter using a builder callback.

`StringComparison` adds wildcard, containment and prefix/suffix checks in addition to the equality and greater/less options defined by `Comparison`.

### ElementExtensions

- `GetElementIdValue()` – returns the element id as a `long` regardless of Revit version.
- `GetElementIdValue(ElementId)` – retrieve the numeric value of an `ElementId` without depending on the Revit version.
- `ToElement()` – retrieve an element from a document by id. Generic overload
  casts the result to the specified element type.
- `CanEdit(out EditStatus)` – determines if the element can be edited in the current workshared document.
- `GetElementType()` – retrieves the element's type element.

### ParameterExtensions

- `GetParameter` and `LookupParameter` – search for a parameter on an element or its type using a flexible `ParameterIdentifier` or name.
- `GetParameterValue` and `SetParameterValue` – read and write parameter values with automatic type conversion.
- Generic overloads of `GetParameterValue` and `LookupParameterValue` return the requested type directly.
- `TrySetParameterValue` methods expose failure reasons without throwing exceptions.

### BuiltInParameterExtensions

- `ToElementId()` – convert a built-in parameter enum value to its corresponding `ElementId`.
- `int` and `long` also provide `ToElementId()` helpers.

### TransactionExtensions

- `CommitAndEnsure`, `AssimilateAndEnsure` – commit or assimilate a transaction (or group) and throw if the operation fails.

### ParameterIdentifier

Represents a parameter by name, GUID, built‑in parameter or element id. It can be parsed from a string and provides a stable representation.

### ParameterFilterSetBuilder and ParameterFilterSet

Compose complex `ElementFilter` instances using nested AND/OR parameter rules. The builder API mirrors the collector extension methods.

### NumericExtensions

`int` and `long` values include `ToElementId()` helpers for convenience.

### BuiltInParameterCollector

Retrieves metadata about built‑in parameters in a document and caches the information to disk.

### ParameterMetadata

Describes a parameter found in the document, including the categories it applies to and whether it is an instance or type parameter.

## Usage examples

```csharp
using Autodesk.Revit.DB;
using RevitExtensions;
using RevitExtensions.Models;

// update a parameter inside a transaction group
using var group = document.StartTransactionGroup("Modify");
using var tx = document.StartTransaction("Set comment");
var pid = document.LookupParameterId("Comments");
if (pid != null)
{
    element.SetParameterValue(pid, "Hello");
}
tx.CommitAndEnsure();
group.AssimilateAndEnsure();

// simple filter by parameter name
document.InstancesOf<Wall>()
    .Where(document, "Fire Rating", StringComparison.Contains, "120")
    .ForEach(w => w.SetParameterValue("HasFireResistant", true));

// complex nested parameter filter
var walls = document.InstancesOf<Wall>()
    .Where(b => b
        .OrSet(
            (new ElementId(20), StringComparison.Equals, "A"),
            (new ElementId(20), StringComparison.Equals, "B"))
        .AndSet(and => and
            .Rule(new ElementId(21), StringComparison.Contains, "Load")
            .Rule(new ElementId(22), StringComparison.Wildcard, "*500*")))
    .ToElements();

// convert ids and check editing status
long idValue = walls[0].Id.GetElementIdValue();
ElementId elementId = idValue.ToElementId();
bool editable = walls[0].CanEdit(out var status);
string statusText = status.ToFriendlyString();
```
