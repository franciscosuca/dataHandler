using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface IBlob
{
    public Task Update(string streamName, Experience experience);
}