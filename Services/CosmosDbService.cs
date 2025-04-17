using Azure.Identity;
using Microsoft.Azure.Cosmos;
using personalSite.Models.Entities;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;

namespace personalSite.Services;

public class CosmosDb : ICosmosDb
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _databaseEndpoint;
    public CosmosDb(IOptions<Configuration> dbConfiguration)
    {
        _databaseName = dbConfiguration.Value.Name;
        _databaseEndpoint = dbConfiguration.Value.Endpoint;

        var credential = new DefaultAzureCredential();
        _cosmosClient = new CosmosClient(
            accountEndpoint: _databaseEndpoint,
            tokenCredential: credential);
    }
    public async Task<List<Experience>> GetExperiencesAsync(string containerName, IReadOnlyList<(string, PartitionKey)> items)
    {
        List<Experience> experiences = new();
        Container container = GetContainer(containerName);
        FeedResponse<Experience> feedResponse = await container.ReadManyItemsAsync<Experience>(
            items: items
        );
        foreach (Experience item in feedResponse)
            experiences.Add(item);
            return experiences;
    }
    private Container GetContainer(string containerName)
    {
        return _cosmosClient.GetDatabase(_databaseName).GetContainer(containerName);
    }
    public async Task<Experience> GetExperienceAsync(string containerName, Experience item)
    {
        Container container = GetContainer(containerName);
        Experience experience = await container.ReadItemAsync<Experience>(
            id: item.id,
            partitionKey: new PartitionKey(item.type));
        return experience;
    }
    public async Task<Experience> CreateExperienceAsync(string containerName, Experience item)
    {
        Container container = GetContainer(containerName);
        Experience createdExperience = await container.UpsertItemAsync<Experience>(
            item: item,
            partitionKey: new PartitionKey(item.type));
        return createdExperience;
    }
    public async Task<Experience> UpdateExperienceAsync(string containerName, Experience item)
    {
        Container container = GetContainer(containerName);
        Experience updatedExperience = await container.ReplaceItemAsync<Experience>(
            item: item,
            id: item.id,
            partitionKey: new PartitionKey(item.type));
        return updatedExperience;
    }
    public async Task<Experience> DeleteExperienceAsync(string containerName, Experience item)
    {
        Container container = GetContainer(containerName);
        Experience deletedExperience = await container.DeleteItemAsync<Experience>(
            id: item.id,
            partitionKey: new PartitionKey(item.type));
        return deletedExperience;
    }
}