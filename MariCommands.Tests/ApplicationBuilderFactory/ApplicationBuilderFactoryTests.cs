using System.Collections.Generic;
using MariCommands.Extensions;
using MariCommands.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.ApplicationBuilderFactory
{
    public class ApplicationBuilderFactoryTests
    {
        [Fact]
        public void CanGetFromDefaultDependencies()
        {
            var services = new ServiceCollection();

            services.AddBasicMariCommandsServices();

            var provider = services.BuildServiceProvider(true);

            var factory = provider.GetRequiredService<ICommandApplicationBuilderFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void CanCreateCommandApplication()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var factory = new CommandApplicationBuilderFactory();

            var app = factory.Create(new Dictionary<string, object>(), provider);

            Assert.NotNull(app);
        }
    }
}