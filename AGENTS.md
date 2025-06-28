This repository provides extension methods for the Autodesk Revit API. It aligns older releases with newer ones so consumers can use a consistent, higher-level API. A core example is retrieving an element id as a `long` regardless of the Revit version.

## Building
Always compile the library across *all* supported Revit versions using [NUKE](https://nuke.build). Use the build script:

```bash
./build.sh --target BuildAll
```

This checks compilation for every Revit year defined in the build configuration. Do not run the build for only a single Revit version. The official API assemblies come from the `Revit_All_Main_Versions_API_x64` package so the code builds without needing Revit installed.

## Testing
The Revit API relies on native code and normally requires a running Revit instance. Unit tests instead reference the `RevitApiStubs` project to simulate the API. Run the tests with the stubs enabled:

```bash
dotnet test RevitExtensions.sln -c Release -p:UseRevitApiStubs=true
```

The stubs only support the surface area covered by the tests and should not be used in production code.

When testing features that cache data to disk, the code uses a file system abstraction.
Assign an in-memory implementation so tests do not touch the real file system:

```csharp
BuiltInParameterCollector.FileSystem = new InMemoryFileSystem();
BuiltInParameterCollector.ClearCache();
```

## Organization
Extension methods live in the root `RevitExtensions` namespace.
Other classes should be placed in dedicated folders and namespaces:

- `Collectors` for classes that gather data (e.g. `BuiltInParameterCollector`)
- `Models` for simple data types like `ParameterIdentifier` and `ParameterMetadata`
- `Utilities` for helper classes such as comparers
- `IO` for file system abstractions like `IFileSystem`

## Coding style
Use `using var` with elements, parameters, and any other `IDisposable` objects when they are only used temporarily and not returned from the method.

To account for API differences across Revit versions, conditional compilation symbols may be used. Symbols follow the pattern `REVIT20xx`, `REVIT20xx_OR_ABOVE`, and `REVIT20xx_OR_LESS` and can be combined to include or exclude code for specific main releases.

All projects should enable C# nullable reference types by setting `<Nullable>enable</Nullable>` in the project file. Methods, properties and fields must be annotated with `?` when they can contain `null`.
Extension methods that wrap Revit API calls should reflect the API's nullability. If `Document.GetElement` might return `null`, your wrapper should return `Element?`.

## Documentation
All public methods must include XML summary comments describing what the method
does and detailing its parameters. Keep these comments short and informative so
they remain easy to read.
