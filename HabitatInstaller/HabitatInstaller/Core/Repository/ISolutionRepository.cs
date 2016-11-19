using HabitatInstaller.Core.Models;

namespace HabitatInstaller.Repository
{
    public interface ISolutionRepository
    {
        ISolution Create(ISolution solution);
    }
}
