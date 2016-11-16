using HabitatInstaller.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HabitatInstaller
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
