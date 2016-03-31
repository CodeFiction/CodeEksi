using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Server.Services.Utils;

namespace Server.Services.Aspects
{
    internal static class CacheKeyHelper
    {
        public static string GetCacheKey(MethodInfo methodInfo, object[] arguments)
        {
            if (arguments.IsNullOrEmptyOrAllItemNull())
            {
                return $"{methodInfo.ReflectedType.FullName}.{methodInfo.Name}";
            }

            string args = arguments.Select(GetHash).ToList().JoinToString(":");

            return $"{methodInfo.ReflectedType.FullName}.{methodInfo.Name}_{args}";
        }

        private static string GetHash(object argument)
        {
            if (argument == null)
            {
                return "null";
            }

            Type type = argument.GetType();

            if (type == typeof(string))
            {
                return $"str_{argument}";
            }

            if (!type.IsClass || type.IsPrimitive)
            {
                return argument.ToString();
            }

            string hash;
            var binaryFormatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, argument);
                byte[] array = stream.ToArray();

                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    hash = Convert.ToBase64String(sha1.ComputeHash(array));
                }
            }

            return hash;
        }
    }
}
