using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Configuration
{
    public static class IConfigurationExtensions
    {
        public static TClass Read<TClass>(this IConfiguration configuration) where TClass : class, new()
        {
            var o = new TClass();
            var section = configuration.GetSection(typeof(TClass).Name);
            if (section.Exists())
            {
                section.Bind(o);
            }
            return o;
        }

        public static TClass Bind<TClass>(this TClass @class, string directory, string environmentName = null) where TClass : class
        {
            var jsonname = GetJsonName<TClass>(environmentName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile(jsonname, optional: true, reloadOnChange: false);

            if(string.IsNullOrEmpty(environmentName))
            {
                jsonname = GetJsonName<TClass>(Hosting.Environments.Production); 
                builder.AddJsonFile(jsonname, optional: true, reloadOnChange: false);
            }

            var configuration = builder.Build();

            configuration.Bind(@class);
            return @class;
        }

        


        public static async Task SaveAsync<TClass>(this TClass @class, string directory, string environmentName = null) where TClass : class
        {
            var filename = GetJsonPath<TClass>(directory, environmentName);

            if(string.IsNullOrEmpty(environmentName))
            {
                var production = GetJsonPath<TClass>(directory, Hosting.Environments.Production);
                if (File.Exists(production))
                {
                    filename = production;
                }
            }

            var jsonContent = JsonConvert.SerializeObject(@class, Formatting.Indented);
            
            using (var writer = File.CreateText(filename))
            {
                await writer.WriteAsync(jsonContent);
            }

        }
        

        public static void Save<TClass>(this TClass @class, string directory, string environmentName = null) where TClass : class
        {
            var filename = GetJsonPath<TClass>(directory, environmentName);

            if (string.IsNullOrEmpty(environmentName))
            {
                var production = GetJsonPath<TClass>(directory, Hosting.Environments.Production);
                if (File.Exists(production))
                {
                    filename = production;
                }
            }

            var jsonContent = JsonConvert.SerializeObject(@class, Formatting.Indented);
            File.WriteAllText(filename, jsonContent);
        }


        private static string GetJsonName<TClass>(string environmentName = null)
            => $"{typeof(TClass).Name}{GetSuffix(environmentName)}";


        private static string GetJsonPath<TClass>(string directory, string environmentName = null)
            => GetJsonPath(typeof(TClass), directory, environmentName);


        private static string GetJsonPath(Type type, string directory, string environmentName = null)
            => Path.Combine(directory, $"{type.Name}{GetSuffix(environmentName)}");


        private static string GetSuffix(string environmentName = null)
            => $"{(string.IsNullOrEmpty(environmentName) ? "" : ("." + environmentName))}.json";
    }
}
