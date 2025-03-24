namespace henryjs.Nuke.Components;

public interface IPublish : IHasPublish, ICompile
{
    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var result = DotNetPublish(_ => _
                .EnableNoLogo()
                .EnableNoRestore()
                .SetProject(MainProject)
                .SetConfiguration(Configuration)
                .SetOutput(PublishDirectory)
                .SetVerbosity(DotNetVerbosity.minimal)
                .EnableProcessOutputLogging()
                );
            Log.Information("Publish output captured: {captured}", result is not null);

        });
}
public interface IHasPublish : IHasMainProject
{
    [Parameter]
    string PublishFolderName => TryGetValue(() => PublishFolderName) ?? "publish";
    AbsolutePath PublishDirectory => Solution.Directory / PublishFolderName;
}
public static class PublishExtensions
{
    public static string? GetPublishedVersion(this Project project, AbsolutePath searchDirectory)
    {
        FileVersionInfo? fileVersionInfo = default;
        Version version = new();
        var files = Directory.GetFiles(searchDirectory, $"{project.Name}*.dll", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                var fileVersionInfoTest = FileVersionInfo.GetVersionInfo(file);
                var fileVersion = new Version(fileVersionInfoTest?.FileVersion ?? "");
                if (version < fileVersion)
                {
                    version = fileVersion;
                    fileVersionInfo = fileVersionInfoTest;
                }
            }
            catch { }
        }
        return fileVersionInfo?.ProductVersion;
    }
}