using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PatchalyzerCore.Config.Repository;
using PatchalyzerCore.IO;

namespace PatchalyzerConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RepoConfig config = new RepoConfig
            {
                Name = "Example Project",
                ConfigVersion = 1,
                LatestVersion = "0.1.1",
                Versions = new Dictionary<string, RepoVersion>()
            };

            RepoVersion v1 = new RepoVersion("Initial Release", DateTimeOffset.Now.ToUnixTimeSeconds(), "Initial Release");
            v1.AddFiles("..\\Tests\\ExampleProject\\v1");
            v1.AddDownloadSource(new RepoSource
            {
                Name = "Main Update Server",
                Type = "HTTP",
                Location = "http://example.com/v1/"
            });
            config.Versions.Add("0.1.0", v1);

            RepoVersion v2 = new RepoVersion("Update", DateTimeOffset.Now.ToUnixTimeSeconds(), "We changed logo.png and added a new file");
            v2.AddFiles("..\\Tests\\ExampleProject\\v2", "..\\Tests\\ExampleProject\\v1");
            v2.AddDownloadSource(new RepoSource
            {
                Name = "Main Update Server",
                Type = "HTTP",
                Location = "http://example.com/v2/"
            });
            v2.AddDownloadSource(new RepoSource
            {
                Name = "Secondary FTP Server",
                Type = "FTP",
                Location = "ftp://example.com/v2/"
            });
            config.Versions.Add("0.1.1", v2);

            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamWriter sw = new StreamWriter(File.Create("..\\Tests\\ExampleProject\\out.json")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, config);
            }

            // Wait for user input
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}