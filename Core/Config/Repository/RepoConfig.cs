using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PatchalyzerCore.Config.Repository
{
    public class RepoConfig
    {
        /// <summary>
        /// The name of the repository
        /// </summary>
        public string Name;
        /// <summary>
        /// Config format revision number
        /// </summary>
        public int ConfigVersion;
        /// <summary>
        /// Version of the latest release (Semver)
        /// </summary>
        public string LatestVersion;
        /// <summary>
        /// Collection of released versions
        /// </summary>
        public Dictionary<string, RepoVersion> Versions;

        public static RepoConfig LoadJSON(string json)
        {
            return JsonConvert.DeserializeObject<RepoConfig>(File.ReadAllText(json));
        }
    }
}
