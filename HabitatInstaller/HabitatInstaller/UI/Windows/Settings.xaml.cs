using System.Windows;
using HabitatInstaller.Core.Class;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            versionLabel.Content = $"Version: {Properties.Settings.Default.Version}";

            solutionUrl.Text = Properties.Settings.Default.SolutionDownloadUrl;
            if (string.IsNullOrEmpty(Properties.Settings.Default.TempDirectory))
                Properties.Settings.Default.TempDirectory = System.IO.Path.GetTempPath();

            tempDirectory.Text = Properties.Settings.Default.TempDirectory;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage;

            if (!Validation.IsValidFieldInput(solutionUrl.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!Validation.DirectoryExists(tempDirectory.Text, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Properties.Settings.Default.SolutionDownloadUrl = solutionUrl.Text.ToString();
                Properties.Settings.Default.TempDirectory = tempDirectory.Text.ToString();
                Properties.Settings.Default.Save();

                this.Close();
            }


        }
    }
}
