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

        public RepoVersion(string name, long releaseDate, string changelog)
        {
            Name = name;
            ReleaseDate = releaseDate;
            Changelog = changelog;
        }

        public void AddDownloadSource(RepoSource source)
        {
            if(DownloadSources == null)
                DownloadSources = new List<RepoSource>();
            DownloadSources.Add(source);
        }

        public void AddFiles(string path, string previousVersionPath = null)
        {
            Files = new List<RepoFile>();

            var fileList = new List<string>();
            GetFiles(path, ref fileList);

            foreach (var file in fileList)
            {
                var repoFile = new RepoFile
                {
                    Path = FileUtil.GetRelativePath(path, file),
                    Checksum = Checksum.GetSHA1Sum(file)
                };

                // Generate patches
                if (previousVersionPath != null)
                {
                    var prevPath = Path.Combine(previousVersionPath, repoFile.Path);
                    if (File.Exists(prevPath))
                    {
                        var origChecksum = Checksum.GetSHA1Sum(prevPath);
                        // Don't generate a patch for an unchanged file
                        if (origChecksum != repoFile.Checksum)
                        {
                            repoFile.PatchSourceChecksum = origChecksum;
                            repoFile.PatchChecksum = "TODO";
                        }
                    }
                }

                Files.Add(repoFile);
            }
        }

        private void GetFiles(string root, ref List<string> fileList)
        {
            string[] files = Directory.GetFiles(root);
            fileList.AddRange(files);

            var directories = Directory.GetDirectories(root);
            foreach (var directory in directories)
                GetFiles(directory, ref fileList);
        }
    }
}
