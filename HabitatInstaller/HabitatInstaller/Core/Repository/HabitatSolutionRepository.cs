using HabitatInstaller.Core.Models;
using HabitatInstaller.UI.Windows;
using System.Windows;
using System;

namespace HabitatInstaller.Repository
{
    public class HabitatSolutionRepository : IHabitatSolutionRepository
    {
        public IHabitatSolution MapUserInput(IHabitatSolution solution)
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
