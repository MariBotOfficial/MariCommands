using System;
using MariCommands.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class TypeCommandFilterFactoryTests
    {
        [Fact]
        public void CreateInstance_Should_Create_Filter_Instance_Without_Filter_Injected()
        {
            // Act
            var provider = new ServiceCollection()
                                .AddSingleton<SomeDependencyTest>()
                                .BuildServiceProvider(true);

            var factory = new TypeCommandFilterFactory(typeof(TestFilter));

            // Arrange
            var instance = factory.CreateInstance(provider);

            // Assert
            Assert.NotNull(instance);
            Assert.IsType<TestFilter>(instance);
        }

        [Fact]
        public void CreateInstance_Should_Create_Filter_Instance_With_FilterFactory()
        {
            // Act
            var provider = new ServiceCollection()
                                .AddSingleton<SomeDependencyTest>()
                                .BuildServiceProvider(true);

            var factory = new TypeCommandFilterFactory(typeof(TestFilterFactory));

            // Arrange
            var instance = factory.CreateInstance(provider);

            // Assert
            Assert.NotNull(instance);
            Assert.IsType<TestFilter>(instance);
        }

        private class TestFilter : ICommandFilter
        {
#pragma warning disable IDE0052
            private readonly SomeDependencyTest _someDependency;
#pragma warning disable IDE0052

            public TestFilter(SomeDependencyTest someDependency)
            {
                _someDependency = someDependency;
            }
        }

        private class TestFilterFactory : ICommandFilterFactory
        {
#pragma warning disable IDE0052
            private readonly SomeDependencyTest _someDependency;
#pragma warning disable IDE0052

            public TestFilterFactory(SomeDependencyTest someDependency)
            {
                _someDependency = someDependency;
            }

            public Type ImplementationType => typeof(TestFilter);

            public ICommandFilter CreateInstance(IServiceProvider serviceProvider)
                => new TestFilter(serviceProvider.GetRequiredService<SomeDependencyTest>());
        }

        private class SomeDependencyTest
        {

        }
    }
}