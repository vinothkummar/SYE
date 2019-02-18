using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SYE.Repository.AppConfig;

namespace SYE.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Document> GetItemByIdAsync(string id);

        Task<Document> CreateItemAsync(T item);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        Task DeleteItemAsync(string id);

        Task<Document> UpdateItemAsync(string id, T item);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly DocumentClient _client;

        public GenericRepository()
        {
            var appSettings = new AppConfiguration();
            _client = new DocumentClient(new Uri(appSettings.EndPoint), appSettings.Key);
            _databaseId = appSettings.DatabaseId;
            _collectionId = appSettings.CollectionId;
        }

        public async Task<Document> GetItemByIdAsync(string id)
        {
            try
            {
                return await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                    new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(T item)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                item);
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
        }

        public async Task DeleteItemAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }

    }

}
