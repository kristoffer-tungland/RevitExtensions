# RevitExtensions

Helper extensions for the Autodesk Revit API. The library smooths over differences between Revit versions so code can use a consistent set of helpers.

## Available extensions

All extension methods live in the `RevitExtensions` namespace. The library exposes helpers for common Revit API patterns.

### DocumentExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – create a `FilteredElementCollector` for element instances or types.
- Overloads allow filtering by a `BuiltInCategory` or multiple categories.
- `GetElement()` overloads retrieve an element by id (`ElementId`, `int` or `long`).
  Generic versions cast the result to the specified element type and may return
  `null` if the id does not exist.
- `StartTransaction`, `StartTransactionGroup` and `StartSubTransaction` start the respective transaction object and ensure it began successfully.

### FilteredElementCollectorExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – filter an existing collector by type.
- Overloads filter by a category or multiple categories.
- `ForEach(Action<Element>)` – enumerates the collector and disposes each element after the action executes.
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

`StringComparison` adds containment and prefix/suffix checks in addition to the equality and greater/less options defined by `Comparison`.

### ElementExtensions

- `GetElementIdValue()` – returns the element id as a `long` regardless of Revit version.
- `ToElement()` – retrieve an element from a document by id. Generic overload
  casts the result to the specified element type.
- `CanEdit(out EditStatus)` – determines if the element can be edited in the current workshared document.
- `GetElementType()` – retrieves the element's type element.

### ParameterExtensions

- `GetParameter` and `LookupParameter` – search for a parameter on an element or its type using a flexible `ParameterIdentifier` or name.
- `GetParameterValue` and `SetParameterValue` – read and write parameter values with automatic type conversion.

### BuiltInParameterExtensions

- `ToElementId()` – convert a built-in parameter enum value to its corresponding `ElementId`.
- `int` and `long` also provide `ToElementId()` helpers.

### TransactionExtensions

- `CommitAndEnsure`, `AssimilateAndEnsure` – commit or assimilate a transaction (or group) and throw if the operation fails.

### ParameterIdentifier

Represents a parameter by name, GUID, built‑in parameter or element id. It can be parsed from a string and provides a stable representation.

## Usage examples

```csharp
using Autodesk.Revit.DB;
using RevitExtensions;

// start a transaction and update a parameter
using var tx = document.StartTransaction("Set parameter");
element.SetParameterValue("Comments", "Hello");
tx.CommitAndEnsure();

// get all wall instances in the document
var walls = document.InstancesOf<Wall>().ToElements();

// retrieve an element id as a long
long id = element.GetElementIdValue();

// filter walls by a parameter value
var exteriorWalls = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .Where(new ElementId(10), StringComparison.Equals, "Exterior")
    .ToElements();

// combine multiple rules
var fireConcrete = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .WhereAnd(
        (new ElementId(20), StringComparison.Contains, "Fire"),
        (new ElementId(21), StringComparison.Equals, "Concrete"))
    .ToElements();

// filter by multiple values for one parameter
var codes = new[] { "A", "B" };
var multi = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .WhereOr(new ElementId(20), StringComparison.Equals, codes)
    .ToElements();

// wildcard string comparison
var fooBar = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .Where(new ElementId(25), StringComparison.Equals, "foo*bar")
    .ToElements();

var fooIsBar = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .Where(new ElementId(25), StringComparison.Equals, "foo*is*bar")
    .ToElements();

// combine sets of filters
var complex = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .Where(b => b
        .AddOr(
            (new ElementId(20), StringComparison.Equals, "A"),
            (new ElementId(20), StringComparison.Equals, "B"))
        .AddRule(new ElementId(21), StringComparison.Equals, "C"))
    .ToElements();

// multiple levels of nested sets
var nested = new FilteredElementCollector(document)
    .InstancesOf<Wall>()
    .Where(b => b
        .AddOr(or => or
            .AddRule(new ElementId(30), StringComparison.Equals, "A")
            .AddAnd(and => and
                .AddRule(new ElementId(31), StringComparison.Equals, "B")
                .AddRule(new ElementId(32), StringComparison.Equals, "C")))
        .AddRule(new ElementId(33), StringComparison.Equals, "D"))
    .ToElements();
```
