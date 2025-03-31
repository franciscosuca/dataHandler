using Microsoft.Azure.Cosmos;
using Azure.Core;
using Azure.Identity;

public record Experience(
    string Company,
    string Position
);
public class CosmosDbService
{
    private readonly CosmosClient dbClient;
    private readonly Database onlineCvDatabase;

    public CosmosDbService()
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