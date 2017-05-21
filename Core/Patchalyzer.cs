using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PatchalyzerCore.Builder;
using PatchalyzerCore.Config.Repository;
using Semver;

namespace PatchalyzerCore
{
    public class Patchalyzer
    {
        private string RepoPath;
        private RepoConfig Config;

        /// <summary>
        /// Load an existing Patchalyzer project from a path.
        /// </summary>
        /// <param name="repoPath">Path to the repository</param>
        public Patchalyzer(string repoPath)
        {
            LoadRepo(repoPath, RepoConfig.LoadJSON(Path.Combine(Path.GetFullPath(RepoPath), "patchalyzer.json")));
        }

        /// <summary>
        /// Load an existing Patchalyzer project from an already loaded RepoConfig instance.
        /// </summary>
        /// <param name="repoPath">Path to the repository</param>
        /// <param name="repoConfig">Instance of the RepoConfig</param>
        public Patchalyzer(string repoPath, RepoConfig repoConfig)
        {
            LoadRepo(repoPath, repoConfig);
        }

        /// <summary>
        /// Create a new Patchalyzer repo.
        /// </summary>
        /// <param name="repoPath">Path to create the repository in.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <returns>The newly created Patchalyzer project.</returns>
        public static Patchalyzer InitRepo(string repoPath, string projectName)
        {
            RepoConfig config = new RepoConfig
            {
                Name = projectName,
                ConfigVersion = 1
            };

            return new Patchalyzer(repoPath, config);
        }

        private void LoadRepo(string repoPath, RepoConfig repoConfig)
        {
            RepoPath = repoPath;
            Config = repoConfig;
        }

        /// <summary>
        /// Write the repository to the disk.
        /// </summary>
        public void SaveRepo()
        {
            Directory.CreateDirectory(RepoPath);

            JsonSerializer serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (StreamWriter sw = new StreamWriter(File.Create(Path.Combine(Path.GetFullPath(RepoPath), "patchalyzer.json"))))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, Config);
            }
        }

        public void AddVersion(SemVersion version, string path, string name, string changelog = "", long releaseDate = -1)
        {
            if(Config.Versions == null)
                Config.Versions = new Dictionary<string, RepoVersion>();

            Config.Versions[version.ToString()] = RepoVersionBuilder.Build(this, version, path, name, changelog, releaseDate);
            Config.LatestVersion = version.ToString();
        }

        public string GetRepoPath()
        {
            return RepoPath;
        }

        public RepoConfig GetRepoConfig()
        {
            return Config;
        }
    }
}
