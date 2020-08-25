using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandStringMatcherMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, IModuleCache moduleCache)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddSingleton<IModuleCache>(moduleCache);

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UseStringMatcher();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        private string GetRemainingInput(string cmdName, string input)
            => input.Substring(cmdName.Length);

        [Fact]
        public async Task DoAnythingIfResultSetted()
        {
            var context = new CommandContext();
            var result = new SuccessResult();

            var moduleCacheMock = new Mock<IModuleCache>();

            context.Result = result;

            await ExecuteMiddlewareAsync(context, moduleCacheMock.Object);

            Assert.NotNull(context.Result);
            Assert.Equal(result, context.Result);
        }

        [Fact]
        public async Task DoAnythingIfCommandIsSetted()
        {
            var context = new CommandContext();
            var command = new Mock<ICommand>().Object;

            var moduleCacheMock = new Mock<IModuleCache>();

            context.Command = command;

            await ExecuteMiddlewareAsync(context, moduleCacheMock.Object);

            Assert.NotNull(context.Command);
            Assert.Equal(command, context.Command);
        }


        [Fact]
        public async Task ThrowsExceptionIfRawArgsIsNullOrWhiteSpace()
        {
            var context = new CommandContext();

            var moduleCacheMock = new Mock<IModuleCache>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, moduleCacheMock.Object);
            });
        }

        [Fact]
        public async Task NotFoundIfNoneMatches()
        {
            const string input = "testInput";

            var context = new CommandContext();

            var moduleCacheMock = new Mock<IModuleCache>();

            moduleCacheMock.Setup(a => a.SearchCommandsAsync(input)).Returns(() =>
            {
                var matches = new List<ICommandMatch>();

                return new ValueTask<IReadOnlyCollection<ICommandMatch>>(matches);
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context, moduleCacheMock.Object);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<CommandNotFoundResult>(context.Result);
            Assert.Equal(input, (context.Result as CommandNotFoundResult).Input);
        }

        [Fact]
        public async Task JustCatchMatchesWhereEnabled()
        {
            const string input = "testInput";

            var context = new CommandContext();

            var moduleCacheMock = new Mock<IModuleCache>();

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();
            var match3Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command.IsEnabled).Returns(true);

            match2Mock.SetupGet(a => a.Command.IsEnabled).Returns(false);

            match3Mock.SetupGet(a => a.Command.IsEnabled).Returns(true);

            var matches = new List<ICommandMatch>()
            {
                match1Mock.Object,
                match2Mock.Object,
                match3Mock.Object,
            };

            moduleCacheMock.Setup(a => a.SearchCommandsAsync(input)).Returns(() =>
            {
                return new ValueTask<IReadOnlyCollection<ICommandMatch>>(matches);
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context, moduleCacheMock.Object);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
            Assert.Equal(matches.Count(a => a.Command.IsEnabled), matchesFeature.CommandMatches.Count);
        }
    }
}