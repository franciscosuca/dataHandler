using Microsoft.Extensions.Logging;
using personalSite.Interfaces;
using personalSite.Models.Entities;

namespace personalSite.Services;

public class ExperienceHandler: IExperienceHandler
{
    private readonly ILogger<ExperienceHandler> _logger;
    private readonly ICosmosDb _cosmosDb;
    private string _cosmosDbContainerName = string.Empty;

    /// <summary>
    /// Handles the creation, modification, or deletion of an experience in the database.
    /// </summary>
    /// <remarks>
    /// This method performs the following actions based on the state of the experience:
    /// <list type="bullet">
    /// <item>
    /// <description>If the experience does not exist in the database, it creates a new entry.</description>
    /// </item>
    /// <item>
    /// <description>If the experience already exists, it updates the existing entry.</description>
    /// </item>
    /// <item>
    /// <description>If the experience exists but has been removed from the blob, it deletes the entry.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public ExperienceHandler(ICosmosDb cosmosDb, string containerName, ILoggerFactory loggerFactory)
    {
        _cosmosDb = cosmosDb;
        _cosmosDbContainerName = containerName;
        _logger = loggerFactory.CreateLogger<ExperienceHandler>();
    }

    public async Task<Experience> CreateUpdate(Experience experience)
    {
        if (string.IsNullOrEmpty(experience.id))
        {
            experience.id = Guid.NewGuid().ToString();
            Experience experienceResult = await _cosmosDb.CreateExperienceAsync(_cosmosDbContainerName, experience);
            _logger.LogInformation($"Experience {experience.id} was created.");
            return experienceResult;
        }
        else
        {
            Experience experienceResult = await _cosmosDb.UpdateExperienceAsync(_cosmosDbContainerName, experience);
            _logger.LogInformation($"Experience {experience.id} was updated.");
            return experienceResult;
        }
    }

    //TODO: check if this should actually exists on the mehtod above
    public async Task Remover(Experience experience)
    {
        //TODO: identify what information the blob-triggers gives when the blob is deleted
        //TODO: then, delete the info in Cosmos
        await _cosmosDb.DeleteExperienceAsync(_cosmosDbContainerName, experience);
        _logger.LogInformation($"Experience {experience.id} was removed.");
    }
}