# RevitExtensions

This repository contains helper extensions for the Autodesk Revit API. The goal is to make it easier to use APIs across Revit versions. One important helper is retrieving an element id as a `long` value regardless of the Revit release. The library depends on the `Revit_All_Main_Versions_API_x64` NuGet package and builds against either .NET Framework 4.8 or .NET 8 depending on the Revit version. A tiny `RevitApiStubs` project is included for unit tests so they can run without Autodesk binaries.

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

### DocumentExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – create a `FilteredElementCollector` for
  element instances or types.
- Overloads allow filtering by a `BuiltInCategory` or multiple categories.
- `StartTransaction`, `StartTransactionGroup` and `StartSubTransaction` start
  the respective transaction object and ensure it began successfully.

### FilteredElementCollectorExtensions

- `InstancesOf<T>()` / `TypesOf<T>()` – filter an existing collector by type.
- Overloads filter by a category or multiple categories.
- `ForEach(Action<Element>)` – enumerates the collector and disposes each
  element after the action executes.

### ElementExtensions

- `GetElementIdValue()` – returns the element id as a `long` regardless of
  Revit version.
- `CanEdit(out EditStatus)` – determines if the element can be edited in the
  current workshared document.
- `GetElementType()` – retrieves the element's type element.

### ParameterExtensions

- `GetParameter` and `LookupParameter` – search for a parameter on an element or
  its type using a flexible `ParameterIdentifier` or name.
- `GetParameterValue` and `SetParameterValue` – read and write parameter values
  with automatic type conversion.

### TransactionExtensions

- `CommitAndEnsure`, `AssimilateAndEnsure` – commit or assimilate a transaction
  (or group) and throw if the operation fails.

### ParameterIdentifier

Represents a parameter by name, GUID, built‑in parameter or element id. It can
be parsed from a string and provides a stable representation.

### Usage examples

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
