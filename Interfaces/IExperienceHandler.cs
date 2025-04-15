using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface IExperienceHandler
{
    public Task<Experience> Handler(Experience experience);
    public Task Remover(Experience experience);
}