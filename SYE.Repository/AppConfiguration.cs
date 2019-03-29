
namespace SYE.Repository
{
    public interface ISearchConfiguration
    {
        string SearchServiceName { get; set; }
        string SearchApiKey { get; set; }
        string IndexName { get; set; }
    }

    public interface IAppConfiguration<T> where T : class
    {
        string Endpoint { get; set; }
        string Key { get; set; }
        string DatabaseId { get; set; }
        string CollectionId { get; set; }
    }

    public class AppConfiguration<T> : IAppConfiguration<T> where T : class
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
    }

    public class SearchConfiguration : ISearchConfiguration
    {
        public string SearchServiceName { get; set; }
        public string SearchApiKey { get; set; }
        public string IndexName { get; set; }
    }
}
