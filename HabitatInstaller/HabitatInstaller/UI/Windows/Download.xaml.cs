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
        }
        private async Task Start()
        {
            Thread.Sleep(500);
            await DownloadFile();

            await ExtractFiles();

            if (!_errors)
            {
                await RunNPM();
            }
        }

        private async Task DownloadFile()
        {
            _dlFilePath = _solution.TempDownloadDirectory + "Habitat.zip";

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadComplete);
                await client.DownloadFileTaskAsync(new Uri(_solution.SolutionDownloadUrl), _dlFilePath);
            }
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
                OutputErrorCloseWindow(e.Error);
            }
        }

        private async Task ExtractFiles()
        {
               await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lblDownloading.Content = "Download complete";
                        extractLabel.IsBusy = true;
                        this.Title = "Step 2: Extracting";
                    });
                });

                await Task.Run(() =>
                {
                    try
                    {
                        var dirName = new DirectoryInfo(_solution.SolutionInstallPath).Name;
                        //TODO: CHECK TEMP PATH DOESNT EXIST
                        _tempPath = _solution.SolutionInstallPath.Replace(dirName, dirName + @"_temp");

                        ZipFile.ExtractToDirectory(_dlFilePath, _tempPath);
                    }
                    catch (Exception ex)
                    {
                        OutputErrorCloseWindow(ex);
                    }
                });

                await Task.Run(() =>
                {
                    try
                    {
                        Thread.Sleep(4000);
                        //TODO: resolve ACCESS IS DENIED ERROR
                        // Directory.SetAccessControl(_tempPath + "Habitat-master");
                        Directory.Move(_tempPath + "Habitat-master", _solution.SolutionInstallPath);
                        Thread.Sleep(2000);
                        Directory.Delete(_tempPath, true);

                        this.Dispatcher.Invoke(() =>
                        {
                            extractLabel.IsBusy = false;
                        });

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
                    }
                    catch (Exception ex)
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

                        OutputErrorCloseWindow(ex);
                    }
                });

        }

        private async Task RunNPM()
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
                p.EnableRaisingEvents = true;
                p.Exited += new EventHandler(ProcessExited);
                //p.WaitForExit();

                //pInfo.Arguments = "/c gulp";
                //Process t = Process.Start(pInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "NPM error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void ProcessExited(object sender, System.EventArgs e)
        {
            //Handle process exit here
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
            MessageBox.Show("Habitat solution installed to " + _solution.SolutionInstallPath, "Complete", MessageBoxButton.OK, MessageBoxImage.None);

        }

        private void OutputErrorCloseWindow(Exception ex)
        {
            _errors = true;
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
            MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }
}
