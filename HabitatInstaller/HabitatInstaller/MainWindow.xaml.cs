using System.Windows;
using HabitatInstaller.Class;

namespace HabitatInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            projectPath.Text = Properties.Settings.Default.ProjectPath;
            instanceRoot.Text = Properties.Settings.Default.WebsiteLocation;
            publishUrl.Text = Properties.Settings.Default.WebsiteUrl;
            hostname.Text = Properties.Settings.Default.Hostname;
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.ShowDialog();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessageProject, errorMessageWebsitePath, errorMessageWebsiteUrl, errorMessageHostname;
            var isValidSourceDir = Class.Validation.IsValidFieldInputWithTrailingChar(projectPath.Text, @"\", out errorMessageProject);
            var isValidWebsiteDir = Class.Validation.IsValidFieldInput(instanceRoot.Text, out errorMessageWebsitePath);
            var isValidWebsiteUrl = Class.Validation.IsValidFieldInput(publishUrl.Text, out errorMessageWebsiteUrl);
            var isValidHostname = Class.Validation.IsValidFieldInput(hostname.Text, out errorMessageHostname);

            if (!isValidSourceDir)
            {
                MessageBox.Show(errorMessageProject, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!isValidWebsiteDir)
            {
                MessageBox.Show(errorMessageWebsitePath, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!isValidWebsiteUrl)
            {
                MessageBox.Show(errorMessageWebsiteUrl, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!isValidHostname)
            {
                MessageBox.Show(errorMessageHostname, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                var HabitatInstance = new HabitatInstance();

                HabitatInstance.HabitatDownloadUrl = Properties.Settings.Default.HabitatUrl;
                HabitatInstance.TempDownloadDirectory = Properties.Settings.Default.TempDirectory;
                HabitatInstance.SolutionInstallPath = projectPath.Text;
                HabitatInstance.InstanceRoot = instanceRoot.Text;
                HabitatInstance.PublishUrl = publishUrl.Text;
                HabitatInstance.Hostname = hostname.Text;

                string confirmation = string.Format("Install Habitat to: {0}?", HabitatInstance.SolutionInstallPath);

                var result = MessageBox.Show(confirmation, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result.Equals(MessageBoxResult.Yes))
                {
                    var installWindow = new Install();
                    installWindow.ShowDialog();
                }
            }
        }

    }
}
