using henryjs.Nuke.Components;
using Nuke.Common;
using Nuke.Common.Tooling;

public class Build : NukeBuild, IClean, ICompile, ITest, IPack, INugetPush, IHasMainProject
{
    [NuGetPackage(
    packageId: "vpk",
    packageExecutable: "vpk.dll",
    Version = "0.0.1053"
)]
    readonly Tool Vpk;
    public static int Main() => Execute<Build>(x => (x as ICompile).Compile);

    string IHasMainProject.ProjectName => "henryjs.Nuke";
}
