using HabitatInstaller.Core.Models;
using HabitatInstaller.UI.Windows;

namespace HabitatInstaller.Repository
{
    public interface ISolutionRepository
    {
        ISolution MapUserInput(ISolution solution);
    }
}
