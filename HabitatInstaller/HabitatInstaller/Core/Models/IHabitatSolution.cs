namespace HabitatInstaller.Core.Models
{
    public interface IHabitatSolution : ISolution
    {
        string PublishUrl { get; set; }
    }
}
