using System;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    internal static class ServiceUtils
    {
        private static IServiceProvider Instance { get; set; }

        static ServiceUtils()
        {
            Instance = CreateDefaultServiceProvider();
        }

        internal static IServiceProvider GetDefaultServiceProvider()
        {
            if (Instance.HasNoContent())
                Instance = CreateDefaultServiceProvider();

            return Instance;
        }

        private static IServiceProvider CreateDefaultServiceProvider()
        {
            return new ServiceCollection()
                                    .TryAddDefaultServices()
                                    .BuildServiceProvider(false);
        }

        internal static IServiceCollection TryAddDefaultServices(this IServiceCollection collection, bool injectCommandService = false)
        {
            if (injectCommandService)
                collection.TryAddSingleton<ICommandService, CommandService>();

            collection.TryAddSingleton<ICommandServiceOptions, CommandServiceOptions>();
            collection.TryAddSingleton<ILoggerFactory, LoggerFactory>();
            collection.TryAddSingleton<IModuleFactory, ModuleFactory>();
            collection.TryAddSingleton<ICommandFactory, CommandFactory>();
            collection.TryAddSingleton<IParameterFactory, ParameterFactory>();
            collection.TryAddSingleton<IModuleCache, ModuleCache>();
            collection.TryAddSingleton<IContextExecutor, ContextExecutor>();
            collection.TryAddSingleton<IArgumentParser, ArgumentParser>();

            return collection;
        }
    }
}