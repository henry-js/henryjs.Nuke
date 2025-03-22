namespace henryjs.Nuke.Components;

public interface IAssetReleaser : IHasAssetReleaser
{
    void ReleaseAssets(Project project, AbsolutePath publishDirectory, AbsolutePath releaseDirectory, string appName);
}
