using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Filters;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class FilterProviderTests
    {
        [Fact]
        public async Task InvokeFiltersAsync_Should_Invoke_ResultFilterFactory_Builded_Delegate()
        {
            // Act
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(TestResultFilter));
            });

            services.AddFilterFactory<ResultFilterFactory, ICommandResultFilter, CommandResultDelegate>();
            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<FilterProvider>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var filterProvider = provider.GetRequiredService<FilterProvider>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var result = new SuccessResult();
            var resultContext = new CommandResultContext(context, result);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            await filterProvider.InvokeFiltersAsync<CommandResultContext, ICommandResultFilter>(resultContext);

            Assert.True(singletonDependency.Invoked);
        }

        [Fact]
        public async Task InvokeFiltersAsync_Should_Invoke_ExceptionFilterFactory_Builded_Delegate()
        {
            // Act
            var services = new ServiceCollection();

            services.AddOptions<MariCommandsOptions>();

            services.Configure<MariCommandsOptions>(options =>
            {
                options.Filters.Add(typeof(TestExceptionFilter));
            });

            services.AddFilterFactory<ExceptionFilterFactory, ICommandExceptionFilter, CommandExceptionDelegate>();
            services.AddSingleton<SingletonDependency>();
            services.AddSingleton<FilterProvider>();

            var provider = services.BuildServiceProvider(true);
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var filterProvider = provider.GetRequiredService<FilterProvider>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory,
            };

            var edi = ExceptionDispatchInfo.Capture(new Exception());
            var exceptionContext = new CommandExceptionContext(context, edi);

            var singletonDependency = provider.GetRequiredService<SingletonDependency>();

            // Arrange + Assert
            await filterProvider.InvokeFiltersAsync<CommandExceptionContext, ICommandExceptionFilter>(exceptionContext);

            Assert.True(singletonDependency.Invoked);
        }

        [Fact]
        public void FiltersChanged_Should_Notify_Correct_Filter_Factories_When_Add()
        {
            // Act
            var rebuilded = false;
            var options = new MariCommandsOptions();

            options.Filters.Add(typeof(TestResultFilter));

            var filterFactoryMock = new Mock<IFilterFactory>();

            filterFactoryMock.Setup(a => a.CanInvoke(typeof(TestResultFilter))).Returns(true);

            filterFactoryMock.Setup(a => a.FiltersDefinitionWasChanged(options.Filters)).Callback<IEnumerable<ICommandFilter>>(filters =>
            {
                rebuilded = filters.Equals(options.Filters);
            });

            var filterFactory = filterFactoryMock.Object;

            var filtersFactories = new List<IFilterFactory>
            {
                filterFactory,
            };

            var filterProvider = new FilterProvider(filtersFactories, options);

            // Arrange
            options.Filters.Add(typeof(TestResultFilter));

            // Assert
            Assert.True(rebuilded);
        }

        [Fact]
        public void FiltersChanged_Should_Notify_Correct_Filter_Factories_When_Remove()
        {
            // Act
            var rebuilded = false;
            var options = new MariCommandsOptions();

            var filter = new TypeCommandFilterFactory(typeof(TestResultFilter));

            options.Filters.Add(filter);

            var filterFactoryMock = new Mock<IFilterFactory>();

            filterFactoryMock.Setup(a => a.CanInvoke(typeof(TestResultFilter))).Returns(true);

            filterFactoryMock.Setup(a => a.FiltersDefinitionWasChanged(options.Filters)).Callback<IEnumerable<ICommandFilter>>(filters =>
            {
                rebuilded = filters.Equals(options.Filters);
            });

            var filterFactory = filterFactoryMock.Object;

            var filtersFactories = new List<IFilterFactory>
            {
                filterFactory,
            };

            var filterProvider = new FilterProvider(filtersFactories, options);

            // Arrange
            options.Filters.Remove(filter);

            // Assert
            Assert.True(rebuilded);
        }

        [Fact]
        public void FiltersChanged_Should_Notify_Correct_Filter_Factories_When_Reset()
        {
            // Act
            var rebuilded = false;
            var options = new MariCommandsOptions();

            options.Filters.Add(typeof(TestResultFilter));

            var filterFactoryMock = new Mock<IFilterFactory>();

            filterFactoryMock.Setup(a => a.CanInvoke(typeof(TestResultFilter))).Returns(true);

            filterFactoryMock.Setup(a => a.FiltersDefinitionWasChanged(options.Filters)).Callback<IEnumerable<ICommandFilter>>(filters =>
            {
                rebuilded = filters.Equals(options.Filters);
            });

            var filterFactory = filterFactoryMock.Object;

            var filtersFactories = new List<IFilterFactory>
            {
                filterFactory,
            };

            var filterProvider = new FilterProvider(filtersFactories, options);

            // Arrange
            options.Filters.Clear();

            // Assert
            Assert.True(rebuilded);
        }

        [Fact]
        public void FiltersChanged_Should_Notify_Correct_Filter_Factories_When_Replace()
        {
            // Act
            var callCount = 0;
            var expectedCallCount = 2;
            var options = new MariCommandsOptions();

            options.Filters.Add(typeof(TestResultFilter));

            var filterFactoryMock = new Mock<IFilterFactory>();

            filterFactoryMock.Setup(a => a.CanInvoke(typeof(TestResultFilter))).Returns(true);

            filterFactoryMock.Setup(a => a.FiltersDefinitionWasChanged(options.Filters)).Callback<IEnumerable<ICommandFilter>>(filters =>
            {
                if (filters.Equals(options.Filters))
                    callCount++;
            });

            var filterFactory = filterFactoryMock.Object;

            var filtersFactories = new List<IFilterFactory>
            {
                filterFactory,
            };

            var filterProvider = new FilterProvider(filtersFactories, options);

            // Arrange
            options.Filters[0] = new ServiceCommandFilterFactory(typeof(TestResultFilter));

            // Assert
            Assert.Equal(expectedCallCount, callCount);
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