using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandInputCountMatcherMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, MariCommandsOptions config = null)
        {
            var services = new ServiceCollection();

            services.AddLogging();


            if (config.HasContent())
                services.AddSingleton<IOptions<MariCommandsOptions>>(config);
            else
                services.AddOptions<MariCommandsOptions>();

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
        public async Task DoAnyThingIfArgsIsSetted()
        {
            var context = new CommandContext();
            var args = new object[1];

            context.Args = args;

            await ExecuteMiddlewareAsync(context);

            Assert.Null(context.Result);
            Assert.Equal(args, context.Args);
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

        [Fact]
        public async Task ReturnsMultiMatchResultForNoMultiMatchHandling()
        {
            var config = new MariCommandsOptions();

            var context = new CommandContext();

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Error);
            match2Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Error);

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                    match2Mock.Object,
                }
            });

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<MultiMatchErrorResult>(context.Result);
            Assert.Equal(2, (context.Result as MultiMatchErrorResult).Matches.Count);
        }

        [InlineData("arg1")]
        [InlineData("arg1 arg2")]
        [Theory]
        public async Task ReturnBestMatchesWhenHasOneOrMore(string input)
        {
            var config = new MariCommandsOptions();

            config.MatchHandling = MultiMatchHandling.Best;

            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();
            var paramMock2 = new Mock<IParameter>();

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();
            var match3Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
                paramMock2.Object,
            });

            match2Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match2Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match2Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
                paramMock2.Object,
            });

            match3Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match3Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match3Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                    match2Mock.Object,
                    match3Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context, config);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
        }

        [InlineData("arg1 arg2")]
        [InlineData("arg1 arg2 arg3")]
        [InlineData("arg1 arg2 arg3 arg4")]
        [InlineData("arg1 arg2 arg3 arg4 arg5")]
        [Theory]
        public async Task ReturnsMultiMatchResultWhenDontHasOne(string input)
        {
            var config = new MariCommandsOptions();

            config.MatchHandling = MultiMatchHandling.Best;

            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();
            var match3Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            match2Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match2Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match2Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            match3Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match3Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match3Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                    match2Mock.Object,
                    match3Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context, config);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<MatchesFailedResult>(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty((context.Result as MatchesFailedResult).FailedCommands);
        }

        [InlineData("arg1")]
        [InlineData("arg1 arg2")]
        [InlineData("arg1 arg2 arg3")]
        [Theory]
        public async Task ReturnsBestMatchesWhenParamsIsOptional(string input)
        {
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();
            var paramMock2 = new Mock<IParameter>();
            var paramMock3 = new Mock<IParameter>();

            paramMock1.SetupGet(a => a.IsOptional).Returns(true);
            paramMock2.SetupGet(a => a.DefaultValue).Returns(true);
            paramMock2.SetupGet(a => a.IsParams).Returns(true);

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
                paramMock2.Object,
                paramMock3.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
        }

        [InlineData("")]
        [InlineData("arg1")]
        [Theory]
        public async Task OptionalButNoOptionalCorrectCount(string input)
        {
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();
            var paramMock2 = new Mock<IParameter>();
            var paramMock3 = new Mock<IParameter>();

            paramMock1.SetupGet(a => a.IsOptional).Returns(true);

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
                paramMock2.Object,
                paramMock3.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<BadArgCountResult>(context.Result);
            Assert.NotNull(matchesFeature);
        }

        [InlineData("arg1 arg2")]
        [InlineData("arg1 arg2 arg3")]
        [InlineData("arg1 arg2 arg3 arg4")]
        [InlineData("arg1 arg2 arg3 arg4 arg5")]
        [Theory]
        public async Task FailsWithoutIgnoreExtraArgsAndMoreCount(string input)
        {
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<BadArgCountResult>(context.Result);
            Assert.NotNull(matchesFeature);
        }

        [InlineData("arg1 arg2")]
        [InlineData("arg1 arg2 arg3")]
        [InlineData("arg1 arg2 arg3 arg4")]
        [InlineData("arg1 arg2 arg3 arg4 arg5")]
        [Theory]
        public async Task SuccessWithIgnoreExtraArgsAndMoreCount(string input)
        {
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            config.IgnoreExtraArgs = true;

            var paramMock1 = new Mock<IParameter>();

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.Setup(a => a.Command.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            match1Mock.Setup(a => a.Command.GetIgnoreExtraArgs(config)).Returns(true);
            match1Mock.SetupGet(a => a.RemainingInput).Returns(input);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1Mock.Object,
                }
            });

            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context, config);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
        }

        [InlineData("arg1 arg2")]
        [InlineData("arg1 arg2 arg3")]
        [InlineData("arg1 arg2 arg3 arg4")]
        [InlineData("arg1 arg2 arg3 arg4 arg5")]
        [Theory]
        public async Task AlsoValidateCommandWithoutArgs(string input)
        {
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            var paramMock1 = new Mock<IParameter>();

            paramMock1.SetupGet(a => a.IsOptional).Returns(true);

            var command1Mock = new Mock<ICommand>();

            command1Mock.Setup(a => a.Module.GetMatchHandling(config)).Returns(MultiMatchHandling.Best);
            command1Mock.SetupGet(a => a.Parameters).Returns(new List<IParameter>
            {
                paramMock1.Object,
            });

            context.Command = command1Mock.Object;
            context.RawArgs = input;

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<BadArgCountResult>(context.Result);
            Assert.NotNull(matchesFeature);
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