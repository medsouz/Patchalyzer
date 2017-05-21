namespace PatchalyzerCore.Config.Repository
{
    public class RepoSource
    {
        /// <summary>
        /// The name of this download source.
        /// </summary>
        public string Name;
        /// <summary>
        /// The type of download. Example: HTTP, FTP, Torrent
        /// </summary>
        public string Type;
        /// <summary>
        /// The URL/Magnet Link/etc of this download source.
        /// </summary>
        public string Location;
    }
}