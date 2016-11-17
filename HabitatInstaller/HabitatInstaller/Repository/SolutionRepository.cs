using HabitatInstaller.Models;

namespace HabitatInstaller.Repository
{
    public class SolutionRepository : ISolutionRepository
    {

        public ISolution Create(ISolution solution)
        {
            solution.SolutionDownloadUrl = Properties.Settings.Default.SolutionDownloadUrl;
            solution.TempDownloadDirectory = Properties.Settings.Default.TempDirectory;
            solution.SolutionInstallPath = Properties.Settings.Default.ProjectPath;
            solution.InstanceRoot = Properties.Settings.Default.WebsiteLocation;
            solution.PublishUrl = Properties.Settings.Default.WebsiteUrl;
            solution.Hostname = Properties.Settings.Default.Hostname;

            return solution;
        }
    }
}
