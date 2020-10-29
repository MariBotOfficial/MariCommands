using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<Attribute> GetAllAttributes(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(true)
                                .Select(a => (Attribute)a)
                                .OrderBy(a => ((Type)a.TypeId).MetadataToken)
                                .ToList();
        }

        public static IEnumerable<Attribute> GetAllAttributes(this ParameterInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(true)
                                .Select(a => (Attribute)a)
                                .ToList();
        }
    }
}