using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using personalSite.Models.Entities;

namespace personalSite.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(string connectionString, string blobContainerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
    }

    /// <summary>
    /// Updates the given blob based on the experience passed as an argument.
    /// </summary>
    public async Task Update(string streamName, Experience experience)
    {
        var blobClient = _containerClient.GetBlobClient(streamName);

        // Serialize the updated content
        var updatedContent = JsonSerializer.Serialize(experience);
        byte[] byteArray = Encoding.UTF8.GetBytes(updatedContent);
        using var memoryStream = new MemoryStream(byteArray);

        // Get current blob properties to get the ETag
        var properties = await blobClient.GetPropertiesAsync();
        var conditions = new BlobRequestConditions
        {
            IfMatch = properties.Value.ETag
        };

        // Upload with conditions
        await blobClient.UploadAsync(memoryStream, conditions: conditions);
    }
}