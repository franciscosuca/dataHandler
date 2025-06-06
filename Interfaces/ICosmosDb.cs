using Microsoft.Azure.Cosmos;
using personalSite.Models.Entities;

namespace personalSite.Interfaces;

public interface ICosmosDb
{
    Task<List<Experience>> GetExperiencesAsync(string containerName, IReadOnlyList<(string, PartitionKey)> items);
    Task<Experience> GetExperienceAsync(string containerName, Experience item);
    Task<Experience> CreateExperienceAsync(string containerName, Experience item);
    Task<Experience> UpdateExperienceAsync(string containerName, Experience item);
}
