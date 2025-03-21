namespace henryjs.Nuke.Components;

public interface IHasSolution : INukeBuild
{
    [Required]
    [Solution(SuppressBuildProjectCheck = true)]
    Solution Solution => TryGetValue(() => Solution);
    public IEnumerable<Project> TestProjects => Solution.AllProjects.Where(p => p.GetProperty("IsTestProject") != null);


}
