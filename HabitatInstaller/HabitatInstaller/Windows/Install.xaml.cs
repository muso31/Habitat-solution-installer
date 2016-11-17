using System.Windows;

namespace HabitatInstaller.Windows
{
    /// <summary>
    /// Interaction logic for Install.xaml
    /// </summary>
    public partial class Install : Window
    {
        public Install()
        {
            InitializeComponent();

            //try
            //{
            //    DownloadManager downloadManager = new DownloadManager();
            //    downloadManager.DownloadFile(habitatInstance.SourceUrl, habitatInstance.DownloadDirectory);
            //}
            ////the download has failed
            //catch (Exception ex)
            //{

            //}

        }
    }
}
