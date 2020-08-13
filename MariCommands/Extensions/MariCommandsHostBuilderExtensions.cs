using System;
using MariCommands.Builder;
using MariCommands.Factories;
using MariCommands.Hosting;
using MariCommands.Parsers;
using MariCommands.Providers;
using MariCommands.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace MariCommands.Extensions
{
    /// <summary>
    /// A class to add MariCommands in your ASP.NET Core project
    /// </summary>
    public static class MariCommandsHostBuilderExtensions
    {
        private static bool HasStartup { get; set; }

        /// <summary>
        /// Adds a command startup to configure your app in the current ASP.NET Core app.
        /// </summary>
        /// <param name="webBuilder">The current host builder.</param>
        /// <param name="addAllDefaultTypeParsers">If this lib wil inject all default type parsers to the dependency.</param>
        /// <param name="createNullables">If this lib will create nullables type parsers for these type parsers.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>        
        public static IWebHostBuilder UseCommandStartup<TStartup>(this IWebHostBuilder webBuilder, bool addAllDefaultTypeParsers = true, bool createNullables = true)
            where TStartup : class, ICommandStartup
        {
            CheckStartup();

            webBuilder.ConfigureServices(services =>
            {
                services.AddCommandStartup<TStartup>(addAllDefaultTypeParsers, createNullables);

                services.AddTransient<IStartupFilter, CommandStartupFilter>();
            });

            return webBuilder;
        }

        /// <summary>
        /// Adds a command startup to configure your app in the current ASP.NET Core app.
        /// OBS: this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandStartup{TStartup}(IWebHostBuilder, bool, bool)" /> instead this method.
        /// </summary>
        /// <remarks>
        /// this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandStartup{TStartup}(IWebHostBuilder, bool, bool)" /> instead this method.
        /// </remarks>
        /// <param name="hostBuilder">The current host builder.</param>
        /// <param name="addAllDefaultTypeParsers">If this lib wil inject all default type parsers to the dependency.</param>
        /// <param name="createNullables">If this lib will create nullables type parsers for these type parsers.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>
        public static IHostBuilder UseCommandStartup<TStartup>(this IHostBuilder hostBuilder, bool addAllDefaultTypeParsers = true, bool createNullables = true)
            where TStartup : class, ICommandStartup
        {
            CheckStartup();

            hostBuilder.ConfigureServices(services =>
            {
                services.AddCommandStartup<TStartup>(addAllDefaultTypeParsers, createNullables);

                services.AddHostedService<BackgroundStartupService>();
            });

            return hostBuilder;
        }

        private static IServiceCollection AddCommandStartup<TStartup>(this IServiceCollection services, bool addAllDefaultTypeParsers = true, bool createNullables = true)
            where TStartup : class, ICommandStartup
        {
            services.AddTransient<ICommandStartup, TStartup>();
            services.AddBasicMariCommandsServices(addAllDefaultTypeParsers, createNullables);

            return services;
        }

        private static void CheckStartup()
        {
            if (HasStartup)
                throw new InvalidOperationException("A startup has already been added to this project.");
        }
    }
}