using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
//TEMPORARY
using personalSite.Models.Entities;
using Microsoft.Azure.Cosmos;

namespace personalSite{

    public class contentTrigger
    {
        private readonly ILogger<contentTrigger> _logger;
        private readonly ICosmosDb _cosmosDb;
        private readonly Configuration _configuration;
        private string _containerName = string.Empty;

        public contentTrigger(ILogger<contentTrigger> logger, ICosmosDb cosmosDb, IOptions<Configuration> configuration)
        {
            _logger = logger;
            _cosmosDb = cosmosDb;
            _configuration = configuration.Value;
        }

        [FunctionName(nameof(contentTrigger))]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "f6bc32_STORAGE")] Stream stream, string name)
        {
            //BLOB TRIGGER
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            //TODO: parse the content into an Experience object

            //COSMOSDB SERVICE
            //TODO: invoke a class that handles the logic for creating/modifying or deleting an experience from the db.
            _containerName = _configuration.Container;

            //!Section to be removed
            Experience experience = new() { id = "Test", title = "Test", type = "research", sdate = "Test", edate = "Test", company = "Test", location = "Test", summary = "Test", sampleSkills = "Test" };
            //* Create an item
            await _cosmosDb.CreateExperienceAsync(_containerName, experience);
            //* Get all items from the Experience container
            PartitionKey pt = new("research");
            List<(string, PartitionKey)> testItems = new()
            {
                ("8be6d831-819d-4cb4-ab0f-79bc014ac397", pt),
                ("Test", pt)
            };         
            List<Experience>exps = await _cosmosDb.GetExperiencesAsync(_containerName, (IReadOnlyList<(string, PartitionKey)>)testItems);
            Console.WriteLine(exps);
            //* Update an item in the Experience container
            experience.title = "tasty test!";
            await _cosmosDb.UpdateExperienceAsync(_containerName, experience);
            //* Get a single item from the Experience container
            var singleExperience = await _cosmosDb.GetExperienceAsync(_containerName, experience);
            Console.WriteLine(singleExperience.title);
            //* Delete an item from the Experience container
            await _cosmosDb.DeleteExperienceAsync(_containerName, experience);
            
            _logger.LogInformation("The function has completed its task!");
        }
    }
}