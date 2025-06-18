using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.IO;

class Build : NukeBuild
{
    [Parameter("Package version for NuGet packages", Name = "version")]
    readonly string PackageVersion = "0.0.1";

    [Parameter("GitHub package feed url", Name = "ghfeed")] readonly string GitHubFeed;
    [Parameter("GitHub token for publishing", Name = "ghtoken")] readonly string GitHubToken;
    [Parameter("NuGet API key for publishing", Name = "nugetkey")] readonly string NuGetApiKey;

    AbsolutePath Project => RootDirectory / "RevitExtensions" / "RevitExtensions.csproj";
    AbsolutePath Output => RootDirectory / "nupkgs";

    readonly (int Year, string ApiVersion)[] RevitVersions = new[]
    {
        (2019, "2019.0.1"),
        (2020, "2020.0.1"),
        (2021, "2021.1.9"),
        (2022, "2022.1.0"),
        (2023, "2023.0.0"),
        (2024, "2024.2.0"),
        (2025, "2025.0.0"),
        (2026, "2026.0.0")
    };

    static string GetFramework(int year) => year >= 2025 ? "net8.0" : "net48";

    static string BuildDefines(int year)
    {
        var defs = new List<string> { $"REVIT{year}" };
        for (var y = 2019; y <= 2026; y++)
        {
            if (year >= y) defs.Add($"REVIT{y}_OR_ABOVE");
            if (year <= y) defs.Add($"REVIT{y}_OR_LESS");
        }
        return string.Join(";", defs);
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            if (Directory.Exists(Output))
                Directory.Delete(Output, true);
            Directory.CreateDirectory(Output);
        });

    Target BuildAll => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            foreach (var (year, api) in RevitVersions)
            {
                var tf = GetFramework(year);
                var defines = BuildDefines(year);
                DotNetRestore(s => s
                    .SetProjectFile(Project)
                    .SetProperty("TargetFramework", tf)
                    .SetProperty("TargetFrameworks", tf)
                    .SetProperty("RevitApiPackageVersion", api)
                    .SetProperty("UseRevitApiStubs", "false"));

                DotNetBuild(s => s
                    .SetProjectFile(Project)
                    .SetConfiguration(Configuration.Release)
                    .SetFramework(tf)
                    .EnableNoRestore()
                    .SetProperty("TargetFrameworks", tf)
                    .SetProperty("DefineConstants", defines)
                    .SetProperty("RevitApiPackageVersion", api)
                    .SetProperty("UseRevitApiStubs", "false")
                    .SetProperty("RevitYear", year.ToString())
                    .SetProperty("AssemblyVersion", GetAssemblyVersion()));
            }
        });

    Target Pack => _ => _
        .DependsOn(BuildAll)
        .Executes(() =>
        {
            foreach (var (year, api) in RevitVersions)
            {
                var tf = GetFramework(year);
                var defines = BuildDefines(year);
                DotNetPack(s => s
                    .SetProject(Project)
                    .SetConfiguration(Configuration.Release)
                    .SetOutputDirectory(Output)
                    .EnableNoBuild()
                    .SetProperty("PackageVersion", PackageVersion)
                    .SetProperty("PackageId", $"RevitExtensions.{year}")
                    .SetProperty("TargetFramework", tf)
                    .SetProperty("TargetFrameworks", tf)
                    .SetProperty("DefineConstants", defines)
                    .SetProperty("RevitApiPackageVersion", api)
                    .SetProperty("UseRevitApiStubs", "false")
                    .SetProperty("RevitYear", year.ToString())
                    .SetProperty("AssemblyVersion", GetAssemblyVersion()));
            }
        });

    Target Test => _ => _
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(RootDirectory / "RevitExtensions.sln")
                .SetConfiguration(Configuration.Release)
                .SetProperty("UseRevitApiStubs", "true"));
        });

    Target Publish => _ => _
        .DependsOn(Pack, Test)
        .Executes(() =>
        {
            foreach (var package in Directory.GetFiles(Output, "*.nupkg"))
            {
                if (!string.IsNullOrEmpty(GitHubFeed) && !string.IsNullOrEmpty(GitHubToken))
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource(GitHubFeed)
                        .SetApiKey(GitHubToken)
                        .EnableSkipDuplicate());
                }
                if (!string.IsNullOrEmpty(NuGetApiKey))
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(package)
                        .SetSource("https://api.nuget.org/v3/index.json")
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate());
                }
            }
        });

    string GetAssemblyVersion()
    {
        var core = PackageVersion.Split('-','+')[0];
        var parts = core.Split('.').Select(int.Parse).ToList();
        while (parts.Count < 4) parts.Add(0);
        return string.Join('.', parts.Take(4));
    }

    public static int Main () => Execute<Build>(x => x.Publish);
}
