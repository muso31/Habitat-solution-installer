using System.Windows;
using HabitatInstaller.Models;
using HabitatInstaller.Repository;

namespace HabitatInstaller.Windows
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
            string errorMessage;

            //create class FormValidator, returns bool
            //projectPath.Text.Validate("text to validatie", Enum.ValidationType, )
            //or add attributes to the solution model then pass the solution object to a validator class.

            if (!Class.Validation.IsValidFieldInputWithTrailingChar(projectPath.Text, @"\", out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Class.Validation.IsValidFieldInput(instanceRoot.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Class.Validation.IsValidFieldInput(publishUrl.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Class.Validation.IsValidFieldInput(hostname.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Properties.Settings.Default.ProjectPath = projectPath.Text;
                Properties.Settings.Default.WebsiteLocation = instanceRoot.Text;
                Properties.Settings.Default.WebsiteUrl = publishUrl.Text;
                Properties.Settings.Default.Hostname = hostname.Text;

                var repository = new SolutionRepository();

                var habitatInstance = repository.Create(new Solution());

                string confirmation = string.Format("Install Habitat to: {0}?", habitatInstance.SolutionInstallPath);

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
