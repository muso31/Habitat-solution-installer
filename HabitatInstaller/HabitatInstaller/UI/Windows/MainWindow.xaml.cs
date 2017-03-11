using System.Windows;
using HabitatInstaller.Core.Models;
using HabitatInstaller.Repository;
using HabitatInstaller.Core.Class;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IHabitatSolutionRepository _solutionRepository;
        private IHabitatSolution _solution;

        public MainWindow(IHabitatSolutionRepository solutionRepository, IHabitatSolution solution)
        {
            _solutionRepository = solutionRepository;
            _solution = solution;
        }

        public MainWindow() : this(new HabitatSolutionRepository(), new HabitatSolution())
        {
            InitializeComponent();
            // First time run assign temp path setting
            if (string.IsNullOrEmpty(Properties.Settings.Default.TempDirectory))
                Properties.Settings.Default.TempDirectory = System.IO.Path.GetTempPath();

            //TODO: SETUP MODEL BINDING
            solutionInstallPath.Text = Properties.Settings.Default.SolutionInstallPathDefault;
            instanceRoot.Text = Properties.Settings.Default.InstanceRootDefault;
            publishUrl.Text = Properties.Settings.Default.PublishUrlDefault;
            hostname.Text = Properties.Settings.Default.HostnameDefault;
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
                //TODO: SETUP DI
                _solution = _solutionRepository.MapUserInput(_solution);

                var confirmation = $"Install Habitat to: {_solution.SolutionInstallPath}?";
                var result = MessageBox.Show(confirmation, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result.Equals(MessageBoxResult.Yes))
                {
                    var downloadWindow = new DownloadWindow(_solution);
                    downloadWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    downloadWindow.Owner = Application.Current.MainWindow;
                    downloadWindow.ShowDialog();
                }
            }
        }

        public string SolutionInstallPathText
        {
            get { return solutionInstallPath.Text; }

            set
            {
                solutionInstallPath.Text = value;
            }
        }

        public string InstanceRootText
        {
            get { return instanceRoot.Text; }

            set
            {
                instanceRoot.Text = value;
            }
        }

        public string PublishUrlText
        {
            get { return publishUrl.Text; }

            set
            {
                publishUrl.Text = value;
            }
        }

        public string HostnameText
        {
            get { return hostname.Text; }

            set
            {
                hostname.Text = value;
            }
        }


    }
}
