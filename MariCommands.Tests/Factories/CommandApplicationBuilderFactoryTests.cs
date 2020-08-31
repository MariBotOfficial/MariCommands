using System.Collections.Generic;
using MariCommands.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Factories
{
    public class CommandApplicationBuilderFactoryTests
    {
        [Fact]
        public void CanCreateCommandApplicationBuilder()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var factory = new CommandApplicationBuilderFactory();

            var app = factory.Create(new Dictionary<string, object>(), provider);

            Assert.NotNull(app);
        }
    }
}