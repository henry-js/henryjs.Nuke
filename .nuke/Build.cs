using henryjs.Nuke.Components;
using Nuke.Common;

class Build : NukeBuild, IClean, ICompile, ITest, IPublish, IAssetRelease, IHasMainProject
{
    public static int Main() => Execute<Build>(x => (x as ICompile).Compile);

    string IHasMainProject.ProjectName => "henryjs.Nuke";
}
