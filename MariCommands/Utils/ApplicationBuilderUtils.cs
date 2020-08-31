using System;
using System.Collections.Generic;
using System.Reflection;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Factories;
using MariCommands.Hosting;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands.Utils
{
    internal static class ApplicationBuilderUtils
    {
        private static bool AlreadyStarted { get; set; }

        internal static void InitializeCommandApp(IServiceProvider appProvider)
        {
            if (AlreadyStarted)
                throw new InvalidOperationException("The command application has already been started.");

            var applicationBuilderFactory = appProvider.GetRequiredService<ICommandApplicationBuilderFactory>();

            var applicationBuilder = applicationBuilderFactory.Create(new Dictionary<string, object>(), appProvider);
            var provider = applicationBuilder.ApplicationServices;

            var configurer = provider.GetRequiredService<IModuleConfigurer>();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(LoggingUtils.HOSTING_CATEGORY_NAME);
            var service = provider.GetRequiredService<ICommandService>();
            var startup = provider.GetService<ICommandStartup>();

            if (startup.HasContent())
            {
                logger.LogDebug("Startup found.");

                logger.LogDebug($"Executing {nameof(ICommandStartup.ConfigureModules)}");

                startup.ConfigureModules(configurer);

                logger.LogDebug($"{nameof(ICommandStartup.ConfigureModules)} executed sucessfully.");

                logger.LogDebug($"Executing {nameof(ICommandStartup.ConfigureApp)}");

                startup.ConfigureApp(applicationBuilder);

                logger.LogDebug($"{nameof(ICommandStartup.ConfigureApp)} executed sucessfully.");
            }
            else
            {
                var config = provider.GetRequiredService<ICommandServiceOptions>();

                if (config.AutoAddRunningAssembly)
                {
                    logger.LogDebug($"{nameof(ICommandServiceOptions.AutoAddRunningAssembly)}" +
                    "is setted to true the lib will auto inject all modules in this running assembly.");

                    configurer.AddModules(Assembly.GetExecutingAssembly());
                }

                logger.LogDebug("No startup injected, the lib will inject the default middlewares.");

                applicationBuilder.UseDefaultMiddlewares();

                logger.LogDebug("Successfully injected default middlewares.");
            }

            logger.LogDebug("Building the application...");

            var app = applicationBuilder.Build();

            logger.LogDebug("Successfully builded the application.");

            logger.LogDebug("Command application is starting.");

            service.Initialize(app);

            logger.LogInformation("Command application is ready.");
        }

        public static ICommandApplicationBuilder UseDefaultMiddlewares(this ICommandApplicationBuilder app)
        {
            app.UseDefaultExceptionMiddleware();
            app.UseStringMatcher();
            app.UseInputCountMatcher();
            app.UseParser();
            app.UseParamPreconditioner();
            app.UsePreconditioner();
            app.UseCommandExecutor();

            return app;
        }
    }
}