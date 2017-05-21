using System;
using System.Collections.Generic;
using System.IO;
using PatchalyzerCore.IO;

namespace PatchalyzerCore.Config.Repository
{
    public class RepoVersion
    {
        /// <summary>
        /// Name of the release. Doesn't have to be the same as the Semver
        /// </summary>
        public string Name;
        /// <summary>
        /// Time of release in Unix Time
        /// </summary>
        public long ReleaseDate;
        /// <summary>
        /// List of changes in this version
        /// </summary>
        public string Changelog;
        /// <summary>
        /// List of locations this release can be downloaded from
        /// </summary>
        public List<RepoSource> DownloadSources;
        /// <summary>
        /// Collection of files involved in the release
        /// </summary>
        public List<RepoFile> Files;
    }
}
