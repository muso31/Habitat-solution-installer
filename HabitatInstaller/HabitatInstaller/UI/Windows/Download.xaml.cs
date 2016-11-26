using HabitatInstaller.Core.Models;
using System;
using System.Net;
using System.Windows;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

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
        private bool _errors = false;

        public DownloadWindow(ISolution solution)
        {
            _solution = solution;
            InitializeComponent();
            Start();

            //Task taskA = Task.Run(() => DownloadFile());

            //var taskDL = ExtractFiles();
            //taskDL.ContinueWith(t =>
            //{
            //    OutputErrorCloseWindow(t.Exception.ToString());
            //}, TaskContinuationOptions.OnlyOnFaulted);
        }
        private async void Start()
        {
            await DownloadFile();

            // lblDownloading.Content = "Download complete";

            //// this.Dispatcher.Invoke(() =>
            // //{
            //     lblExtracting.Content = "Extracting files...";
            //     pbExtractStatus.Visibility = Visibility.Visible;
            // //});

            await ExtractFiles();

            //if (!_errors)
            //{
            //    RunNPM();
            //    this.Close();
            //    MessageBox.Show("Habitat solution installed to " + _solution.SolutionInstallPath, "Complete", MessageBoxButton.OK, MessageBoxImage.None);
            //}
        }

        private async Task DownloadFile()
        {
            _dlFilePath = _solution.TempDownloadDirectory + "Habitat.zip";

            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadComplete);
            await wc.DownloadFileTaskAsync(new Uri(_solution.SolutionDownloadUrl), _dlFilePath);
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                pbDownloadStatus.Value = e.ProgressPercentage;
            });
        }

        private void wc_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OutputErrorCloseWindow(e.Error.Message);
            }
        }

        private async Task ExtractFiles()
        {
            try
            {
               var updateUI = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lblDownloading.Content = "Download complete";
                        lblExtracting.Content = "Extracting files...";
                        pbExtractStatus.Visibility = Visibility.Visible;
                        this.Title = "Step 2: Extracting";
                    });
                });

                await updateUI;

                var extractFiles = Task.Factory.StartNew(() =>
                {
                    var dirName = new DirectoryInfo(_solution.SolutionInstallPath).Name;
                    //TODO: CHECK TEMP PATH DOESNT EXIST
                    _tempPath = _solution.SolutionInstallPath.Replace(dirName, dirName + @"_temp");

                    ZipFile.ExtractToDirectory(_dlFilePath, _tempPath);
                });

                await extractFiles;

                var moveFiles = Task.Factory.StartNew(() =>
                {
                    // Directory.SetAccessControl(_tempPath + "Habitat-master");
                    Thread.Sleep(4000);
                    //ACCESS IS DENIED ERROR
                    Directory.Move(_tempPath + "Habitat-master", _solution.SolutionInstallPath);
                    Thread.Sleep(4000);
                    Directory.Delete(_tempPath, true);
                });

                await moveFiles;

                var updateFiles = Task.Factory.StartNew(() =>
                {
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
                    this.Dispatcher.Invoke(() =>
                    {
                        lblExtracting.Content = "Extract complete";
                    });
                });

                await updateFiles;
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

                OutputErrorCloseWindow(e.ToString());
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

        private void OutputErrorCloseWindow(string errorMessage)
        {
            _errors = true;
            this.Close();
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }
}
