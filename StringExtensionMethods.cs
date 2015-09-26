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
            var ReturnString = string.Empty;
            for (int i = 0; i < Splitted.Length; i++)
            {
                ReturnString += Splitted[i].TrimEnd() + Environment.NewLine;
            }
            return ReturnString.TrimEnd();
        }
    }
}
