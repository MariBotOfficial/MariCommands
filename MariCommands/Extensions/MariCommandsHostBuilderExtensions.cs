using System;
using MariCommands.Builder;
using MariCommands.Factories;
using MariCommands.Hosting;
using MariCommands.Parsers;
using MariCommands.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
        /// <param name="configureOptions">The configure options for this lib.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>        
        public static IWebHostBuilder UseCommandStartup<TStartup>(this IWebHostBuilder webBuilder, Action<MariCommandsOptions> configureOptions = null)
            where TStartup : class, ICommandStartup
        {
            CheckStartup();

            webBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<BackgroundStartupService>();

                services.AddCommandStartup<TStartup>(configureOptions);
            });

            return webBuilder;
        }

        /// <summary>
        /// Adds a command service startup to configure your app in the current ASP.NET Core app.
        /// </summary>
        /// <param name="webBuilder">The current host builder.</param>
        /// <param name="configureOptions">The configure options for this lib.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns> 
        public static IWebHostBuilder UseCommandServiceStartup<TStartup>(this IWebHostBuilder webBuilder, Action<MariCommandsOptions> configureOptions = null)
            where TStartup : class, ICommandServiceStartup
        {
            CheckStartup();

            webBuilder.ConfigureServices((context, services) =>
            {
                services.AddHostedService<BackgroundStartupService>();

                services.AddCommandServiceStartup<TStartup>(context.Configuration, context.HostingEnvironment, configureOptions);
            });

            return webBuilder;
        }

        /// <summary>
        /// Adds a command startup to configure your app in the current ASP.NET Core app.
        /// OBS: this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandStartup{TStartup}(IWebHostBuilder, Action{MariCommandsOptions})" /> instead this method.
        /// </summary>
        /// <remarks>
        /// this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandStartup{TStartup}(IWebHostBuilder, Action{MariCommandsOptions})" /> instead this method.
        /// </remarks>
        /// <param name="hostBuilder">The current host builder.</param>
        /// <param name="configureOptions">The configure options for this lib.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>
        public static IHostBuilder UseCommandStartup<TStartup>(this IHostBuilder hostBuilder, Action<MariCommandsOptions> configureOptions = null)
            where TStartup : class, ICommandStartup
        {
            CheckStartup();

            hostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<BackgroundStartupService>();

                services.AddCommandStartup<TStartup>(configureOptions);
            });

            return hostBuilder;
        }

        /// <summary>
        /// Adds a command startup to configure your app in the current ASP.NET Core app.
        /// OBS: this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandServiceStartup{TStartup}(IWebHostBuilder, Action{MariCommandsOptions})" /> instead this method.
        /// </summary>
        /// <remarks>
        /// this method is for non Web ASP.NET Core apps if you using a Web ASP.NET Core
        /// app use <see cref="UseCommandServiceStartup{TStartup}(IWebHostBuilder, Action{MariCommandsOptions})" /> instead this method.
        /// </remarks>
        /// <param name="hostBuilder">The current host builder.</param>
        /// <param name="configureOptions">The configure options for this lib.</param>
        /// <typeparam name="TStartup">The startup to be added.</typeparam>
        /// <returns>The current host builder.</returns>
        public static IHostBuilder UseCommandServiceStartup<TStartup>(this IHostBuilder hostBuilder, Action<MariCommandsOptions> configureOptions = null)
            where TStartup : class, ICommandServiceStartup
        {
            CheckStartup();

            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddHostedService<BackgroundStartupService>();

                services.AddCommandServiceStartup<TStartup>(context.Configuration, context.HostingEnvironment, configureOptions);
            });

            return hostBuilder;
        }

        private static IServiceCollection AddCommandServiceStartup<TStartup>(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            Action<MariCommandsOptions> configureOptions)
                where TStartup : class, ICommandServiceStartup
        {
            var hostBuilderProvider = new HostBuilderServiceProvider(configuration, hostEnvironment);

            var commandStartup = ActivatorUtilities.CreateInstance(hostBuilderProvider, typeof(TStartup)) as ICommandServiceStartup;

            commandStartup.ConfigureServices(services);

            services.AddCommandStartup<TStartup>(configureOptions);

            return services;
        }

        private static IServiceCollection AddCommandStartup<TStartup>(this IServiceCollection services, Action<MariCommandsOptions> configureOptions)
            where TStartup : class, ICommandStartup
        {
            services.AddTransient<ICommandStartup, TStartup>();
            services.AddBasicMariCommandsServices(configureOptions);

            return services;
        }

        private static void CheckStartup()
        {
            if (HasStartup)
                throw new InvalidOperationException("A startup has already been added to this project.");

            HasStartup = true;
        }

        // Clear this class for unit tests.
        internal static void Clear()
        {
            HasStartup = false;
        }
    }
}