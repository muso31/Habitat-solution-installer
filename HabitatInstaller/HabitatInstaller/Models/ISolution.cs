using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitatInstaller.Models
{
    public interface ISolution
    {
        string SolutionDownloadUrl { get; set; }

        string TempDownloadDirectory { get; set; }

        string SolutionInstallPath { get; set; }

        string InstanceRoot { get; set; }

        string PublishUrl { get; set; }

        string Hostname { get; set; }
    }
}
