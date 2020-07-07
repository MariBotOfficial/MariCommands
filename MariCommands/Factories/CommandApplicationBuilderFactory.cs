using System;
using System.Collections.Generic;
using MariCommands.Builder;

namespace MariCommands.Factories
{
    internal sealed class CommandApplicationBuilderFactory : ICommandApplicationBuilderFactory
    {
        public ICommandApplicationBuilder Create(IDictionary<string, object> properties, IServiceProvider provider)
            => new CommandApplicationBuilder(properties, provider);
    }
}