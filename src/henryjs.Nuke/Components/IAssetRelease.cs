using henryjs.Nuke.Extensions;

namespace henryjs.Nuke.Components;
public interface IAssetRelease : IHasAssetReleaser, IHasAssetRelease, IPublish
{
    Target AssetRelease => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            AssetReleaser.ReleaseAssets(MainProject, PublishDirectory, ReleaseDirectory, Solution.Name);
        });
}

public class VelopackAssetReleaser : IAssetReleaser
{
    public Tool Vpk { get; set; }
    public VelopackAssetReleaser(Tool Velopack)
    {
        Vpk = Velopack;
    }

    public void ReleaseAssets(Project project, AbsolutePath publishDirectory, AbsolutePath releaseDirectory, string appName)
    {
        var x = Vpk.Invoke($"pack --packId tasktitan --packVersion {project.GetPublishedVersion(publishDirectory)} --packDir {publishDirectory} --mainExe {project.Name}.exe --packTitle {appName} --outputDir {releaseDirectory} --shortcuts None");
    }
}