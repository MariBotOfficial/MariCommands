using System;
using System.Threading.Tasks;
using MariCommands.Filters;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class ResultFilterFactoryTests
    {
        [Fact]
        public async Task Should_Build_Filters_When_Call_GetFiltersDelegate()
        {
            // Act
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(TestResultFilter));
            });

            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<ResultFilterFactory>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var resultFilterFactory = provider.GetRequiredService<ResultFilterFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var result = new SuccessResult();

            var resultContext = new CommandResultContext(context, result);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            var resultDelegate = resultFilterFactory.GetFiltersDelegate();

            Assert.NotNull(resultDelegate);

            await resultDelegate(resultContext);

            Assert.True(singletonDependency.Invoked);
        }

        [Fact]
        public async Task FiltersDefinitionWasChanged_Should_Rebuild_Delegates()
        {
            // Act
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(TestResultFilter));
            });

            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<ResultFilterFactory>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var resultFilterFactory = provider.GetRequiredService<ResultFilterFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var result = new SuccessResult();

            var resultContext = new CommandResultContext(context, result);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            var resultDelegate = resultFilterFactory.GetFiltersDelegate();

            Assert.NotNull(resultDelegate);

            await resultDelegate(resultContext);

            Assert.True(singletonDependency.Invoked);

            singletonDependency.Invoked = false;

            var filters = Array.Empty<ICommandFilter>();

            resultFilterFactory.FiltersDefinitionWasChanged(filters);

            resultDelegate = resultFilterFactory.GetFiltersDelegate();

            await resultDelegate(resultContext);

            Assert.False(singletonDependency.Invoked);
        }

        private class TestResultFilter : ICommandResultFilter
        {
            private readonly SingletonDependency _singletonDependency;

            public TestResultFilter(SingletonDependency singletonDependency)
            {
                _singletonDependency = singletonDependency;
            }

            public Task InvokeAsync(CommandResultContext context, CommandResultDelegate next)
            {
                _singletonDependency.Invoked = true;

                return next(context);
            }
        }

        private class SingletonDependency
        {
            public bool Invoked { get; set; }
        }
    }
}