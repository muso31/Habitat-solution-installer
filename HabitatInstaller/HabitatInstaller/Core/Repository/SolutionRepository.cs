using HabitatInstaller.Core.Models;

namespace HabitatInstaller.Repository
{
    public class SolutionRepository : ISolutionRepository
    {

        public ISolution Create(ISolution solution)
        {
            solution.SolutionDownloadUrl = Properties.Settings.Default.SolutionDownloadUrl;
            solution.TempDownloadDirectory = Properties.Settings.Default.TempDirectory;
            solution.SolutionInstallPath = Properties.Settings.Default.SolutionInstallPath;
            solution.InstanceRoot = Properties.Settings.Default.InstanceRoot;
            solution.PublishUrl = Properties.Settings.Default.PublishUrl;
            solution.Hostname = Properties.Settings.Default.Hostname;

            return solution;
        }
    }
}
