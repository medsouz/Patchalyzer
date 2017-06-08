using System;
using System.Collections.Generic;
using System.IO;
using deltaq.BsDiff;
using PatchalyzerCore.Config.Repository;
using PatchalyzerCore.IO;
using Semver;

namespace PatchalyzerCore.Builder
{
    public class RepoVersionBuilder
    {
        public static RepoVersion Build(Patchalyzer patchalyzer, SemVersion version, string path, string name, string changelog = "", long releaseDate = -1)
        {
            RepoConfig config = patchalyzer.GetRepoConfig();
            string repoPath = patchalyzer.GetRepoPath();

            // If releaseDate is missing then set it to the current time.
            if (releaseDate == -1)
                releaseDate = DateTimeOffset.Now.ToUnixTimeSeconds();

            RepoVersion repoVersion = new RepoVersion
            {
                Name = name,
                ReleaseDate = releaseDate,
                Changelog = changelog
            };

            bool isUpdate = false;
            // Check if a previous version exists
            if (config.LatestVersion != null && config.Versions.ContainsKey(config.LatestVersion))
            {
                SemVersion latestVersion;
                if (SemVersion.TryParse(config.LatestVersion, out latestVersion))
                    isUpdate = (latestVersion < version);
            }

            List<RepoFile> files = new List<RepoFile>();

            if (isUpdate)
                files = AddFiles(repoPath, version, path, config.LatestVersion, config.Versions[config.LatestVersion]);
            else
                files = AddFiles(repoPath, version, path);

            repoVersion.Files = files;

            return repoVersion;
        }

        private static List<RepoFile> AddFiles(string repoPath, SemVersion version, string path, string previousVersionNumber = null, RepoVersion previousVersion = null)
        {
            string previousVersionPath = null;
            if (previousVersionNumber != null)
                previousVersionPath = Path.Combine(repoPath, previousVersionNumber);

            Console.WriteLine("Adding files from " + path + ((previousVersionPath != null) ? " | Comparing them to: " + previousVersionPath : ""));
            List<RepoFile> files = new List<RepoFile>();

            var fileList = new List<string>();
            GetFiles(path, ref fileList);

            foreach (var file in fileList)
            {
                var repoFile = new RepoFile
                {
                    Path = FileUtil.GetRelativePath(path, file).Replace("\\", "/"),
                    Checksum = Checksum.GetSHA1Sum(file)
                };

                string exportPath = Path.Combine(repoPath, version.ToString(), repoFile.Path);

                if (previousVersion != null)
                {
                    var oldRepoFile = previousVersion.GetFile(repoFile.Path);

                    if (oldRepoFile?.Checksum == repoFile.Checksum)
                    {
                        // Flag the file as unchanged to consolidate repo space
                        repoFile.UnchangedSince = oldRepoFile.UnchangedSince ?? previousVersionNumber;
                    }
                    else
                    {
                        Console.WriteLine("\tAdding file " + repoFile.Path);

                        // Copy the entire file if a previous version doesn't exist.
                        Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
                        File.Copy(file, exportPath);

                        // Generate patches
                        string prevPath;
                        if (oldRepoFile?.UnchangedSince != null)
                        {
                            Console.WriteLine("\t\tUnchanged since " + oldRepoFile.UnchangedSince);
                            prevPath = Path.Combine(Path.Combine(repoPath, oldRepoFile.UnchangedSince), repoFile.Path);
                        }
                        else
                        {
                            prevPath = Path.Combine(previousVersionPath, repoFile.Path);
                        }

                        if (File.Exists(prevPath))
                        {
                            var origChecksum = Checksum.GetSHA1Sum(prevPath);
                            // Don't generate a patch for an unchanged file
                            if (origChecksum != repoFile.Checksum)
                            {
                                Console.WriteLine("\t\tGenerating patch...");

                                repoFile.PatchSourceChecksum = origChecksum;
                                // Create the BsDiff patch
                                using (FileStream patchFileStream =
                                    new FileStream(exportPath + ".patch", FileMode.Create))
                                    BsDiff.Create(File.ReadAllBytes(exportPath), File.ReadAllBytes(prevPath),
                                        patchFileStream);
                                // Get checksum of the patch
                                repoFile.PatchChecksum = Checksum.GetSHA1Sum(exportPath + ".patch");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\tAdding file " + repoFile.Path);
                    // Copy the entire file if a previous version doesn't exist.
                    Directory.CreateDirectory(Path.GetDirectoryName(exportPath));
                    File.Copy(file, exportPath);
                }


                files.Add(repoFile);
            }

            return files;
        }

        private static void GetFiles(string root, ref List<string> fileList)
        {
            string[] files = Directory.GetFiles(root);
            fileList.AddRange(files);

            var directories = Directory.GetDirectories(root);
            foreach (var directory in directories)
                GetFiles(directory, ref fileList);
        }
    }
}
