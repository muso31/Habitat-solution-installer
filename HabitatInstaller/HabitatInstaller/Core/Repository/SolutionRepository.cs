using HabitatInstaller.Core.Models;
using HabitatInstaller.UI.Windows;
using System.Windows;

namespace HabitatInstaller.Repository
{
    public class SolutionRepository : ISolutionRepository
    {
        public ISolution MapUserInput(ISolution solution)
        {
            var _form = Application.Current.Windows[0] as MainWindow;

            solution.SolutionInstallPath = _form.SolutionInstallPathText;
            solution.InstanceRoot = _form.InstanceRootText;
            solution.PublishUrl = _form.PublishUrlText;
            solution.Hostname = _form.HostnameText;
            //changeable saved fields
            solution.SolutionDownloadUrl = Properties.Settings.Default.SolutionDownloadUrl;
            solution.TempDownloadDirectory = Properties.Settings.Default.TempDirectory;

            return solution;
        }
    }
}
