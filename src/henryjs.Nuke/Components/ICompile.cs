namespace henryjs.Nuke.Components;

public interface ICompile : IClean, IHasMainProject
{
    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution)
            );
            var projects = new Project[] { MainProject };
            var compileCombinations =
            from project in projects
            from framework in Frameworks
            from runtime in Runtimes
            select new { project, framework, runtime };
            Log.Information("Configuration: {Configuration}", Configuration);
            Log.Information("=== Projects ===");
            foreach (var project in projects)
            {
                Log.Information(project);
            }
            Log.Information("=== RIDs ===");
            foreach (var rid in Runtimes)
            {
                Log.Information("{RID}", rid);
            }
            Log.Information("=== Combinatorials ===");
            foreach (var combinatorial in compileCombinations)
            {
                Log.Information("Project: {project}, Framework: {framework}, Runtime: {runtime}", combinatorial.project, combinatorial.framework, combinatorial.runtime);
            }

            DotNetBuild(_ => _
                .EnableNoLogo()
                .SetConfiguration(Configuration)
                .CombineWith(compileCombinations, (_, v) => _
                    .SetProjectFile(v.project)
                    .SetFramework(v.framework)
                    .SetRuntime(v.runtime)
                )
            );
            foreach (var testProject in TestProjects)
            {
                DotNetBuild(_ => _
                    .EnableNoLogo()
                    .EnableNoRestore()
                    .SetProjectFile(testProject)
                    .SetConfiguration(Configuration));
            }
        });
}
