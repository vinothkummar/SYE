
namespace SYE.Repository
{
    public interface ISearchConfiguration
    {
        string SearchServiceName { get; set; }
        string SearchApiKey { get; set; }
        string IndexName { get; set; }
    }

    public interface ICosmosConnection
    {
        string Endpoint { get; set; }
        string Key { get; set; }
    }

    public class CosmosConnection : ICosmosConnection
    {
        public string Endpoint { get; set; }
        public string Key { get; set; }
    }

    public interface IAppConfiguration<T> where T : class
    {
        string DatabaseId { get; set; }
        string CollectionId { get; set; }
        string ConfigRecordId { get; set; }
    }
    public class AppConfiguration<T> : IAppConfiguration<T> where T : class
    {
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }
        public string ConfigRecordId { get; set; }
    }

    public class SearchConfiguration : ISearchConfiguration
    {
        public string SearchServiceName { get; set; }
        public string SearchApiKey { get; set; }
        public string IndexName { get; set; }
    }

    public interface IEmailFieldMapping
    {
        string Name { get; set; }
        string TemplateField { get; set; }
        string FormField { get; set; }
    }

    public class EmailFieldMapping : IEmailFieldMapping
    {
        public string Name { get; set; }
        public string TemplateField { get; set; }
        public string FormField { get; set; }
    }

}
