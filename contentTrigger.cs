using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;

namespace personalSite
{
    public class contentTrigger
    {
        private readonly ILogger<contentTrigger> _logger;
        private readonly ICosmosDb _cosmosDb;

        public contentTrigger(ILogger<contentTrigger> logger, ICosmosDb cosmosDb)
        {
            _logger = logger;
            _cosmosDb = cosmosDb;
        }

        [FunctionName(nameof(contentTrigger))]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "f6bc32_STORAGE")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            // Example of using the CosmosDB service
            var res = await _cosmosDb.Test();
        }
    }
}
