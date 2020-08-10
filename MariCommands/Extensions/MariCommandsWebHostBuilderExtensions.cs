using System;
using MariCommands.Builder;
using MariCommands.Factories;
using MariCommands.Hosting;
using MariCommands.Parsers;
using MariCommands.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MariCommands.Extensions
{
    /// <summary>
    /// A class to add MariCommands in your ASP.NET Core project
    /// </summary>
    public static class MariCommandsWebHostBuilderExtensions
    {
        private static bool HasStartup { get; set; }

        /// <summary>
        /// Adds a command startup to configure your app in the current ASP.NET Core app.
        /// </summary>
        /// <param name="webBuilder">The current host builder.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>        
        public static IWebHostBuilder UseCommandStartup<TStartup>(this IWebHostBuilder webBuilder)
            where TStartup : class, ICommandStartup
        {
            if (HasStartup)
                throw new InvalidOperationException("A startup has already been added to this project.");

            webBuilder.ConfigureServices(services =>
            {
                services.AddTransient<ICommandStartup, TStartup>();
                services.AddTransient<IStartupFilter, CommandStartupFilter>();

                services.TryAddSingleton<ICommandServiceOptions, CommandServiceOptions>();
                services.TryAddSingleton<IContextExecutor, ContextExecutor>();
                services.TryAddSingleton<IModuleCache, ModuleCache>();

                services.TryAddTransient<ICommandService, CommandService>();
                services.TryAddTransient<ICommandApplicationBuilderFactory, CommandApplicationBuilderFactory>();
                services.TryAddTransient<IModuleConfigurer, ModuleConfigurer>();
                services.TryAddTransient<IModuleFactory, ModuleFactory>();
                services.TryAddTransient<ICommandFactory, CommandFactory>();
                services.TryAddTransient<IParameterFactory, ParameterFactory>();
                services.TryAddTransient<IArgumentParser, ArgumentParser>();
                services.TryAddTransient<ITypeParserProvider, TypeParserProvider>();
            });

            return webBuilder;
        }
    }
}