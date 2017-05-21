namespace PatchalyzerCore.Config.Repository
{
    public class RepoFile
    {
        /// <summary>
        /// Contains the path of the file relative to the repository root
        /// </summary>
        public string Path;
        /// <summary>
        /// SHA1 checksum of the file
        /// </summary>
        public string Checksum;
        /// <summary>
        /// Checksum of the patch file. Can be null.
        /// </summary>
        public string PatchChecksum;
        /// <summary>
        /// Checksum of the file the is applied to. Can be null.
        /// </summary>
        public string PatchSourceChecksum;
    }
}
