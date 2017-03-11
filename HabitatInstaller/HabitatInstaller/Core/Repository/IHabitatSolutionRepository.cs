using HabitatInstaller.Core.Models;

namespace HabitatInstaller.Repository
{
    public interface IHabitatSolutionRepository
    {
        IHabitatSolution MapUserInput(IHabitatSolution solution);
    }
}
