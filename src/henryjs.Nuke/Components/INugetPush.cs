namespace henryjs.Nuke.Components;

public interface INugetPush : IPack, IHasNugetPush
{
    Target NugetPush => _ => _
        .DependsOn(Pack)
        .TriggeredBy(Pack)
        .OnlyWhenStatic(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
        .OnlyWhenStatic(() => !string.IsNullOrWhiteSpace(NuGetApiUrl))
        .Executes(() =>
        {
            foreach (var package in NupkgDirectory.GetFiles("*.nupkg"))
            {
                DotNetNuGetPush(_ => _
                    .SetApiKey(NuGetApiKey)
                    .SetSource(NuGetApiUrl)
                    .SetTargetPath(package)
                );
            }
        });
}

public interface IHasNugetPush : INukeBuild
{
    /// <summary>
    /// NuGetApiUrl
    /// </summary>
    [Secret][Parameter] public string NuGetApiUrl => TryGetValue(() => NuGetApiUrl) ?? "https://api.nuget.org/v3/index.json";
    /// <summary>
    /// NuGetApiKey
    /// </summary>
    [Secret][Parameter] public string NuGetApiKey => TryGetValue(() => NuGetApiKey);

}
