using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services.Utils
{
    internal static class StringExtensions
    {
        public static string JoinToString(this List<string> items, string delimeter)
        {
            return string.Join(delimeter, items.ToArray());
        }
    }
}
