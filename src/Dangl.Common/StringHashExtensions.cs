using System;
using System.Security.Cryptography;
using System.Text;

namespace Dangl
{
    /// <summary>
    /// Extension methods to produce hashes from strings
    /// </summary>
    public static class StringHashExtensions
    {
        /// <summary>
        /// Returns the MD5 hash of the string. Will return null for null input
        /// and string.Empty for empty strings. All-whitespace strings will be
        /// hashed normally.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToMd5(this string source)
        {
            if (source == null)
            {
                return null;
            }
            if (source.Length == 0)
            {
                return string.Empty;
            }

            using (var md5 = MD5.Create())
            {
                var textData = Encoding.UTF8.GetBytes(source);
                var hash = md5.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }

        /// <summary>
        /// Returns the SHA256 hash of the string. Will return null for null input
        /// and string.Empty for empty strings. All-whitespace strings will be
        /// hashed normally.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSha256(this string source)
        {
            if (source == null)
            {
                return null;
            }
            if (source.Length == 0)
            {
                return string.Empty;
            }

            using (var sha = SHA256.Create())
            {
                var textData = Encoding.UTF8.GetBytes(source);
                var hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}
