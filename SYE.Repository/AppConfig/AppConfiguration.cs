using System.IO;
using Microsoft.Extensions.Configuration;

namespace SYE.Repository.AppConfig
{
    public class AppConfiguration
    {
        public string EndPoint { get; set; }
        public string Key { get; set; }
        public string DatabaseId { get; set; }
        public string CollectionId { get; set; }


        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            var section = root.GetSection("CosmosDB");

            EndPoint = section.GetSection("EndPoint").Value;
            Key = section.GetSection("Key").Value;
            DatabaseId = section.GetSection("DatabaseId").Value;
            CollectionId = section.GetSection("CollectionId").Value;

        }

    }
}
