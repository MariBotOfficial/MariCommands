using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Parsers;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandParserMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, IArgumentParser argumentParser, Action<MariCommandsOptions> config = null)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddOptions<MariCommandsOptions>();

            if (config.HasContent())
                services.Configure(config);

            services.AddSingleton<IArgumentParser>(argumentParser);

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UseParser();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        [Fact]
        public async Task DoAnythingIfResultSetted()
        {
            var parser = new TestArgumentParser();
            var context = new CommandContext();

            var result = new SuccessResult();

            context.Result = result;

            await ExecuteMiddlewareAsync(context, parser);

            Assert.NotNull(context.Result);
            Assert.Equal(result, context.Result);
        }

        [Fact]
        public async Task DoAnyThingIfArgsAndCommandIsSetted()
        {
            var parser = new TestArgumentParser();
            var context = new CommandContext();

            var args = new object[0];
            var command = new Mock<ICommand>().Object;

            context.Command = command;
            context.Args = args;

            await ExecuteMiddlewareAsync(context, parser);

            Assert.NotNull(context.Args);
            Assert.Equal(args, context.Args);
            Assert.NotNull(context.Command);
            Assert.Equal(command, context.Command);
        }

        [Fact]
        public async Task ThrowsIfCommandNotSettedAndDontHasMatchesFeature()
        {
            var parser = new TestArgumentParser();
            var context = new CommandContext();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, parser);
            });
        }

        [Fact]
        public async Task ChooseOneWhenContinueMultiMatchAfterParserIsDisabled()
        {
            var parser = new TestArgumentParser();
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            config.ContinueMultiMatchAfterParser = false;

            const string aliasName = "testAlias";

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>());
            match1Mock.SetupGet(a => a.Command.Priority).Returns(3);
            match1Mock.SetupGet(a => a.Alias).Returns(aliasName);

            match2Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>());
            match2Mock.SetupGet(a => a.Command.Priority).Returns(2);
            match2Mock.SetupGet(a => a.Alias).Returns(aliasName);

            var match1 = match1Mock.Object;
            var match2 = match2Mock.Object;

            var matches = new List<ICommandMatch>()
            {
                match1,
                match2,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            await ExecuteMiddlewareAsync(context, parser, config =>
            {
                config.ContinueMultiMatchAfterParser = false;
            });

            Assert.Null(context.Result);
            Assert.NotNull(context.Command);
            Assert.Equal(match1.Command, context.Command);
            Assert.False(string.IsNullOrWhiteSpace(context.Alias));
            Assert.Equal(aliasName, context.Alias);
            Assert.NotNull(context.Args);
        }

        [Fact]
        public async Task ErrorIfAllMatchesFailToParse()
        {
            // set true for fail parsing.
            var parser = new TestArgumentParser(true);
            var context = new CommandContext();

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(new Mock<ICommand>().Object);

            match2Mock.SetupGet(a => a.Command).Returns(new Mock<ICommand>().Object);

            var match1 = match1Mock.Object;
            var match2 = match2Mock.Object;

            var matches = new List<ICommandMatch>()
            {
                match1,
                match2,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            await ExecuteMiddlewareAsync(context, parser);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<MatchesFailedResult>(context.Result);
            Assert.NotEmpty((context.Result as MatchesFailedResult).FailedCommands);
        }

        [Fact]
        public async Task ChooseAllMatchesParseSuccess()
        {
            var cmdName1 = "command1";
            var cmdName2 = "command2";

            // set true for fail parsing.
            var parser = new TestArgumentParser(true, cmdName2);
            var config = new MariCommandsOptions();
            var context = new CommandContext();

            config.ContinueMultiMatchAfterParser = true;

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command.Name).Returns(cmdName1);
            match1Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>());

            match2Mock.SetupGet(a => a.Command.Name).Returns(cmdName2);
            match2Mock.SetupGet(a => a.Command.Parameters).Returns(new List<IParameter>());

            var match1 = match1Mock.Object;
            var match2 = match2Mock.Object;

            var matches = new List<ICommandMatch>()
            {
                match1,
                match2,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            await ExecuteMiddlewareAsync(context, parser, config =>
            {
                config.ContinueMultiMatchAfterParser = true;
            });

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();
            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
            Assert.NotNull(argumentParserFeature);
            Assert.NotEmpty(argumentParserFeature.CommandArgs);
        }

        [Fact]
        public async Task AlsoCanParseSettedCommandWithRawArgs()
        {
            var parser = new TestArgumentParser();
            var context = new CommandContext();

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Parameters).Returns(new List<IParameter>());

            var command = commandMock.Object;

            context.Command = command;
            context.RawArgs = string.Empty;

            await ExecuteMiddlewareAsync(context, parser);

            Assert.Null(context.Result);
            Assert.NotNull(context.Command);
            Assert.Equal(command, context.Command);
            Assert.NotNull(context.RawArgs);
            Assert.Equal(string.Empty, context.RawArgs);
            Assert.NotNull(context.Args);
        }
    }

    public class TestArgumentParser : IArgumentParser
    {
        public TestArgumentParser(bool fails = false, string commandNameToFail = null)
        {
            Fails = fails;
            CommandNameToFail = commandNameToFail;
        }

        public bool Fails { get; set; }

        public string CommandNameToFail { get; set; }

        public Task<IArgumentParserResult> ParseAsync(CommandContext context, ICommand command, string rawArgs)
        {
            if (Fails && (string.IsNullOrWhiteSpace(CommandNameToFail) || CommandNameToFail.Equals(command.Name)))
            {
                var mockResult = new Mock<IArgumentParserResult>();

                mockResult.SetupGet(a => a.Success).Returns(false);

                return Task.FromResult(mockResult.Object as IArgumentParserResult);
            }

            var args = new Dictionary<IParameter, object>();

            foreach (var parameter in command.Parameters)
                args.Add(parameter, new object());

            var result = ArgumentParseSuccessResult.FromArgs(args);

            return Task.FromResult(result as IArgumentParserResult);
        }
    }
}