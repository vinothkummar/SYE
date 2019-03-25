using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace SYE.Services.Wrappers
{
    public interface ICustomSearchIndexClient
    {
        //DocumentSearchResult<T> Search<T>(string searchTerm, SearchParameters parameters) where T : class;
        Task<DocumentSearchResult> SearchAsync(string searchTerm, SearchParameters parameters);
    }

    public class CustomSearchIndexClient : ICustomSearchIndexClient
    {
        private readonly SearchIndexClient _searchIndexClient;

        public CustomSearchIndexClient(string searchServiceName, string indexName, string apiKey)
        {
            _searchIndexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(apiKey));
        }

        public async Task<DocumentSearchResult> SearchAsync(string searchTerm, SearchParameters parameters)
        {
            return await _searchIndexClient.Documents.SearchAsync(searchTerm, parameters);
        }
    }
}
