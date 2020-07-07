using System;
using System.Collections.Generic;
using MariCommands.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

            var service = provider.GetRequiredService<ICommandService>();
            var startup = provider.GetService<ICommandStartup>();
        }
    }
}