using System;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <summary>
    /// General extensions for dependency injection.
    /// </summary>
    public static class MariCommandsDependencyInjectionExtensions
    {
        internal static T GetOrDefault<T, TDefault>(this IServiceProvider provider)
            where TDefault : T, new()
        {
            return provider.GetService<T>() ?? new TDefault();
        }
    }
}