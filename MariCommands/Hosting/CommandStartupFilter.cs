using System;
using System.Collections.Generic;
using System.Reflection;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Factories;
using MariCommands.Middlewares;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands.Hosting
{
    internal sealed class CommandStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                ConfigureInternal(builder);
                next(builder);
            };
        }

        private void ConfigureInternal(IApplicationBuilder builder)
        {
            var applicationBuilderFactory = builder.ApplicationServices.GetRequiredService<ICommandApplicationBuilderFactory>();

            var applicationBuilder = applicationBuilderFactory.Create(new Dictionary<string, object>(), builder.ApplicationServices);
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

                applicationBuilder.UseDefaultExceptionMiddleware();
                applicationBuilder.UseStringMatcher();
                applicationBuilder.UseInputCountMatcher();
                applicationBuilder.UseParser();
                applicationBuilder.UseParamPreconditioner();

                logger.LogDebug("Successfully injected default middlewares.");
            }

            logger.LogDebug("Building the application...");

            var app = applicationBuilder.Build();

            logger.LogDebug("Successfully builded the application.");

            logger.LogDebug("Command application is starting.");

            service.Initialize(app);

            logger.LogInformation("Command application is ready.");
        }
    }
}