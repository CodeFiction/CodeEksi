using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Services.Utils
{
    internal static class StringExtensions
    {
        //private static readonly Regex UrlValidationRegex =
        //    new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&\/\/=]*)");
        //private static readonly Regex BkzRegex = new Regex(@"\(bkz: [\w ]*\)");
        private static readonly Regex LinkRegex = new Regex("\\/\\?q=[\\w%+']*", RegexOptions.CultureInvariant);
        private static readonly Regex BkzRegex = new Regex("\\/entry\\/[0-9]*", RegexOptions.CultureInvariant);

        public static string JoinToString(this List<string> items, string delimeter)
        {
            return string.Join(delimeter, items.ToArray());
        }

        public static string FixLinks(this string item)
        {
            //var urlMatches = UrlValidationRegex.Matches(item);
            //var bkzMatches = BkzRegex.Matches(item);
            var linkMatches = LinkRegex.Matches(item);
            var bkzMatches = BkzRegex.Matches(item);

            foreach (Match linkMatch in linkMatches)
            {
                item = item.Replace(linkMatch.Value, $"#title/{linkMatch.Value.Replace("/?q=", "").Replace("+", " ")}");
            }

            foreach (Match bkzMatch in bkzMatches)
            {
                item = item.Replace(bkzMatch.Value, $"#entry/{bkzMatch.Value.Replace("/entry/", "")}");
            }
            return item;
        }
    }
}
