using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
//TEMPORARY
using personalSite.Models.Entities;
using System.Text.Json;
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

            try
            {
                var experienceChangeTriggered = JsonSerializer.Deserialize<Experience>(content);
                if (experienceChangeTriggered != null)
                {
                    _containerName = _configuration.Container;
                    //TODO: invoke a class that handles the logic for creating/modifying or deleting an experience from the db.
                    var response = await _cosmosDb.CreateExperienceAsync(_containerName, experienceChangeTriggered);
                }
                else
                    _logger.LogError("Failed to deserialize the content into an Experience object.");
            }
            catch (CosmosException e)
            {
                _logger.LogError("An Error occurred while running the function. Error message: {0}", e.Message);
            }

            _logger.LogInformation("The function has completed its task!");
        }
        //TODO: enable HTTP trigger for the frontend application.
    }
}