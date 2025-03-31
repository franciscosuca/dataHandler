using personalSite.Models.Entities;

namespace personalSite;

public interface ICosmosDb
{
    Task<List<Experience>> GetItemsAsync(string containerName);

}
