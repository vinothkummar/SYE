
namespace SYE.Repository
{
    public interface IAppConfiguration
    {
        string Endpoint { get; set; }
        string Key { get; set; }
        string DatabaseId { get; set; }
        string CollectionId { get; set; }
    }


    public sealed class AppConfiguration : IAppConfiguration
    {

        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }

        public string CollectionId { get; set; }


    }
}
