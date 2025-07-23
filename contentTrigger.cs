using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;
using personalSite.Models.Entities;
using System.Text.Json;
using personalSite.Services;

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
        private const string _containerName = "onlinecv";

        [FunctionName(nameof(contentTrigger))]
        public async Task Run(
            [BlobTrigger(_containerName + "/{name}",
            Connection = "f6bc32_STORAGE")] Stream stream,
            string name)
        {
            ExperienceHandler experienceHandler = new ExperienceHandler(_cosmosDb, _configuration.ContainerName, _loggerFactory);
            //TODO: replace the f6bc32_STORAGE for AzureWebJobsStorage
            string? BlobConnectionString = Environment.GetEnvironmentVariable("f6bc32_STORAGE");

            if (string.IsNullOrEmpty(BlobConnectionString))
            {
                throw new Exception("No Connection String for the blob container found.");
            }

            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();

            BlobService blobService = new(BlobConnectionString, _containerName);

            try
            {
                var experienceChangeTriggered = JsonSerializer.Deserialize<Experience>(content);
                if (experienceChangeTriggered != null)
                {
                    Experience experienceResult;

                    if (string.IsNullOrEmpty(experienceChangeTriggered.id))
                    {
                        experienceResult = await experienceHandler.Create(experienceChangeTriggered);
                        await blobService.Update(name, experienceResult); // Only update the blob when creating a new experience to add the ID
                        _logger.LogInformation($"Experience {experienceResult.id} was created successfully.");
                    }
                    else
                    {
                        experienceResult = await experienceHandler.Update(experienceChangeTriggered);
                        _logger.LogInformation($"Experience {experienceResult.id} was processed successfully.");
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