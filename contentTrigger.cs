using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
using personalSite.Models.Entities;
using System.Text.Json;
using personalSite.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "f6bc32_STORAGE")] Stream stream, string name)
        {
            //BLOB TRIGGER
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            ExperienceHandler experienceHandler = new ExperienceHandler(_cosmosDb, _configuration.ContainerName, _loggerFactory);

            try
            {
                var experienceChangeTriggered = JsonSerializer.Deserialize<Experience>(content);
                if (experienceChangeTriggered != null)
                {
                    //TODO: check memory consumption of the function
                    Experience experienceResult = await experienceHandler.Handler(experienceChangeTriggered);
                    //TODO: move blob logic to a class
                    if (string.IsNullOrEmpty(experienceResult.id))
                    {    
                        experienceChangeTriggered = experienceResult;
                        
                        // Get connection string from app settings
                        var connectionString = Environment.GetEnvironmentVariable("f6bc32_STORAGE");
                        var blobServiceClient = new BlobServiceClient(connectionString);
                        var containerClient = blobServiceClient.GetBlobContainerClient("samples-workitems");
                        var blobClient = containerClient.GetBlobClient(name);

                        // Serialize the updated content
                        var updatedContent = JsonSerializer.Serialize(experienceChangeTriggered);
                        byte[] byteArray = Encoding.UTF8.GetBytes(updatedContent);
                        using var memoryStream = new MemoryStream(byteArray);

                        // Get current blob properties to get the ETag
                        var properties = await blobClient.GetPropertiesAsync();
                        var conditions = new BlobRequestConditions
                        {
                            IfMatch = properties.Value.ETag
                        };

                        // Upload with conditions - single attempt
                        await blobClient.UploadAsync(memoryStream, conditions: conditions);
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

            _logger.LogDebug("The function has completed its task!");
        }
    }
}