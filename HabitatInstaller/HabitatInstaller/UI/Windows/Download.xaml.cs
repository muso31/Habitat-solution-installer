using HabitatInstaller.Core.Models;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        private string _tempPath;
        private string _dlFilePath;
        private ISolution _solution;

        public DownloadWindow(ISolution solution)
        {
            _solution = solution;
            InitializeComponent();
            DownloadFile();
            ExtractFiles();
            RunNPM();

            //wait for npm install to finish
            this.Close();
            MessageBox.Show("Habitat solution installed to " + _solution.SolutionInstallPath, "Complete", MessageBoxButton.OK, MessageBoxImage.None);
        }

        private void DownloadFile()
        {
                _dlFilePath = _solution.TempDownloadDirectory + "Habitat.zip";

                WebClient wc = new WebClient();
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadComplete);
                wc.DownloadFileAsync(new Uri(_solution.SolutionDownloadUrl), _dlFilePath);
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
                pbDownloadStatus.Value = e.ProgressPercentage;
        }

        private void wc_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OutputErrorCloseApp(e.Error.Message);
            }
            else
            {
                //update UI
                lblDownloading.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblDownloading.Content = "Download complete";
                }));
                lblExtracting.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblExtracting.Content = "Extracting files...";
                }));
            }
        }

        private void ExtractFiles()
        {
            try
            {
                this.Title = "Step 2: Extracting";

                var dirName = new DirectoryInfo(_solution.SolutionInstallPath).Name;
                //TODO: CHECK TEMP PATH DOESNT EXIST
                _tempPath = _solution.SolutionInstallPath.Replace(dirName, dirName + @"_temp");

                ZipFile.ExtractToDirectory(_dlFilePath, _tempPath);

                Directory.Move(_tempPath + "Habitat-master", _solution.SolutionInstallPath);
                Directory.Delete(_tempPath, true);

                //update the z.Habitat.DevSettings.config file
                string pathToDevSettingsFile = _solution.SolutionInstallPath + @"src\Project\Habitat\code\App_Config\Include\Project\z.Habitat.DevSettings.config";
                File.WriteAllText(pathToDevSettingsFile, File.ReadAllText(pathToDevSettingsFile).Replace(@"C:\projects\Habitat\", _solution.SolutionInstallPath));
                File.WriteAllText(pathToDevSettingsFile, File.ReadAllText(pathToDevSettingsFile).Replace("dev.local", _solution.Hostname));
                //update the gulp-config.js
                var gulpFile = _solution.SolutionInstallPath + "gulp-config.js";
                File.WriteAllText(gulpFile, File.ReadAllText(gulpFile).Replace(@"C:\\websites\\Habitat.dev.local", _solution.InstanceRoot));
                //update the publishsettings.targets file
                var publishSettingsFile = _solution.SolutionInstallPath + "publishsettings.targets";
                File.WriteAllText(publishSettingsFile, File.ReadAllText(publishSettingsFile).Replace("http://habitat.dev.local", _solution.PublishUrl));

                lblExtracting.Content = "Extract complete";
            }
            catch (Exception e)
            {
                //clean up the folders
                if (Directory.Exists(_tempPath))
                {
                    Directory.Delete(_tempPath, true);
                }
                if (Directory.Exists(_solution.SolutionInstallPath.TrimEnd('\\')))
                {
                    Directory.Delete(_solution.SolutionInstallPath.TrimEnd('\\'), true);
                }

                OutputErrorCloseApp(e.ToString());
            }
        }

        private void RunNPM()
        {
            try
            {
                this.Title = "Step 3: Running npm install";
                //TODO: catch error if npm fails
                //run node modules
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.UseShellExecute = true;
                pInfo.WorkingDirectory = _solution.SolutionInstallPath; 
                pInfo.FileName = "cmd.exe";
                pInfo.Arguments = "/c npm install";

                var p = Process.Start(pInfo);
                p.WaitForExit();

                //pInfo.Arguments = "/c gulp";
                //Process t = Process.Start(pInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "NPM error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OutputErrorCloseApp(string errorMessage) {
            this.Close();
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
