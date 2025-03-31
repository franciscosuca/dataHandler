using Microsoft.Azure.Cosmos;
using Azure.Core;
using Azure.Identity;

public record Experience(
    string Company,
    string Position
);
public class TestCosmosDbService
{
    private readonly CosmosClient dbClient;
    private readonly Database onlineCvDatabase;

    public TestCosmosDbService()
    {
        Console.WriteLine("Connecting to CosmosDB");
        var credential = new DefaultAzureCredential();
        dbClient = new CosmosClient(
            accountEndpoint: "https://db-personal-site.documents.azure.com:443/",
            tokenCredential: credential);
        
        onlineCvDatabase = dbClient.GetDatabase("OnlineCv");
        Console.WriteLine("Connected to CosmosDB");
    }

    public async Task<List<Experience>> GetItems(string containerName)
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
    public Container GetContainer(string containerName)
    {
        return onlineCvDatabase.GetContainer(containerName);
    }
        
}

using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace DataHandler.Core.Services
{
    public interface ICosmosDbService
    {
        Task<ItemResponse<T>> CreateItemAsync<T>(string containerName, T item);
        Task<ItemResponse<T>> GetItemAsync<T>(string containerName, string id, string partitionKey);
        Task<ItemResponse<T>> UpdateItemAsync<T>(string containerName, string id, T item, string partitionKey);
        Task<ItemResponse<T>> DeleteItemAsync<T>(string containerName, string id, string partitionKey);
    }


    //TODO To be tested

    public class CosmosDbService : ICosmosDbService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName;

        public CosmosDbService(string accountEndpoint, string databaseName)
        {
            _cosmosClient = new CosmosClient(accountEndpoint);
            _databaseName = databaseName;
        }

        public async Task<ItemResponse<T>> CreateItemAsync<T>(string containerName, T item)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            return await container.CreateItemAsync(item);
        }

        public async Task<ItemResponse<T>> GetItemAsync<T>(string containerName, string id, string partitionKey)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            return await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public async Task<ItemResponse<T>> UpdateItemAsync<T>(string containerName, string id, T item, string partitionKey)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            return await container.UpsertItemAsync(item, new PartitionKey(partitionKey));
        }

        public async Task<ItemResponse<T>> DeleteItemAsync<T>(string containerName, string id, string partitionKey)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            return await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }
    }
} 