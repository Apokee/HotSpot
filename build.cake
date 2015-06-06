#l "utilities.cake"

using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

public sealed class BuildConfiguration
{
    [YamlAlias("ksp_dir")]
    public string KspDir { get; set; }

    [YamlAlias("ksp_bin")]
    public string KspBin { get; set; }

    public string KspPath(params string[] paths)
    {
        return KspDir == null ? null : System.IO.Path.Combine(KspDir, System.IO.Path.Combine(paths));
    }
}

var target = Argument<string>("target", "Package");
var configuration = Argument<string>("configuration", "Debug");
var release = Argument<bool>("release", false);

var buildConfiguration = GetBuildConfiguration<BuildConfiguration>();

var solution = GetSolution();

var identifier = "EnhancedThermalData";
var outputDirectory = "Output";
var buildDirectory = Directory($"{outputDirectory}/Build/{configuration}");
var binDirectory = Directory($"{buildDirectory}/Common/bin");
var stageDirectory = Directory($"{outputDirectory}/Stage/{configuration}");
var stageGameDataDirectory = Directory($"{stageDirectory}/GameData");
var stageModDirectory = Directory($"{stageGameDataDirectory}/{identifier}");
var deployModDirectory = buildConfiguration.KspPath("GameData", identifier);
var packageDirectory = Directory($"{outputDirectory}/Package/{configuration}");

Task("Init")
    .Does(() =>
{
    var kspLibDirectory = System.IO.Path.Combine("Library", "KSP");
    var kspLibs = new [] { "Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll", "UnityEngine.dll" };

    CreateDirectory(kspLibDirectory);

    foreach (var kspLib in kspLibs)
    {
        if (!FileExists(System.IO.Path.Combine(kspLibDirectory, kspLib)))
        {
            CopyFileToDirectory(
                buildConfiguration.KspPath("KSP_Data", "Managed", kspLib),
                kspLibDirectory
            );
        }
    }
});

Task("CleanBuild")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildDirectory });
});

Task("CleanStage")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { stageDirectory });
});

Task("CleanPackage")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { packageDirectory });
});

Task("CleanDeploy")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { deployModDirectory });
});

Task("Restore")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("BuildBuildVersion")
    .Does(() =>
{
    string buildVersion;

    var version = GetVersion();
    var rev = GetGitRevision(useShort: true);

    if (rev != null && !release)
    {
        if (version.Build == null)
        {
            buildVersion = new SemVer(version.Major, version.Minor, version.Patch, version.Pre, rev);
        }
        else
        {
            throw new Exception("VERSION already contains build metadata");
        }
    }
    else
    {
        buildVersion = version;
    }

    System.IO.File.WriteAllText("Output/VERSION", buildVersion);
});

Task("BuildAssemblyInfo")
    .Does(() =>
{
    BuildAssemblyInfo($"Source/{identifier}/Properties/AssemblyInfo.cs");
    BuildAssemblyInfo($"Tests/{identifier}Tests/Properties/AssemblyInfo.cs");
});

Task("Build")
    .IsDependentOn("CleanBuild")
    .IsDependentOn("Init")
    .IsDependentOn("Restore")
    .IsDependentOn("BuildBuildVersion")
    .IsDependentOn("BuildAssemblyInfo")
    .Does(() =>
{
    MSBuild(solution, s => { s.Configuration = configuration; });
});

Task("Stage")
    .IsDependentOn("CleanStage")
    .IsDependentOn("Build")
    .Does(() =>
{
    var pluginsDirectory = System.IO.Path.Combine(stageModDirectory, "Plugins");

    CreateDirectory(stageGameDataDirectory);
    CreateDirectory(stageModDirectory);
    CreateDirectory(pluginsDirectory);

    CopyFiles($"{binDirectory}/*", pluginsDirectory);
    CopyDirectory("Configuration", $"{stageModDirectory}/Configuration");
    CopyDirectory("Patches", $"{stageModDirectory}/Patches");
    CopyFileToDirectory("CHANGES.md", stageModDirectory);
    CopyFileToDirectory("LICENSE.md", stageModDirectory);
    CopyFileToDirectory("README.md", stageModDirectory);
});

Task("Deploy")
    .IsDependentOn("Stage")
    .IsDependentOn("CleanDeploy")
    .Does(() =>
{
    CopyDirectory(stageModDirectory, $"{buildConfiguration.KspPath("GameData")}/{identifier}");
});

Task("Run")
    .IsDependentOn("Deploy")
    .Does(() =>
{
    StartProcess(System.IO.Path.Combine(buildConfiguration.KspDir, buildConfiguration.KspBin), new ProcessSettings
        {
            WorkingDirectory = buildConfiguration.KspDir
        });
});

Task("Package")
    .IsDependentOn("CleanPackage")
    .IsDependentOn("Stage")
    .Does(() =>
{
    CreateDirectory(packageDirectory);

    Zip(stageDirectory, File($"{packageDirectory}/{identifier}-{GetBuildVersion()}.zip"));
});

RunTarget(target);

private void BuildAssemblyInfo(string file)
{
    var version = GetBuildVersion();

    var output = TransformTextFile($"{file}.in")
        .WithToken("VERSION", version)
        .WithToken("VERSION.MAJOR", version.Major)
        .WithToken("VERSION.MINOR", version.Minor)
        .WithToken("VERSION.PATCH", version.Patch)
        .WithToken("VERSION.PRE", version.Pre)
        .WithToken("VERSION.BUILD", version.Build)
        .ToString();

    System.IO.File.WriteAllText(file, output);
}

private SemVer GetBuildVersion()
{
    return new SemVer(System.IO.File.ReadAllText("Output/VERSION"));
}
