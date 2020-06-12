using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    internal static class ServiceUtils
    {
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