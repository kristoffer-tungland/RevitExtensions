# RevitExtensions

This repository contains helper extensions for the Autodesk Revit API. The goal is to make it easier to use APIs across Revit versions. One important helper is retrieving an element id as a `long` value regardless of the Revit release. The library depends on the `Revit_All_Main_Versions_API_x64` NuGet package and builds against either .NET Framework 4.8 or .NET 8 depending on the Revit version. A tiny `RevitApiStubs` project is included for unit tests so they can run without Autodesk binaries.

## Building packages

Run `./build.sh` to compile the library for all supported Revit versions.
To produce NuGet packages run `./pack.sh`. Packages will be written to the
`nupkgs` directory. Pass a version as the first argument to override the default
`0.0.1`. The CI workflow uses GitVersion to calculate a semantic version and
forwards it to the script when publishing packages.

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
