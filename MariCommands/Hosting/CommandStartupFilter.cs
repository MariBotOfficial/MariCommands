using System;
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
            var service = builder.ApplicationServices.GetRequiredService<ICommandService>();
            var startup = builder.ApplicationServices.GetRequiredService<ICommandStartup>();
        }
    }
}