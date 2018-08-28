using Microsoft.Azure.Documents.Client;
using ReviewWebsite.Core.Interfaces;
using ReviewWebsite.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;

namespace ReviewWebsite.Infrastructure
{
    public class ReviewRepository : IRepository<ReviewDocument>
    {
        private readonly string _databaseName;
        private readonly string _reviewCollectionName;
        private readonly DocumentClient _client;
        private readonly Uri _collectionUri;

        public ReviewRepository(CosmosDbConfiguration CosmosDbConfiguration)
        {
            _databaseName = CosmosDbConfiguration.DatabaseName;
            _reviewCollectionName = CosmosDbConfiguration.CollectionName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _reviewCollectionName);

            _client = new DocumentClient(new Uri(CosmosDbConfiguration.EndPointUrl), CosmosDbConfiguration.AuthorizationKey,
                    new ConnectionPolicy { ConnectionMode = ConnectionMode.Direct, ConnectionProtocol = Protocol.Tcp });
        }

        public async Task<ReviewDocument> AddAsync(ReviewDocument entity)
        {
            await _client.UpsertDocumentAsync(_collectionUri, entity);

            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            await _client.DeleteDocumentAsync(
                            UriFactory.CreateDocumentUri(_databaseName, _reviewCollectionName, id));
        }

        public async Task<ReviewDocument> GetByIdAsync(string id)
        {
            return await _client.ReadDocumentAsync<ReviewDocument>(UriFactory.CreateDocumentUri(_databaseName, _reviewCollectionName, id));
        }

        public async Task<List<ReviewDocument>> ListAsync()
        {
            var reviews =
               from f in _client.CreateDocumentQuery<ReviewDocument>(_collectionUri)
               select f;

           return await reviews.ToAsyncEnumerable().ToList();
        }

        public Task<ReviewDocument> UpdateAsync(ReviewDocument entity)
        {
            return AddAsync(entity);
        }
    }
}
