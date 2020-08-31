using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandParamPreconditionMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UseParamPreconditioner();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        [Fact]
        public async Task DoAnythingIfResultSetted()
        {
            var result = new SuccessResult();
            var context = new CommandContext();

            context.Result = result;

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.Equal(result, context.Result);
        }

        [Fact]
        public async Task ThrowsExceptionForEmptyCommandMatches()
        {
            var context = new CommandContext();

            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }

        [Fact]
        public async Task ThrowsExceptionForEmptyArguments()
        {
            var context = new CommandContext();

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }

        [Fact]
        public async Task SuccessIfAllPreconditionsSucceeds()
        {
            var value = new object();

            var context = new CommandContext();

            var paramPreconditionMock1 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock2 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock3 = new Mock<ParamPreconditionAttribute>();

            var parameter1Mock = new Mock<IParameter>();

            parameter1Mock.SetupGet(a => a.Preconditions).Returns(new List<ParamPreconditionAttribute>
            {
                paramPreconditionMock1.Object,
                paramPreconditionMock2.Object,
                paramPreconditionMock3.Object,
            });

            var command1Mock = new Mock<ICommand>();

            var parameter = parameter1Mock.Object;

            command1Mock.SetupGet(a => a.Parameters).Returns(() =>
            {
                return new List<IParameter>
                {
                    parameter,
                };
            });

            var command = command1Mock.Object;

            paramPreconditionMock1.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            paramPreconditionMock2.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            paramPreconditionMock3.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(command);

            var match = match1Mock.Object;

            var matches = new List<ICommandMatch>
            {
                match,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature
            {
                CommandArgs = new Dictionary<ICommandMatch, object[]>
                {
                    { match, new object[] { value, } },
                },
            });

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();
            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
            Assert.NotNull(argumentParserFeature);
            Assert.NotEmpty(argumentParserFeature.CommandArgs);
        }

        [Fact]
        public async Task FailsIfAnyPreconditionFails()
        {
            var value = new object();

            var context = new CommandContext();

            var paramPreconditionMock1 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock2 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock3 = new Mock<ParamPreconditionAttribute>();

            var parameter1Mock = new Mock<IParameter>();

            parameter1Mock.SetupGet(a => a.Name).Returns("testParam");
            parameter1Mock.SetupGet(a => a.Preconditions).Returns(new List<ParamPreconditionAttribute>
            {
                paramPreconditionMock1.Object,
                paramPreconditionMock2.Object,
                paramPreconditionMock3.Object,
            });

            var command1Mock = new Mock<ICommand>();

            var command = command1Mock.Object;

            parameter1Mock.SetupGet(a => a.Command).Returns(command);

            var parameter = parameter1Mock.Object;

            command1Mock.SetupGet(a => a.Name).Returns("testCommand");
            command1Mock.SetupGet(a => a.Parameters).Returns(() =>
            {
                return new List<IParameter>
                {
                    parameter,
                };
            });

            paramPreconditionMock1.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            paramPreconditionMock2.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult("fail") as IPreconditionResult);
            });
            paramPreconditionMock3.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(command);

            var match = match1Mock.Object;

            var matches = new List<ICommandMatch>
            {
                match,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature
            {
                CommandArgs = new Dictionary<ICommandMatch, object[]>
                {
                    { match, new object[] { value, } },
                },
            });

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<ParamPreconditionsFailResult>(context.Result);
            Assert.Equal(parameter, (context.Result as ParamPreconditionsFailResult).Parameter);
        }

        [Fact]
        public async Task AlsoExecuteForSettedCommandAndArgs()
        {
            var value = new object();

            var context = new CommandContext();

            var paramPreconditionMock1 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock2 = new Mock<ParamPreconditionAttribute>();
            var paramPreconditionMock3 = new Mock<ParamPreconditionAttribute>();

            var parameter1Mock = new Mock<IParameter>();

            parameter1Mock.SetupGet(a => a.Name).Returns("testParam");
            parameter1Mock.SetupGet(a => a.Preconditions).Returns(new List<ParamPreconditionAttribute>
            {
                paramPreconditionMock1.Object,
                paramPreconditionMock2.Object,
                paramPreconditionMock3.Object,
            });

            var command1Mock = new Mock<ICommand>();

            var command = command1Mock.Object;

            parameter1Mock.SetupGet(a => a.Command).Returns(command);

            var parameter = parameter1Mock.Object;

            command1Mock.SetupGet(a => a.Name).Returns("testCommand");
            command1Mock.SetupGet(a => a.Parameters).Returns(() =>
            {
                return new List<IParameter>
                {
                    parameter,
                };
            });

            paramPreconditionMock1.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            paramPreconditionMock2.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult("fail") as IPreconditionResult);
            });
            paramPreconditionMock3.Setup(a => a.ExecuteAsync(value, parameter, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            context.Command = command;
            context.Args = new object[] { value };

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<ParamPreconditionsFailResult>(context.Result);
            Assert.Equal(parameter, (context.Result as ParamPreconditionsFailResult).Parameter);
        }

        [Fact]
        public async Task SuccessIfNoParameters()
        {
            var context = new CommandContext();

            var command1Mock = new Mock<ICommand>();

            command1Mock.SetupGet(a => a.Name).Returns("testCommand");
            command1Mock.SetupGet(a => a.Parameters).Returns(() =>
            {
                return new List<IParameter>();
            });

            var command = command1Mock.Object;

            context.Command = command;
            context.Args = new object[0];

            await ExecuteMiddlewareAsync(context);

            Assert.Null(context.Result);
        }
    }
}