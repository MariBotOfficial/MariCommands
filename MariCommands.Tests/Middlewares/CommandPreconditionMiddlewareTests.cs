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
    public class CommandPreconditionMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UsePreconditioner();

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
        public async Task ThrowsExceptionForEmptyCommandMatches()
        {
            var context = new CommandContext();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }

        [Fact]
        public async Task SuccessIfAllPreconditionsSucceeds()
        {
            var context = new CommandContext();

            var precondition1Mock = new Mock<PreconditionAttribute>();
            var precondition2Mock = new Mock<PreconditionAttribute>();
            var precondition3Mock = new Mock<PreconditionAttribute>();

            var command1Mock = new Mock<ICommand>();

            command1Mock.Setup(a => a.GetAllPreconditions()).Returns(new List<PreconditionAttribute>
            {
                precondition1Mock.Object,
                precondition2Mock.Object,
                precondition3Mock.Object,
            });

            var command = command1Mock.Object;

            precondition1Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            precondition2Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            precondition3Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(command);

            var matches = new List<ICommandMatch>
            {
                match1Mock.Object,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            await ExecuteMiddlewareAsync(context);

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.Null(context.Result);
            Assert.NotNull(matchesFeature);
            Assert.NotEmpty(matchesFeature.CommandMatches);
        }


        [Fact]
        public async Task FailsIfAnyPreconditionFails()
        {
            var context = new CommandContext();

            var precondition1Mock = new Mock<PreconditionAttribute>();
            var precondition2Mock = new Mock<PreconditionAttribute>();
            var precondition3Mock = new Mock<PreconditionAttribute>();

            var command1Mock = new Mock<ICommand>();

            command1Mock.Setup(a => a.GetAllPreconditions()).Returns(new List<PreconditionAttribute>
            {
                precondition1Mock.Object,
                precondition2Mock.Object,
                precondition3Mock.Object,
            });

            var command = command1Mock.Object;

            precondition1Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            precondition2Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult("failed") as IPreconditionResult);
            });
            precondition3Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            var match1Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(command);

            var matches = new List<ICommandMatch>
            {
                match1Mock.Object,
            };

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<PreconditionsFailResult>(context.Result);
            Assert.Equal(command, (context.Result as PreconditionsFailResult).Command);
        }

        [Fact]
        public async Task AlsoExecuteForSettedCommand()
        {
            var context = new CommandContext();

            var precondition1Mock = new Mock<PreconditionAttribute>();
            var precondition2Mock = new Mock<PreconditionAttribute>();
            var precondition3Mock = new Mock<PreconditionAttribute>();

            var command1Mock = new Mock<ICommand>();

            command1Mock.Setup(a => a.GetAllPreconditions()).Returns(new List<PreconditionAttribute>
            {
                precondition1Mock.Object,
                precondition2Mock.Object,
                precondition3Mock.Object,
            });

            var command = command1Mock.Object;

            precondition1Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });
            precondition2Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult("failed") as IPreconditionResult);
            });
            precondition3Mock.Setup(a => a.ExecuteAsync(command, context)).Returns(() =>
            {
                return Task.FromResult(new PreconditionResult() as IPreconditionResult);
            });

            context.Command = command;

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Command);
            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.IsType<PreconditionsFailResult>(context.Result);
            Assert.Equal(command, (context.Result as PreconditionsFailResult).Command);
        }
    }
}