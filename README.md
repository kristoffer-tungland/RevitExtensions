# RevitExtensions

This repository contains helper extensions for the Autodesk Revit API. The goal is to make it easier to use APIs across Revit versions. One important helper is retrieving an element id as a `long` value regardless of the Revit release. The library depends on the `Revit_All_Main_Versions_API_x64` NuGet package and builds against either .NET Framework 4.8 or .NET 8 depending on the Revit version. A tiny `RevitApiStubs` project is included for unit tests so they can run without Autodesk binaries.

## Building packages

Run `./pack.sh` to build NuGet packages for Revit versions 2019 through 2026.
Packages will be written to the `nupkgs` directory.

## Running tests

The test project uses the `RevitApiStubs` library so no Autodesk binaries are
required. Run tests with defines for a specific Revit version, for example:

```bash
dotnet test RevitExtensions.sln -c Release \
  -p:UseRevitApiStubs=true \
  -p:DefineConstants=REVIT2026%3BREVIT2026_OR_ABOVE%3BREVIT2025_OR_ABOVE%3BREVIT2024_OR_ABOVE
```
