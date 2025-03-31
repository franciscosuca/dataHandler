using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface ICosmosDb
{
    Task<List<Experience>> GetItemsAsync(string containerName);
    Task <string> Test();

}
