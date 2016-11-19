using System.ComponentModel.DataAnnotations;

namespace HabitatInstaller.Core.Models
{
    public class Solution : ISolution
    {
        public string SolutionDownloadUrl { get; set; }

        public string TempDownloadDirectory { get; set; }

        public string SolutionInstallPath { get; set; }

        public string InstanceRoot { get; set; }

        public string PublishUrl { get; set; }

        public string Hostname{ get; set; }
    }
}
