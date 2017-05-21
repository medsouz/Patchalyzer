using System.IO;

namespace PatchalyzerCore.IO
{
    public class FileUtil
    {
        public static string GetRelativePath(string basePath, string path)
        {
            var outPath = Path.GetFullPath(path).Replace(Path.GetFullPath(basePath), "");
            // Remove leading / if it exists
            if (outPath.StartsWith("\\") || outPath.StartsWith("/"))
                outPath = outPath.Substring(1, outPath.Length - 1);
            return outPath;
        }
    }
}
