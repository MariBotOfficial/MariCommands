using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Services
{
    public class ScopedCommandContextAccessorTests
    {
        [Fact]
        public void GetFromScoped()
        {
            var services = new ServiceCollection();

            services.AddScoped<ICommandContextAccessor, ScopedCommandContextAccessor>();

            using var provider = services.BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var commandContextAccessor = scope.ServiceProvider.GetRequiredService<ICommandContextAccessor>();

            Assert.NotNull(commandContextAccessor);
            Assert.Null(commandContextAccessor.CommandContext);
        }

        [Fact]
        public void GetFromScopedDependency()
        {
            var services = new ServiceCollection();

            services.AddScoped<ICommandContextAccessor, ScopedCommandContextAccessor>();
            services.AddScoped<TestDependency>();

            using var provider = services.BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var commandContextAccessor = scope.ServiceProvider.GetRequiredService<ICommandContextAccessor>();

            var context = new CommandContext();

            commandContextAccessor.CommandContext = context;

            var testDependency = scope.ServiceProvider.GetRequiredService<TestDependency>();

            Assert.NotNull(commandContextAccessor);
            Assert.NotNull(commandContextAccessor.CommandContext);
            Assert.NotNull(testDependency);
            Assert.NotNull(testDependency.CommandContext);
            Assert.Equal(context, commandContextAccessor.CommandContext);
            Assert.Equal(context, testDependency.CommandContext);
        }

        [Fact]
        public void GetFromTransientDependency()
        {
            var services = new ServiceCollection();

            services.AddScoped<ICommandContextAccessor, ScopedCommandContextAccessor>();
            services.AddTransient<TestDependency>();

            using var provider = services.BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var commandContextAccessor = scope.ServiceProvider.GetRequiredService<ICommandContextAccessor>();

            var context = new CommandContext();

            commandContextAccessor.CommandContext = context;

            var testDependency = scope.ServiceProvider.GetRequiredService<TestDependency>();

            Assert.NotNull(commandContextAccessor);
            Assert.NotNull(commandContextAccessor.CommandContext);
            Assert.NotNull(testDependency);
            Assert.NotNull(testDependency.CommandContext);
            Assert.Equal(context, commandContextAccessor.CommandContext);
            Assert.Equal(context, testDependency.CommandContext);
        }

        [Fact]
        public async Task AlwaysInitializedWhenRunContext()
        {
            var services = new ServiceCollection();

            services.AddScoped<ICommandContextAccessor, ScopedCommandContextAccessor>();
            services.AddScoped<TestDependency>();

            using var provider = services.BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();

            var context = new CommandContext();

            context.ServiceScopeFactory = scopeFactory;

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.Use((next) =>
            {
                return (context) =>
                {
                    return Task.CompletedTask;
                };
            });

            var commandDelegate = app.Build();

            await commandDelegate(context);

            var commandContextAccessor = context.CommandServices.GetRequiredService<ICommandContextAccessor>();
            var testDependency = context.CommandServices.GetRequiredService<TestDependency>();

            Assert.NotNull(commandContextAccessor);
            Assert.NotNull(commandContextAccessor.CommandContext);
            Assert.NotNull(testDependency);
            Assert.NotNull(testDependency.CommandContext);
            Assert.Equal(context, commandContextAccessor.CommandContext);
            Assert.Equal(context, testDependency.CommandContext);
        }
    }

    public class TestDependency
    {
        private readonly ICommandContextAccessor _commandContextAccessor;

        public TestDependency(ICommandContextAccessor commandContextAccessor)
        {
            _commandContextAccessor = commandContextAccessor;
        }

        public CommandContext CommandContext => _commandContextAccessor.CommandContext;
    }
}