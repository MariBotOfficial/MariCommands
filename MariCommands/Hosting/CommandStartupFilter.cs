using System;
using MariCommands.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

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
            => ApplicationBuilderUtils.InitializeCommandApp(builder.ApplicationServices);
    }
}