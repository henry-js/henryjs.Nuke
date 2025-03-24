using henryjs.Nuke.Extensions;

namespace henryjs.Nuke.Components;

public interface ITest : ICompile, IHasTest
{
    Target Test => _ => _
        .DependsOn(Compile)
        .TriggeredBy(Compile)
        .Executes(() =>
        {
            ExecuteTests(TestProjects);
        });

    public new void ExecuteTests(IEnumerable<Project> testProjects, Func<DotNetTestSettings, DotNetTestSettings>? customDotNetTestSettings = null)
    {
        var testCombinations =
        from project in testProjects
        select new { project, };
        DotNetTest(_ => _
            .EnableNoLogo()
            .EnableNoBuild()
            .SetConfiguration(Configuration)
            .CombineWith(testCombinations, (_, v) => _
                    .SetProjectFile(v.project)
            )
        );

    }
}

public interface IHasTest
{
    public void ExecuteTests(IEnumerable<Project> testProjects, Func<DotNetTestSettings, DotNetTestSettings>? customDotNetTestSettings = null)
    {
        foreach (var project in testProjects)
        {
            var isTest = project.GetProperty("IsTestProject");
            var configurations = project.GetReleases();
            foreach (var conf in configurations)
            {
                DotNetTest(_ => _
                    .SetProjectFile(project)
                    // .SetVerbosity(DotNetVerbosity.normal)
                    .SetCustomDotNetTestSettings(customDotNetTestSettings)
                    );
            }
        }
    }
}
