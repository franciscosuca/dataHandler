using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
using personalSite.Models.Entities;
using System.Text.Json;
using personalSite.Services;
using Azure.Storage.Blobs;

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
            string name)
        {
            ExperienceHandler experienceHandler = new ExperienceHandler(_cosmosDb, _configuration.ContainerName, _loggerFactory);
            string? BlobConnectionString = Environment.GetEnvironmentVariable("f6bc32_STORAGE");
            
            if (string.IsNullOrEmpty(BlobConnectionString))
            {
                throw new Exception("No Connection String for the blob container found.");
            }

            // Create a BlobServiceClient
            var blobServiceClient = new BlobServiceClient(BlobConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("samples-workitems");
            var blobClient = containerClient.GetBlobClient(name);

            // Try to get blob properties - if we can't, it means the blob was deleted
            try 
            {
                await blobClient.GetPropertiesAsync();
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _logger.LogInformation($"Blob {name} does not exist. It may have been deleted.");
                return; // Exit if the blob does not exist
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while trying to access the blob: {ex.Message}");
                throw;
            }

            // If we get here, the blob exists and we're handling a create/update
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();

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