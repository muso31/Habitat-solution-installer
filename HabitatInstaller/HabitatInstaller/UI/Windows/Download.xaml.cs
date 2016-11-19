using HabitatInstaller.Core.Models;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Linq;
using HabitatInstaller.Core.Class;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        private string _zipName = "Habitat.zip";
        private ISolution _solution;

        public DownloadWindow(ISolution solution)
        {
            _solution = solution;
            InitializeComponent();

            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadComplete);
            wc.DownloadFileAsync(new Uri(solution.SolutionDownloadUrl), solution.TempDownloadDirectory + _zipName);
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownloadStatus.Value = e.ProgressPercentage;
        }

        private void wc_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OutputError(e.Error.Message);
            }
            else
            {
                ExtractFile();
            }
        }

        private void ExtractFile()
        {
            lblDownloading.Content = "Download complete";
            pbZipStatus.Visibility = Visibility.Visible;

            try
            {
                ZipFile.ExtractToDirectory(_solution.TempDownloadDirectory, _solution.SolutionInstallPath);

                //string pathToDevSettingsFile = _solution.SolutionInstallPath + @"src\Project\Habitat\code\App_Config\Include\Project\z.Habitat.DevSettings.config";
                ////update the z.Habitat.DevSettings.config file
                //File.WriteAllText(pathToDevSettingsFile, File.ReadAllText(pathToDevSettingsFile).Replace(@"C:\projects\Habitat\", _solution.SolutionInstallPath));
                ////update the gulp-config.js
                //File.WriteAllText(_solution.SolutionInstallPath, File.ReadAllText(_solution.SolutionInstallPath).Replace(@"C:\\inetpub\\wwwroot\\Habitat.dev.local", _solution.InstanceRoot));
                ////update the publishsettings.targets file
                //File.WriteAllText(_solution.SolutionInstallPath, File.ReadAllText(_solution.SolutionInstallPath).Replace("http://habitat.dev.local", _solution.PublishUrl));

                ////run node modules
                //ProcessStartInfo pInfo = new ProcessStartInfo();
                //pInfo.UseShellExecute = true;
                //pInfo.WorkingDirectory = _solution.SolutionInstallPath;
                //pInfo.FileName = "cmd.exe";
                //pInfo.Arguments = "/c npm install";

                //Process p = Process.Start(pInfo);
                //p.WaitForExit();

                //pInfo.Arguments = "/c gulp";
                //Process t = Process.Start(pInfo);
            }
            catch (Exception e)
            {
                OutputError(e.ToString());
            }
        }

        private void OutputError(string errorMessage) {
            this.Close();
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
