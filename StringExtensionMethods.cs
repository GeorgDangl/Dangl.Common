using System;
using System.Collections.Generic;
using System.Linq;
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
            return Splitted.Aggregate(string.Empty, (current, t) => current + (t.TrimEnd() + Environment.NewLine)).TrimEnd();
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
