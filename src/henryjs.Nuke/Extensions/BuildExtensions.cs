using System.Diagnostics;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Serilog;

namespace henryjs.Nuke.Extensions;

public static class BuildExtensions
{
    private const string CONFIGURATION_DEBUG = "Debug";

    /// <summary>
    /// Deletes all the bin/obj folders except for the ones in the specified build project directory.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="buildProjectDirectory">The build project directory.</param>
    internal static void CleanSolution(this Solution solution, AbsolutePath buildProjectDirectory)
    {
        var dirs = Globbing.GlobDirectories(solution.Directory, "**/bin", "**/obj")
            .Where(x => !PathConstruction.IsDescendantPath(buildProjectDirectory, x));
        Log.Information("Cleaning {count} directories", dirs.Count());
        foreach (var dir in dirs)
        {
            Log.Information("Directory: {directory}", dir);
        }
        dirs.DeleteFiles();
        solution.GetAllProjects("*");
    }

    /// <summary>
    /// Get MainProject
    /// </summary>
    /// <param name="hasMainProject"></param>
    /// <returns></returns>
    internal static Project GetOtherProject(this Solution solution, string projectName)
        => solution.GetAllProjects("*")
            .FirstOrDefault(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets the release configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns>The release configurations.</returns>
    public static IEnumerable<string> GetReleases(this Project project)
    {
        return project.GetConfigurations(CONFIGURATION_DEBUG, true);
    }

    /// <summary>
    /// Gets the configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <param name="contain">The string to contain in the configuration.</param>
    /// <param name="notContain">A flag indicating whether the configuration should not contain the specified string.</param>
    /// <returns>The configurations.</returns>
    public static IEnumerable<string> GetConfigurations(this Project project, string contain, bool notContain = false)
    {
        var configurations = project.GetConfigurations()
            .Where(s => s.Contains(contain, StringComparison.OrdinalIgnoreCase) != notContain);
        return configurations;
    }

    /// <summary>
    /// Gets the configurations for the specified project.
    /// </summary>
    /// <param name="project">The project.</param>
    /// <returns>The configurations.</returns>
    public static IEnumerable<string> GetConfigurations(this Project project)
    {
        var configurations = project.Configurations
                .Select(pair => pair.Value.Split("|").First())
                .Distinct();
        return configurations;
    }

    /// <summary>
    /// GetInformationalVersion => ProductVersion
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    /// <remarks>Works with .dll and .nupkg</remarks>
    public static string GetInformationalVersion(this Project project)
    {
        if (project.GetFileVersionInfo() is FileVersionInfo fileVersionInfo)
        {
            return fileVersionInfo?.ProductVersion.Split('+').First();
        }
        if (project.GetNuGetVersionInfo() is NuGetVersionInfo nugetVersionInfo)
        {
            return nugetVersionInfo?.ProductVersion.Split('+').First();
        }
        return null;
    }

    /// <summary>
    /// Get FileVersionInfo of Greater Dlls or Exe
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static FileVersionInfo GetFileVersionInfo(this Project project)
    {
        return GetFileVersionInfoGreater(project.Directory, $"*{project.Name}*.dll") ??
            GetFileVersionInfoGreater(project.Directory, $"*{project.Name}*.exe");
    }

    /// <summary>
    /// GetFileVersionInfoGreater
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="searchPattern"></param>
    /// <returns></returns>
    private static FileVersionInfo GetFileVersionInfoGreater(string sourceDir, string searchPattern = "*.dll")
    {
        FileVersionInfo fileVersionInfo = null;
        Version version = new Version();
        var files = Directory.GetFiles(sourceDir, searchPattern, SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                var fileVersionInfoTest = FileVersionInfo.GetVersionInfo(file);
                var fileVersion = new Version(fileVersionInfoTest.FileVersion);
                if (version < fileVersion)
                {
                    version = fileVersion;
                    fileVersionInfo = fileVersionInfoTest;
                }
            }
            catch { }
        }
        return fileVersionInfo;
    }

    /// <summary>
    /// Get NuGetVersionInfo of the first nupkg file found
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static NuGetVersionInfo GetNuGetVersionInfo(this Project project)
    {
        var sourceDir = project.Directory;
        var searchPattern = $"*{project.Name}*.nupkg";
        var nupkgFiles = Directory.GetFiles(sourceDir, searchPattern, SearchOption.AllDirectories);
        foreach (var nupkgFile in nupkgFiles)
        {
            var nugetVersionInfo = NuGetVersionInfo.Parse(nupkgFile);
            if (nugetVersionInfo is NuGetVersionInfo) return nugetVersionInfo;
        }

        return null;
    }

}
