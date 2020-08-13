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
            => builder.ApplicationServices.InitializeCommandApp();
    }
}