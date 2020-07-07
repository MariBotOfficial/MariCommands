using System;
using System.Collections.Generic;
using MariCommands.Factories;
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

            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(LoggingUtils.HOSTING_CATEGORY_NAME);
            var service = provider.GetRequiredService<ICommandService>();
            var startup = provider.GetService<ICommandStartup>();

            if (startup.HasContent())
            {
                logger.LogDebug("Startup found.");
                logger.LogDebug($"Executing {nameof(ICommandStartup.ConfigureApp)}");

                startup.ConfigureApp(applicationBuilder);

                logger.LogDebug($"{nameof(ICommandStartup.ConfigureApp)} executed sucessfully.");
            }
            else
            {
                logger.LogDebug("No startup injected injecting default middlewares.");

                // TODO: inject default middlewares.

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