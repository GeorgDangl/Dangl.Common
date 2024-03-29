﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Dangl
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Will replace all linebreaks with <see cref="Environment.NewLine"/> and remove white spaces at line ends as well as any trailing white spaces.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Sanitize(this string source)
        {
            if (source == null) return null;
            var splitted = Regex.Split(source, "\r\n?|\n");
            var stringBuilder = new StringBuilder();
            foreach (var line in splitted)
            {
                stringBuilder.AppendLine(line.TrimEnd());
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
            var buffer = Encoding.UTF8.GetBytes(source);
            var ms = new MemoryStream();
            using (var zipStream = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zipStream.Write(buffer, 0, buffer.Length);
            }
            ms.Position = 0;
            var compressedBytes = new byte[ms.Length];
            ms.Read(compressedBytes, 0, compressedBytes.Length);
            var gzBuffer = new byte[compressedBytes.Length + 4];
            Buffer.BlockCopy(compressedBytes, 0, gzBuffer, 4, compressedBytes.Length);
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
            var gzBuffer = Convert.FromBase64String(souce);
            using (var ms = new MemoryStream())
            {
                var msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
                var buffer = new byte[msgLength];
                ms.Position = 0;
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    // For the actual reading, please see below:
                    // https://stackoverflow.com/questions/70933327/net-6-failing-at-decompress-large-gzip-text
                    // There was an issue discovered with a Dangl.AVA project in JSON format, which
                    // surfaced when tests that worked fine in .NET 5 failed with .NET 6. The errorenous
                    // decompression occured during an embedded image in base64 format.
                    int totalRead = 0;
                    while (totalRead < buffer.Length)
                    {
                        int bytesRead = zip.Read(buffer, totalRead, buffer.Length - totalRead);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        totalRead += bytesRead;
                    }
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }

        /// <summary>
        /// This returns null for null input, otherwise the original string up to a max length
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string WithMaxLength(this string value, int maxLength)
        {
            if (value == null || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength);
        }

        /// <summary>
        /// This returns null for null input, otherwise the original string with all linebreaks removed
        /// </summary>
        /// <param name="value"></param>
        public static string WithoutLinebreaks(this string value)
        {
            if (value == null)
            {
                return value;
            }

            return Regex.Replace(value, "[\r\n]", string.Empty);
        }

        /// <summary>
        /// This removes unprintable characters from the string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string WithoutUnprintableCharacters(this string value)
        {
            if (value == null)
            {
                return value;
            }

            var regexPatternNotPrintableAscii = "[\x00-\x08\x0B\x0C\x0E-\x1F\x81]";
            return Regex.Replace(value, regexPatternNotPrintableAscii, string.Empty);
        }
    }
}
