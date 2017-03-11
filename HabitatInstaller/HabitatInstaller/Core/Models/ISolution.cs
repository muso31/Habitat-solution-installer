namespace HabitatInstaller.Core.Models
{
    public interface ISolution
    {
        string SolutionDownloadUrl { get; set; }
        string TempDownloadDirectory { get; set; }
        string SolutionInstallPath { get; set; }
        string InstanceRoot { get; set; }
        string Hostname { get; set; }
    }
}
