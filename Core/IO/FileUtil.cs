using System.Collections.Generic;
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

		public static void GetFiles(string root, ref List<string> fileList)
		{
			string[] files = Directory.GetFiles(root);
			fileList.AddRange(files);

			var directories = Directory.GetDirectories(root);
			foreach (var directory in directories)
				GetFiles(directory, ref fileList);
		}
    }
}
