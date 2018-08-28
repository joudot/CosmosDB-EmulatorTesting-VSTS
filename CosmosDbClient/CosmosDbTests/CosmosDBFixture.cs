using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using ReviewWebsite.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace CosmosDbTests
{
    public class CosmosDBFixture : IDisposable
    {
        private readonly IConfigurationRoot _config;
        private DocumentClient _client;

        public CosmosDBFixture()
        {
              _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            CosmosDbConfig = new CosmosDbConfiguration
            {
                AuthorizationKey = _config.GetSection("CosmosDbConfiguration")["AuthorizationKey"],
                CollectionName = _config.GetSection("CosmosDbConfiguration")["CollectionName"],
                DatabaseName = _config.GetSection("CosmosDbConfiguration")["DatabaseName"],
                EndPointUrl = _config.GetSection("CosmosDbConfiguration")["EndPointUrl"]
            };
            // Initializing collection with configuration
            _client = new DocumentClient(
               new Uri(CosmosDbConfig.EndPointUrl),
               CosmosDbConfig.AuthorizationKey);
            CreateReviewCollectionAndInitializeIfNotExistsAsync().Wait();
        }


        public CosmosDbConfiguration CosmosDbConfig { get; private set; }

        public async Task CreateReviewCollectionAndInitializeIfNotExistsAsync()
        {
            try
            {
                DocumentCollection collection = new DocumentCollection();
                collection.Id = CosmosDbConfig.CollectionName;

                // Add Partition Key to test this scenario if needed
                // collection.PartitionKey.Paths.Add("/reviewId");

                collection.IndexingPolicy.Automatic = true;

                // TIPS: Lazy will improve write performance but might returned outdated result for a short period of time
                collection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;

                Database databaseInfo = new Database { Id = CosmosDbConfig.DatabaseName };
                await this._client.CreateDatabaseIfNotExistsAsync(databaseInfo);

                collection = await this._client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(CosmosDbConfig.DatabaseName),
                    collection,
                    new RequestOptions { OfferThroughput = 400 });
                await CosmosDbInitUtils.RunBulkImportAsync(_client, collection.SelfLink);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                
               throw e;
            }
        }

        public void Dispose()
        {
            // Cleanup 
        }
    }
}
