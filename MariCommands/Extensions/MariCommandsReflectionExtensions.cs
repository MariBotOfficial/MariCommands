using System;
using System.Reflection;

namespace MariCommands
{
    internal static class MariCommandsReflectionExtensions
    {
        public static T GetAttribute<T>(this Type type)
            where T : Attribute
        {
            return type.GetCustomAttribute(typeof(T)) as T;
        }
    }
}