﻿using System.Windows;
using HabitatInstaller.Class;

namespace HabitatInstaller.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            habitatUrl.Text = Properties.Settings.Default.SolutionDownloadUrl;
            if (string.IsNullOrEmpty(Properties.Settings.Default.TempDirectory))
                Properties.Settings.Default.TempDirectory = System.IO.Path.GetTempPath();

            tempDirectory.Text = Properties.Settings.Default.TempDirectory;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMessageTempDir, errorMessageValidField;
            var isValidField = Validation.IsValidFieldInput(habitatUrl.Text, out errorMessageValidField);
            var isValidTempDir = Validation.DirectoryExists(tempDirectory.Text, out errorMessageTempDir);

            if (!isValidField)
            {
                MessageBox.Show(errorMessageValidField, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!isValidTempDir)
            {
                MessageBox.Show(errorMessageTempDir, "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Properties.Settings.Default.SolutionDownloadUrl = habitatUrl.Text.ToString();
                Properties.Settings.Default.TempDirectory = tempDirectory.Text.ToString();
                Properties.Settings.Default.Save();

                this.Close();
            }


        }
    }
}
