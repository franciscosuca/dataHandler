using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface IExperienceHandler
{
    public Task<Experience> Create(Experience experience);
    public Task<Experience> Update(Experience experience);
}