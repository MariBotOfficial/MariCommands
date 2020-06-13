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
                throw new ArgumentNullException(argName);
        }

        public static void NotNullOrEmpty<T>(this IEnumerable<T> collection, string argName)
        {
            if (collection.HasNoContent())
                throw new ArgumentNullException(argName);
        }

        public static void NotNull(this object obj, string argName)
        {
            if (obj.HasNoContent())
                throw new ArgumentNullException(argName);
        }

        public static void NotNullOrWhiteSpace(this string str, string argName)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(argName);
        }
    }
}