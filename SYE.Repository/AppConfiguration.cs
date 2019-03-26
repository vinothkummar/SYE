using Microsoft.Azure.Documents;

namespace SYE.Repository
{
    public interface IAppConfiguration<T> where T : class
    {
        string Endpoint { get; set; }
        string Key { get; set; }
        string DatabaseId { get; set; }
        string CollectionId { get; set; }
        string SearchServiceName { get; set; }
        string SearchApiKey { get; set; }
        string IndexName { get; set; }
    }


    public class AppConfiguration<T> : IAppConfiguration<T> where T : class
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
    }

    public interface ICosmosDocumentClient<T> where T : class
    {
        IDocumentClient Client { get; set; }
    }

    public class CosmosDocumentClient<T> : ICosmosDocumentClient<T> where T : class
    {
        public IDocumentClient Client { get; set; }
        public string CollectionId { get; set; }
        public string SearchServiceName { get; set; }
        public string SearchApiKey { get; set; }
        public string IndexName { get; set; }
    }
}
