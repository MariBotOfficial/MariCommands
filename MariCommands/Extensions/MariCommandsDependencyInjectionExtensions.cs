using System;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            if (provider.HasNoContent())
                return new TDefault();

            return provider.GetService<T>() ?? new TDefault();
        }

        internal static T GetOrDefault<T, TDefault>(this IServiceProvider provider, TDefault defaultValue)
            where TDefault : T, new()
        {
            if (provider.HasNoContent())
                return defaultValue;

            return provider.GetService<T>() ?? defaultValue;
        }

        internal static IServiceProvider CreateDefaultServiceProvider()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILoggerFactory, LoggerFactory>();
            //TODO: collection.AddSingleton<IModuleBuilder, ModuleBuilder>();
            //TODO: collection.AddSingleton<IModuleSaver, ModuleSaver>();
            //TODO: collection.AddSingleton<ICommandExecutor, CommandExecutor();

            return collection.BuildServiceProvider();
        }
    }
}