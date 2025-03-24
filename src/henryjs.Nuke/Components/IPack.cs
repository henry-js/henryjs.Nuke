namespace henryjs.Nuke.Components;

public interface IPack : ITest, IHasPack
{
    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .EnableNoBuild()
                .SetProject(MainProject)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(NupkgDirectory)
                .EnableProcessOutputLogging()
            );
        });
}

public interface IHasPack : IHasMainProject
{
    [Parameter]
    public string NupkgFolderName => TryGetValue(() => NupkgFolderName) ?? "nupkg";
    AbsolutePath NupkgDirectory => Solution.Directory / NupkgFolderName;
}
