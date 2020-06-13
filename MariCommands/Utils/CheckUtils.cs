using System;
using System.Collections.Generic;
using MariGlobals.Extensions;

namespace MariCommands
{
    internal static class CheckUtils
    {
        public static void NotNullOrEmpty<T>(this T[] arr, string argName)
        {
            if (arr.HasNoContent())
                ThrowNullOrEmpty(argName);
        }

        public static void NotNullOrEmpty<T>(this IEnumerable<T> collection, string argName)
        {
            if (collection.HasNoContent())
                ThrowNullOrEmpty(argName);
        }

        private static void ThrowNullOrEmpty(string argName)
        {
            throw new ArgumentNullException($"{argName} must not be null or empty", argName);
        }

        public static void NotNull(this object obj, string argName)
        {
            if (obj.HasNoContent())
                throw new ArgumentNullException($"{argName} must not be null.", argName);
        }

        public static void NotNullOrWhiteSpace(this string str, string argName)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException($"{argName} must not be null or white space.", argName);
        }
    }
}