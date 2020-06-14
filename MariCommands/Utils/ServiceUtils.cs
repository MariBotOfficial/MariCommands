using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    internal static class ServiceUtils
    {
        public readonly static IServiceProvider Instance;

        static ServiceUtils()
        {
            Instance = CreateDefaultServiceProvider();
        }

        private static IServiceProvider CreateDefaultServiceProvider()
        {
            var collection = new ServiceCollection();

            // Don't inject ICommandService here.

            collection.AddSingleton<ICommandServiceOptions, CommandServiceOptions>();
            collection.AddSingleton<ILoggerFactory, LoggerFactory>();
            collection.AddSingleton<IModuleFactory, ModuleFactory>();
            collection.AddSingleton<ICommandFactory, CommandFactory>();
            //TODO: collection.AddSingleton<ICommandCache, CommandCache>();
            //TODO: collection.AddSingleton<ICommandExecutor, CommandExecutor>();

            return collection.BuildServiceProvider();
        }
    }
}