using Azure.Identity;
using Microsoft.Azure.Cosmos;
using personalSite.Models.Entities;
using personalSite.Interfaces;

namespace personalSite.Services;

public class CosmosDb : ICosmosDb
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;

    public CosmosDb()
    {
        Console.WriteLine("Connecting to CosmosDB");
        var credential = new DefaultAzureCredential();
        _cosmosClient = new CosmosClient(
            accountEndpoint: "https://db-personal-site.documents.azure.com:443/",
            tokenCredential: credential);

        _databaseName = "OnlineCv";
        Console.WriteLine("Connected to CosmosDB");
    }

    public Task<string> Test()
    {
        return Task.FromResult("TEST");
    }

    public async Task<List<Experience>> GetItemsAsync(string containerName)
    {
        Console.WriteLine("Getting items from CosmosDB");
        string query = "SELECT * FROM c";
        List<Experience> experiences = new();
        Container container = GetContainer(containerName);
        // var query = new QueryDefinition(query);
        FeedIterator<Experience> results = container.GetItemQueryIterator<Experience>(query);
        while (results.HasMoreResults)
        {
            FeedResponse<Experience> response = await results.ReadNextAsync();
            foreach (Experience experience in response)
            {
                experiences.Add(experience);
                Console.WriteLine(experience);
            }
        }
        return experiences;
    }

    // Example method to get container
    private Container GetContainer(string containerName)
    {
        return _cosmosClient.GetDatabase(_databaseName).GetContainer(containerName);
    }
}
