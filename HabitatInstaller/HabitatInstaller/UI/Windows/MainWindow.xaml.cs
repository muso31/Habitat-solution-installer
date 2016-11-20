using System.Windows;
using HabitatInstaller.Core.Models;
using HabitatInstaller.Repository;
using HabitatInstaller.Core.Class;
using System.Diagnostics;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISolutionRepository _solutionRepository;
        public ISolution _solution;

        public MainWindow(ISolutionRepository solutionRepository, ISolution solution)
        {
            _solutionRepository = solutionRepository;
            _solution = solution;
        }

        public MainWindow() : this(new SolutionRepository(), new Solution())
        {
            InitializeComponent();

            //TODO: SETUP MODEL BINDING
            solutionInstallPath.Text = Properties.Settings.Default.SolutionInstallPath;
            instanceRoot.Text = Properties.Settings.Default.InstanceRoot;
            publishUrl.Text = Properties.Settings.Default.PublishUrl;
            hostname.Text = Properties.Settings.Default.Hostname;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Setup annotation validation
            string errorMessage;

            if (!Validation.IsValidFieldInputWithTrailingChar(solutionInstallPath.Text, @"\", out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Validation.IsValidFieldInput(instanceRoot.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Validation.IsValidFieldInput(publishUrl.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Validation.IsValidFieldInput(hostname.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                //TODO: SETUP MODEL BINDING
                Properties.Settings.Default.SolutionInstallPath = solutionInstallPath.Text;
                Properties.Settings.Default.InstanceRoot = instanceRoot.Text;
                Properties.Settings.Default.PublishUrl = publishUrl.Text;
                Properties.Settings.Default.Hostname = hostname.Text;

                //TODO: SETUP DI
                var habitatsolution = _solutionRepository.Create(_solution);
                var confirmation = string.Format("Install Habitat to: {0}?", habitatsolution.SolutionInstallPath);
                var result = MessageBox.Show(confirmation, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result.Equals(MessageBoxResult.Yes))
                {
                    var downloadWindow = new DownloadWindow(habitatsolution);
                    downloadWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    downloadWindow.Owner = Application.Current.MainWindow;
                    downloadWindow.ShowDialog();
                }
            }
        }

    }
}
