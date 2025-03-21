using System.Collections;
using henryjs.Nuke.Extensions;
using Nuke.Common;
using Nuke.Common.ProjectModel;

namespace henryjs.Nuke.Components;

public interface IHasMainProject : IHasSolution
{
    /// <summary>
    /// Name of the MainProject (default: <seealso cref="Solution.Name"/>)
    /// </summary>
    [Parameter]
    string ProjectName => TryGetValue(() => ProjectName);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration? Configuration => TryGetValue(() => Configuration) ?? (IsLocalBuild ? Configuration.Debug : Configuration.Release);

    [Parameter("Runtimes you want to compile & test against - Default is '<empty>'")]
    string[] Runtimes => TryGetValue(() => Runtimes) ?? [""];

    [Parameter("Target framework - Default is 'net9.0'")]
    string[] Frameworks => TryGetValue(() => Frameworks) ?? ["net9.0"];

    /// <summary>
    /// MainProject (default: <seealso cref="ProjectName"/>)
    /// </summary>
    public Project MainProject => Solution.GetOtherProject(ProjectName)
        ?? throw new Exception($"{nameof(MainProject)} is null using '{ProjectName}', use 'string {nameof(IHasMainProject)}.{nameof(ProjectName)} => \"YourMainProjectName\"'.");

    // public IEnumerable<Project> BuildProjects => ProjectNames?.Count() > 0
    // ? ProjectNames.Select(n => Solution.GetOtherProject(n))
    // : Solution.GetAllProjects("*");

    /// <summary>
    /// MainProject (default: <seealso cref="ProjectName"/>)
    /// </summary>
    /// <returns></returns>
    public Project GetMainProject() => MainProject;
}
