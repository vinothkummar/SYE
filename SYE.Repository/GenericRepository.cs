using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SYE.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Document> CreateAsync(T item);
        Task DeleteAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<T> GetAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> ascKeySelector, Expression<Func<T, TKey>> descKeySelector);
        Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
        Task<Document> UpdateAsync(string id, T item);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _databaseId;
        private readonly string _collectionId;
        private readonly IDocumentClient _client;
        private readonly IAppConfiguration<T> _appConfig;

        public GenericRepository(IAppConfiguration<T> appConfig, ICosmosDocumentClient<T> client)
        {
            _appConfig = appConfig;
            _databaseId = _appConfig.DatabaseId;
            _collectionId = _appConfig.CollectionId;
            _client = client.Client;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var param = UriFactory.CreateDocumentUri(_databaseId, _collectionId, id);
            Document document = await _client.ReadDocumentAsync(param);
            return (T)(dynamic)document;
        }

        public async Task<T> GetAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> ascKeySelector, Expression<Func<T, TKey>> descKeySelector)
        {
            return await Task.Run(() =>
            {
                var endPoint = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId);

                IQueryable<T> query = _client.CreateDocumentQuery<T>(endPoint, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true } );
                if (predicate != null)
                {
                    query = query.Where(predicate);
                }
                if (ascKeySelector != null)
                {
                    query = query.OrderBy(ascKeySelector);
                }
                if (descKeySelector != null)
                {
                    query = query.OrderByDescending(descKeySelector);
                }
                return query.AsEnumerable().FirstOrDefault() as T;
            });
        }

        public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateAsync(T item)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
        }

        public async Task<Document> UpdateAsync(string id, T item)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
        }

        public async Task DeleteAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
        }

    }

}
