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
    private readonly Configuration _dbConfiguration;

    public CosmosDb(IOptions<Configuration> dbConfiguration)
    {
        _dbConfiguration = dbConfiguration.Value;
        _databaseName = _dbConfiguration.Name;

        Console.WriteLine("Connecting to CosmosDB");
        var credential = new DefaultAzureCredential();
        _cosmosClient = new CosmosClient(
            accountEndpoint: "https://db-personal-site.documents.azure.com:443/",
            tokenCredential: credential);

        Console.WriteLine("Connected to CosmosDB");
    }

    public async Task<List<Experience>> GetItemsAsync(string containerName)
    {
        string query = "SELECT * FROM c";
        List<Experience> experiences = new();
        Container container = GetContainer(containerName);
        FeedIterator<Experience> results = container.GetItemQueryIterator<Experience>(query);
        while (results.HasMoreResults)
        {
            FeedResponse<Experience> response = await results.ReadNextAsync();
            foreach (Experience experience in response)
            {
                experiences.Add(experience);
                //TODO: delete these lines after testing
                Console.WriteLine(experience.Title);
                Console.WriteLine(experience.Location);
            }
        }
        return experiences;
    }

    private Container GetContainer(string containerName)
    {
        return _cosmosClient.GetDatabase(_databaseName).GetContainer(containerName);
    }
}
