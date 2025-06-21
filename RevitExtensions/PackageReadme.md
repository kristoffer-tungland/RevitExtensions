# RevitExtensions

Helper extensions for the Autodesk Revit API. The library smooths over differences between Revit versions so code can use a consistent set of helpers.

## Available extensions

All extension methods live in the `RevitExtensions` namespace. The library exposes helpers for common Revit API patterns.

### DocumentExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – create a `FilteredElementCollector` for element instances or types.
- Overloads allow filtering by a `BuiltInCategory` or multiple categories.
- `StartTransaction`, `StartTransactionGroup` and `StartSubTransaction` start the respective transaction object and ensure it began successfully.

### FilteredElementCollectorExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – filter an existing collector by type.
- Overloads filter by a category or multiple categories.
- `ForEach(Action<Element>)` – enumerates the collector and disposes each element after the action executes.

### ElementExtensions

- `GetElementIdValue()` – returns the element id as a `long` regardless of Revit version.
- `CanEdit(out EditStatus)` – determines if the element can be edited in the current workshared document.
- `GetElementType()` – retrieves the element's type element.

### ParameterExtensions

- `GetParameter` and `LookupParameter` – search for a parameter on an element or its type using a flexible `ParameterIdentifier` or name.
- `GetParameterValue` and `SetParameterValue` – read and write parameter values with automatic type conversion.

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
```
