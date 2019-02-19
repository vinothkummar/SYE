using Microsoft.Extensions.Configuration;
using System.IO;

namespace SYE.Repository
{
    public interface IAppConfiguration
    {
        string Endpoint { get; }
        string Key { get; }
        string DatabaseId { get; }
        string CollectionId { get; }
    }


    public class AppConfiguration : IAppConfiguration
    {

        public string Endpoint { get; }

        public string Key { get; }

        public string DatabaseId { get; }

        public string CollectionId { get; }


        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            
            var submissionDb = root.GetSection("ConnectionStrings").GetSection("SubmissionsDb");
            Endpoint = submissionDb["Endpoint"];
            Key = submissionDb["Key"];
            DatabaseId = submissionDb["DatabaseId"];
            CollectionId = submissionDb["CollectionId"];

        }
        
    }
}
