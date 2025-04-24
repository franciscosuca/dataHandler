using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface IExperienceHandler
{
    public Task<Experience> CreateUpdate(Experience experience);
    public Task Remover(Experience experience);
}