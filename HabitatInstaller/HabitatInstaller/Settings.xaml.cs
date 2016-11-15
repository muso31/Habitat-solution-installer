using System.Windows;

namespace HabitatInstaller
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            habitatUrl.Text = Properties.Settings.Default.HabitatUrl;
            if (string.IsNullOrEmpty(Properties.Settings.Default.TempDirectory))
                Properties.Settings.Default.TempDirectory = System.IO.Path.GetTempPath();

            tempDirectory.Text = Properties.Settings.Default.TempDirectory;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tempDirectory.Text.EndsWith(@"\"))
            {
                Properties.Settings.Default.HabitatUrl = habitatUrl.Text.ToString();
                Properties.Settings.Default.TempDirectory = tempDirectory.Text.ToString();
                Properties.Settings.Default.Save();

                this.Close();
            }
            else {
                //error message here
            }

        }
    }
}
