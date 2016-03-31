using System.Collections.Generic;
using System.Linq;

namespace Server.Services.Utils
{
    internal static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }

        public static bool IsNullOrEmptyOrAnyItemNull<T>(this IEnumerable<T> enumerable)
            where T : class
        {
            var list = enumerable.ToList();

            return list.IsNullOrEmpty() || list.Any(arg => arg == null);
        }

        public static bool IsNullOrEmptyOrAllItemNull<T>(this IEnumerable<T> enumerable)
            where T : class
        {
            var list = enumerable.ToList();

            return list.IsNullOrEmpty() || list.All(arg => arg == null);
        }
    }
}