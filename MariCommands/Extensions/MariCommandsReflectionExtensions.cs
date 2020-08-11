using System;
using System.Reflection;

namespace MariCommands.Extensions
{
    internal static class MariCommandsReflectionExtensions
    {
        public static T GetAttribute<T>(this MemberInfo memberInfo)
            where T : Attribute
        {
            return memberInfo.GetCustomAttribute(typeof(T)) as T;
        }

        public static T GetAttribute<T>(this ParameterInfo parameterInfo)
            where T : Attribute
        {
            return parameterInfo.GetCustomAttribute(typeof(T)) as T;
        }
    }
}