using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dangl
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Will replace all linebreaks with <see cref="Environment.NewLine"/> and remove white spaces as line ends as well as any trailing white spaces.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Sanitize(this string Input)
        {
            if (Input == null) return null;
            var Splitted = Regex.Split(Input, "\r\n?|\n");
            var StringBuilder = new StringBuilder();
            foreach (var CurrentLine in Splitted)
            {
                StringBuilder.AppendLine(CurrentLine.TrimEnd());
            }
            return StringBuilder.ToString().TrimEnd();
        }

        /// <summary>
        /// Will return a Base64 representation of the string.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ToBase64(this string Input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Input));
        }

        /// <summary>
        /// Will return the plain text deoced from a Base64 string representation.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string FromBase64(this string Input)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(Input));
        }

        //TODO COMMENTS ALSO IN ENCRYPTION

        public static string Compress(this string Input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Input);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string Decompress(this string Input)
        {
            byte[] gzBuffer = Convert.FromBase64String(Input);
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
