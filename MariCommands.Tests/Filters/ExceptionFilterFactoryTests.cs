using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands.Filters;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class ExceptionFilterFactoryTests
    {
        [Fact]
        public async Task Should_Build_Filters_When_Call_GetFiltersDelegate()
        {
            // Act
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(TestExceptionFilter));
            });

            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<ExceptionFilterFactory>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var exceptionFilterFactory = provider.GetRequiredService<ExceptionFilterFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var edi = ExceptionDispatchInfo.Capture(new Exception());

            var exceptionContext = new CommandExceptionContext(context, edi);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            var exceptionDelegate = exceptionFilterFactory.GetFiltersDelegate();

            Assert.NotNull(exceptionDelegate);

            await exceptionDelegate(exceptionContext);

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
                options.Filters.Add(typeof(TestExceptionFilter));
            });

            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<ExceptionFilterFactory>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var exceptionFilterFactory = provider.GetRequiredService<ExceptionFilterFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var edi = ExceptionDispatchInfo.Capture(new Exception());

            var exceptionContext = new CommandExceptionContext(context, edi);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            var exceptionDelegate = exceptionFilterFactory.GetFiltersDelegate();

            Assert.NotNull(exceptionDelegate);

            await exceptionDelegate(exceptionContext);

            Assert.True(singletonDependency.Invoked);

            singletonDependency.Invoked = false;

            var filters = Array.Empty<ICommandFilter>();

            exceptionFilterFactory.FiltersDefinitionWasChanged(filters);

            exceptionDelegate = exceptionFilterFactory.GetFiltersDelegate();

            await exceptionDelegate(exceptionContext);

            Assert.False(singletonDependency.Invoked);
        }

        private class TestExceptionFilter : ICommandExceptionFilter
        {
            private readonly SingletonDependency _singletonDependency;

            public TestExceptionFilter(SingletonDependency singletonDependency)
            {
                _singletonDependency = singletonDependency;
            }

            public Task InvokeAsync(CommandExceptionContext context, CommandExceptionDelegate next)
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