using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandInputCountMatcherMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, ICommandServiceOptions config = null)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            if (config.HasContent())
                services.AddSingleton<ICommandServiceOptions>(config);
            else
                services.AddSingleton<ICommandServiceOptions, CommandServiceOptions>();

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UseInputCountMatcher();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        [Fact]
        public async Task DoAnythingIfResultSetted()
        {
            var context = new CommandContext();
            var result = new SuccessResult();

            context.Result = result;

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.Equal(result, context.Result);
        }

        [Fact]
        public async Task ThrowsExceptionForNoCommandMatchesFeature()
        {
            var context = new CommandContext();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }

        [Fact]
        public async Task ThrowsExceptionForEmptyCommandMatches()
        {
            var context = new CommandContext();

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }
    }

    public class TestCommandApplicationBuilder : ICommandApplicationBuilder
    {
        private readonly List<Func<CommandDelegate, CommandDelegate>> _components;

        public TestCommandApplicationBuilder(IServiceProvider applicationServices)
        {
            ApplicationServices = applicationServices;

            _components = new List<Func<CommandDelegate, CommandDelegate>>();
        }

        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public IServiceProvider ApplicationServices { get; }

        public CommandDelegate Build()
        {
            CommandDelegate app = context
                => Task.CompletedTask;

            _components.Reverse();

            foreach (var component in _components)
                app = component(app);

            return app;
        }

        public ICommandApplicationBuilder Use(Func<CommandDelegate, CommandDelegate> component)
        {
            _components.Add(component);

            return this;
        }
    }
}