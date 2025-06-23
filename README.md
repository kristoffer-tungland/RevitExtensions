# RevitExtensions

This repository contains helper extensions for the Autodesk Revit API. The goal is to make it easier to use APIs across Revit versions. One important helper is retrieving an element id as a `long` value regardless of the Revit release. The library depends on the `Revit_All_Main_Versions_API_x64` NuGet package and builds against either .NET Framework 4.8 or .NET 8 depending on the Revit version. A tiny `RevitApiStubs` project is included for unit tests so they can run without Autodesk binaries.

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

// get all element types in the document
var allTypes = document.Types().ToElements();

// retrieve an element id as a long
long id = element.GetElementIdValue();

// filter walls by a parameter value
var exteriorWalls = document.InstancesOf<Wall>()
    .Where(new ElementId(10), StringComparison.Equals, "Exterior")
    .ToElements();

// combine multiple rules
var fireConcrete = document.InstancesOf<Wall>()
    .WhereAnd(
        (new ElementId(20), StringComparison.Contains, "Fire"),
        (new ElementId(21), StringComparison.Equals, "Concrete"))
    .ToElements();

// filter by multiple values for one parameter
var codes = new[] { "A", "B" };
var multi = document.InstancesOf<Wall>()
    .WhereOr(new ElementId(20), StringComparison.Equals, codes)
    .ToElements();

// wildcard string comparison
var fooBar = document.InstancesOf<Wall>()
    .Where(new ElementId(25), StringComparison.Wildcard, "foo*bar")
    .ToElements();

var fooIsBar = document.InstancesOf<Wall>()
    .Where(new ElementId(25), StringComparison.Wildcard, "foo*is*bar")
    .ToElements();

// combine sets of filters
var complex = document.InstancesOf<Wall>()
    .Where(b => b
        .OrSet(
            (new ElementId(20), StringComparison.Equals, "A"),
            (new ElementId(20), StringComparison.Equals, "B"))
        .Rule(new ElementId(21), StringComparison.Equals, "C"))
    .ToElements();

// multiple levels of nested sets
var nested = document.InstancesOf<Wall>()
    .Where(b => b
        .OrSet(or => or
            .Rule(new ElementId(30), StringComparison.Equals, "A")
            .AndSet(and => and
                .Rule(new ElementId(31), StringComparison.Equals, "B")
                .Rule(new ElementId(32), StringComparison.Equals, "C")))
        .Rule(new ElementId(33), StringComparison.Equals, "D"))
    .ToElements();
```


## Building packages

Use [NUKE](https://nuke.build) to build and package the project. Compile all
versions with `nuke --target BuildAll` and produce NuGet packages with
`nuke --target Pack`. Packages will be written to the `nupkgs` directory. Pass a
`version` parameter to override the default `0.0.1`, for example
`nuke Pack --version 1.2.3`. The CI workflow uses GitVersion to calculate a
semantic version and forwards it to the build when publishing packages.

Packages use an assembly name that includes the Revit year and the package
version. For example a package built for Revit 2025 will produce an assembly
named `RevitExtensions.2025.0.0.1` when the version is `0.0.1`.
Each NuGet package is published with an id that also includes the Revit year,
such as `RevitExtensions.2024`, and the package version matches the assembly
version (`0.0.1` by default).

The project file selects a default Revit API package that matches each target
framework so a plain `dotnet restore` succeeds without extra properties.

## Running tests

The test project uses the `RevitApiStubs` library so no Autodesk binaries are
required. Run tests with defines for a specific Revit version, for example:

```bash
dotnet test RevitExtensions.sln -c Release \
  -p:UseRevitApiStubs=true \
  -p:DefineConstants=REVIT2026%3BREVIT2026_OR_ABOVE%3BREVIT2025_OR_ABOVE%3BREVIT2024_OR_ABOVE
```

## Available extensions

All extension methods live in the `RevitExtensions` namespace.
The library exposes helpers for common Revit API patterns.

-### DocumentExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – create a `FilteredElementCollector` for
  element instances or types.
- `Instances()` / `Types()` – create a collector for all element instances or
  types in the document.
  - Overloads allow filtering by a `BuiltInCategory` or multiple categories.
  - `GetElement()` overloads retrieve an element by id (`ElementId`, `int` or `long`).
    Generic versions cast the result to the specified element type and may return
    `null` when the id does not exist.
- `StartTransaction`, `StartTransactionGroup` and `StartSubTransaction` start
  the respective transaction object and ensure it began successfully.

### FilteredElementCollectorExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – filter an existing collector by type.
- `Instances()` / `Types()` – limit an existing collector to element instances
  or types.
- Overloads filter by a category or multiple categories.
- `ForEach(Action<Element>)` – enumerates the collector and disposes each
  element after the action executes.
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

- `GetElementIdValue()` – returns the element id as a `long` regardless of
  Revit version.
- `ToElement()` – retrieve an element from a document by id. Generic overload
  casts the result to the specified element type.
- `CanEdit(out EditStatus)` – determines if the element can be edited in the
  current workshared document.
- `GetElementType()` – retrieves the element's type element.

### ParameterExtensions

- `GetParameter` and `LookupParameter` – search for a parameter on an element or
  its type using a flexible `ParameterIdentifier` or name.
- `GetParameterValue` and `SetParameterValue` – read and write parameter values
  with automatic type conversion.

### BuiltInParameterExtensions

- `ToElementId()` – convert a built-in parameter enum value to its corresponding `ElementId`.
- `int` and `long` also provide `ToElementId()` helpers.

### TransactionExtensions

- `CommitAndEnsure`, `AssimilateAndEnsure` – commit or assimilate a transaction
  (or group) and throw if the operation fails.

### ParameterIdentifier

Represents a parameter by name, GUID, built‑in parameter or element id. It can
be parsed from a string and provides a stable representation.

