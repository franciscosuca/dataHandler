using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using personalSite.Interfaces;
using Microsoft.Extensions.Options;

namespace personalSite
{
    public class contentTrigger
    {
        private readonly ILogger<contentTrigger> _logger;
        private readonly ICosmosDb _cosmosDb;
        private readonly Configuration _configuration;
        private string _containerName;

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

            //COSMOSDB SERVICE
            _containerName = _configuration.Container;

            // Examples of using the CosmosDB service
            //TODO: Create a new item in the Experience container
            //* Get all items from the Experience container
            var res = await _cosmosDb.GetItemsAsync(_containerName);
            // Update an item in the Experience container
            // Get a single item from the Experience container
            // Delete an item from the Experience container

        }
    }
}
