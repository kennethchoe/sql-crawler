#addin nuget:?package=Cake.Npm&version=0.17.0

var name = "sql-crawler";
var solution = "./src/" + name + ".sln";
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("PackageVersion", "1.0.0.0");

var buildDir = Directory(".\\src\\SqlCrawler.Web\\bin") + Directory(configuration);

Task("SetVersion")
    .Does(() =>
{
    var propsFile = "./src/Directory.build.props";
    Information(version);
    XmlPoke(propsFile, "//Version", version);
});
Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build-Vue")
    .Does(() =>
{
    var vuePath = ".\\src\\vue";
    NpmInstall(new NpmInstallSettings {WorkingDirectory = vuePath});
    NpmRunScript(new NpmRunScriptSettings {WorkingDirectory = vuePath, ScriptName = "build"});
});

Task("Build-DotNet")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("SetVersion")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      MSBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      XBuild(solution, settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Build")
    .IsDependentOn("Build-Vue")
    .IsDependentOn("Build-DotNet");

Task("Build-Docker")
    .IsDependentOn("Build-Vue")
    .Does(() =>
{
    var exitCode = StartProcess("docker", "build .\\src -t " + name);
    if (exitCode != 0)
        throw new Exception("Failed.");
});

Task("Run-DotNet-Tests")
    .IsDependentOn("Build-DotNet")
    .Does(() =>
{
    var exitCode = StartProcess("dotnet", "test " + solution + " -o output\\dotnet-tests");
    if (exitCode != 0)
        throw new Exception("Test failed.");
});

Task("Run-Tests")
    .IsDependentOn("Build-Vue")
    .IsDependentOn("Run-DotNet-Tests");

Task("Default")
    .IsDependentOn("Run-Tests");

Task("Publish")
    .IsDependentOn("Default")
    .Does(() =>
{
    CleanDirectory("./output");
    var publishPath = "./output/publish/";
    var settings = new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = publishPath
    };

    DotNetCorePublish(solution, settings);

    var packagePath = "./output/package/";
    CleanDirectory(packagePath);
    Zip(publishPath, packagePath + "latestPackage.zip", publishPath + "**/*");
    CopyFile(packagePath + "latestPackage.zip", packagePath + name + " " + version + ".zip");
});

RunTarget(target);