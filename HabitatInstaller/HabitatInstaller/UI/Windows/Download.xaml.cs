using HabitatInstaller.Core.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HabitatInstaller.UI.Windows
{
    /// <summary>
    /// Interaction logic for Download.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        private string _tempPath;
        private string _dlFilePath;
        private IHabitatSolution _solution;
        private bool _errors = false;

        public DownloadWindow(IHabitatSolution solution)
        {
            _solution = solution;
            InitializeComponent();
            Start();
        }
        private async Task Start()
        {
            await DownloadFile();
            await ExtractFiles();

            if (!_errors)
            {
                await RunNPM();
            }
        }

        private async Task DownloadFile()
        {
            _dlFilePath = $"{_solution.TempDownloadDirectory}Habitat.zip";

            using (var client = new WebClient())
            {
                client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 10.6; Windows NT 6.1; WOW64; Trident/5.0; InfoPath.2; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 2.0.50727) 3gpp-gba UNTRUSTED/1.0");
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadComplete);
                await client.DownloadFileTaskAsync(new Uri(_solution.SolutionDownloadUrl), _dlFilePath);

                while (client.IsBusy)
                {
                    System.Threading.Thread.Sleep(1000);
                }
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
                        _tempPath = _solution.SolutionInstallPath.Replace(dirName, $"{dirName}_temp");

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
                        //TODO: resolve ACCESS IS DENIED ERROR?
                        // Directory.SetAccessControl(_tempPath + "Habitat-master");
                        Directory.Move($"{_tempPath}Habitat-master", _solution.SolutionInstallPath);
                        Thread.Sleep(2000);
                        Directory.Delete(_tempPath, true);

                        this.Dispatcher.Invoke(() =>
                        {
                            extractLabel.IsBusy = false;
                        });

                        //update the z.Habitat.DevSettings.config file
                        var pathToDevSettingsFile = $@"{_solution.SolutionInstallPath}src\Project\Habitat\code\App_Config\Include\Project\z.Habitat.DevSettings.config";
                        File.WriteAllText(pathToDevSettingsFile, File.ReadAllText(pathToDevSettingsFile).Replace(Properties.Settings.Default.SolutionInstallPathDefault, _solution.SolutionInstallPath));
                        File.WriteAllText(pathToDevSettingsFile, File.ReadAllText(pathToDevSettingsFile).Replace(Properties.Settings.Default.HostnameDefault, _solution.Hostname));
                        //update the gulp-config.js
                        var gulpFile = $"{_solution.SolutionInstallPath}gulp-config.js";
                        File.WriteAllText(gulpFile, File.ReadAllText(gulpFile).Replace(Properties.Settings.Default.InstanceRootDefault, _solution.InstanceRoot));
                        //update the publishsettings.targets file
                        var publishSettingsFile = $"{_solution.SolutionInstallPath}publishsettings.targets";
                        File.WriteAllText(publishSettingsFile, File.ReadAllText(publishSettingsFile).Replace(Properties.Settings.Default.PublishUrlDefault, _solution.PublishUrl));

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

                //pInfo.Arguments = "/c gulp";
                //Process t = Process.Start(pInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "NPM error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessExited(object sender, System.EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
            MessageBox.Show($"Habitat solution installed to {_solution.SolutionInstallPath}", "Complete", MessageBoxButton.OK, MessageBoxImage.None);
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
