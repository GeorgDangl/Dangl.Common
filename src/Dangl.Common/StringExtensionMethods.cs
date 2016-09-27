using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Dangl
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Will replace all linebreaks with <see cref="Environment.NewLine"/> and remove white spaces as line ends as well as any trailing white spaces.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Sanitize(this string source)
        {
            if (source == null) return null;
            var splitted = Regex.Split(source, "\r\n?|\n");
            var stringBuilder = new StringBuilder();
            foreach (var currentLine in splitted)
            {
                stringBuilder.AppendLine(currentLine.TrimEnd());
            }
            return stringBuilder.ToString().TrimEnd();
        }

        /// <summary>
        /// Will return a Base64 representation of the string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToBase64(this string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Will return the plain text deoced from a Base64 string representation.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromBase64(this string source)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(source));
        }

        /// <summary>
        /// Returns the Base64 representation of the string after having applied GZip compression.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Compress(this string source)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        /// <summary>
        /// Decompresses a string from a Base64 GZip string.
        /// </summary>
        /// <param name="souce"></param>
        /// <returns></returns>
        public static string Decompress(this string souce)
        {
            byte[] gzBuffer = Convert.FromBase64String(souce);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
