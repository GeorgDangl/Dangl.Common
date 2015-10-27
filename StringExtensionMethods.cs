using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
