using System;
using System.IO;
using System.Security.Cryptography;

namespace PatchalyzerCore.IO
{
    public class Checksum
    {
        /// <summary>
        /// Get SHA1 checksum of a file
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns>SHA1 checksum</returns>
        public static string GetSHA1Sum(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                using (var sha1 = SHA1.Create())
                {
                    byte[] checksum = sha1.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", string.Empty);
                }
            }
        }

        /// <summary>
        /// Compare a file to a SHA1 checksum
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="checksum">SHA1 Checksum</param>
        /// <returns>Checksum matches</returns>
        public static bool IsSHA1Sum(string path, string checksum)
        {
            return GetSHA1Sum(path) == checksum;
        }
    }
}
