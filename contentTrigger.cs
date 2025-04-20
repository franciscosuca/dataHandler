using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
using personalSite.Models.Entities;
using System.Text.Json;
using personalSite.Services;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Microsoft.WindowsAzure.Storage.Blob;

namespace personalSite
{
    public class contentTrigger
    {
        private readonly ILogger<contentTrigger> _logger;
        private readonly ICosmosDb _cosmosDb;
        private readonly Configuration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public contentTrigger(
            ILogger<contentTrigger> logger, 
            ICosmosDb cosmosDb, 
            IOptions<Configuration> configuration,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _cosmosDb = cosmosDb;
            _configuration = configuration.Value;
            _loggerFactory = loggerFactory;
        }

        [FunctionName(nameof(contentTrigger))]
        public async Task Run(
            [BlobTrigger("samples-workitems/{name}", Connection = "f6bc32_STORAGE")] Stream stream,
            string name,
            IDictionary<string, string> metadata, //TODO: remove this argument if is not needed.
            BlobTriggerAttribute attr) //TODO: remove this argument if is not needed.
        {
            ExperienceHandler experienceHandler = new ExperienceHandler(_cosmosDb, _configuration.ContainerName, _loggerFactory);

            // Check if this is a deletion event (stream will be empty)
            if (stream.Length == 0)
            {
                _logger.LogInformation($"Blob {name} was deleted. Removing corresponding experience...");
                // Since the blob is deleted, we need to find and remove the corresponding experience
                // You might need to implement a way to get the Experience ID from the blob name
                //TODO: adjust this so the correct experience is removed from the dB
                var experience = new Experience { id = name };
                //await experienceHandler.Remover(experience);
                return;
            }

            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();

            string? BlobConnectionString = Environment.GetEnvironmentVariable("f6bc32_STORAGE");
            if (string.IsNullOrEmpty(BlobConnectionString))
            {
                throw new Exception("No Connection String for the blob container found.");
            }

            BlobService blobService = new(BlobConnectionString, "samples-workitems");

            try
            {
                var experienceChangeTriggered = JsonSerializer.Deserialize<Experience>(content);
                if (experienceChangeTriggered != null)
                {
                    Experience experienceResult = await experienceHandler.CreateUpdate(experienceChangeTriggered);
                    
                    if (string.IsNullOrEmpty(experienceResult.id))
                    {
                        experienceChangeTriggered = experienceResult;
                        await blobService.Update(name, experienceChangeTriggered);
                    }
                }
                else
                {
                    _logger.LogError("Failed to deserialize the content into an Experience object.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An Error occurred while running the function. Error message: {e.Message}");
                throw;
            }

            _logger.LogInformation("The function has completed its task!");
        }
    }
}