using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class CommandExecutorMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, CommandServiceOptions config)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddSingleton<ICommandServiceOptions>(config);

            var provider = services.BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new TestCommandApplicationBuilder(provider);

            app.UseCommandExecutor();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        private ICommandExecutor CreateExecutor()
        {
            var type = typeof(TestModuleClassExecutor);
            var method = type.GetMethod(nameof(TestModuleClassExecutor.ExecuteAsync));

            var moduleBuilderMock = new Mock<IModuleBuilder>();
            var commandBuilderMock = new Mock<ICommandBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(SuccessResult));

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            return ResultTaskExecutor.Create(moduleBuilder, commandBuilder);
        }

        [Fact]
        public async Task DoAnythingIfResultIsSetted()
        {
            var config = new CommandServiceOptions();
            var context = new CommandContext();
            var result = new SuccessResult();

            context.Result = result;

            await ExecuteMiddlewareAsync(context, config);

            Assert.NotNull(context.Result);
            Assert.Equal(result, context.Result);
        }

        [Fact]
        public async Task ThrowsExceptionForEmptyCommandMatches()
        {
            var config = new CommandServiceOptions();
            var context = new CommandContext();

            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, config);
            });
        }

        [Fact]
        public async Task ThrowsExceptionForEmptyArguments()
        {
            var config = new CommandServiceOptions();
            var context = new CommandContext();

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature());

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, config);
            });
        }

        [Fact]
        public async Task CanExecuteSequential()
        {
            var context = new CommandContext();
            var config = new CommandServiceOptions();

            var executor = CreateExecutor();

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Module.Type).Returns(typeof(TestModuleClassExecutor));
            commandMock.Setup(a => a.GetRunMode(config)).Returns(RunMode.Sequential);
            commandMock.SetupGet(a => a.Executor).Returns(executor);

            var command = commandMock.Object;
            var args = new object[0];

            context.Command = command;
            context.Args = args;

            await ExecuteMiddlewareAsync(context, config);

            Assert.NotNull(context.Result);
            Assert.True(context.Result.Success);
            Assert.IsType<SuccessResult>(context.Result);
            Assert.NotNull(context.Command);
            Assert.Equal(command, context.Command);
            Assert.NotNull(context.Args);
            Assert.Equal(args, context.Args);
        }

        [Fact]
        public async Task CanExecuteConcurrent()
        {
            var context = new CommandContext();
            var config = new CommandServiceOptions();

            var executor = CreateExecutor();

            var commandMock = new Mock<ICommand>();

            commandMock.SetupGet(a => a.Module.Type).Returns(typeof(TestModuleClassExecutor));
            commandMock.Setup(a => a.GetRunMode(config)).Returns(RunMode.Concurrent);
            commandMock.SetupGet(a => a.Executor).Returns(executor);

            var command = commandMock.Object;
            var args = new object[0];

            context.Command = command;
            context.Args = args;

            await ExecuteMiddlewareAsync(context, config);

            Assert.NotNull(context.Result);
            Assert.True(context.Result.Success);
            Assert.NotNull(context.Command);
            Assert.Equal(command, context.Command);
            Assert.NotNull(context.Args);
            Assert.Equal(args, context.Args);
            Assert.IsType<AsyncResult>(context.Result);

            var asyncResult = context.Result as AsyncResult;

            var result = await asyncResult.GetResultAsync();

            Assert.NotNull(result);
            Assert.IsType<SuccessResult>(result);
        }

        [Fact]
        public async Task ChooseHigherPriorityFromMatches()
        {
            var context = new CommandContext();
            var config = new CommandServiceOptions();

            config.AutoDisposeContext = false;

            var alias = "testAlias";
            var executor = CreateExecutor();

            var command1Mock = new Mock<ICommand>();
            var command2Mock = new Mock<ICommand>();

            command1Mock.SetupGet(a => a.Module.Type).Returns(typeof(TestModuleClassExecutor));
            command1Mock.Setup(a => a.GetRunMode(config)).Returns(RunMode.Sequential);
            command1Mock.SetupGet(a => a.Executor).Returns(executor);
            command1Mock.SetupGet(a => a.Priority).Returns(3);

            command2Mock.SetupGet(a => a.Module.Type).Returns(typeof(TestModuleClassExecutor));
            command2Mock.Setup(a => a.GetRunMode(config)).Returns(RunMode.Sequential);
            command2Mock.SetupGet(a => a.Executor).Returns(executor);
            command2Mock.SetupGet(a => a.Priority).Returns(2);

            var command1 = command1Mock.Object;
            var command2 = command2Mock.Object;

            var match1Mock = new Mock<ICommandMatch>();
            var match2Mock = new Mock<ICommandMatch>();

            match1Mock.SetupGet(a => a.Command).Returns(command1);
            match1Mock.SetupGet(a => a.Alias).Returns(alias);

            match2Mock.SetupGet(a => a.Command).Returns(command2);
            match2Mock.SetupGet(a => a.Alias).Returns(alias);

            var match1 = match1Mock.Object;
            var match2 = match2Mock.Object;

            var matches = new List<ICommandMatch>
            {
                match1,
                match2,
            };

            var args = new object[0];

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = matches,
            });
            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature
            {
                CommandArgs = new Dictionary<ICommandMatch, object[]>
                {
                    { match1, args },
                    { match2, args },
                }
            });

            await ExecuteMiddlewareAsync(context, config);

            var commandMatchesFeature = context.Features.Get<ICommandMatchesFeature>();
            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            Assert.NotNull(context.Result);
            Assert.NotNull(commandMatchesFeature);
            Assert.NotNull(argumentParserFeature);
            Assert.True(context.Result.Success);
            Assert.IsType<SuccessResult>(context.Result);
            Assert.NotNull(context.Command);
            Assert.NotNull(context.Args);
            Assert.NotNull(context.Alias);
            Assert.Equal(command1, context.Command);
            Assert.Equal(args, context.Args);
            Assert.Equal(alias, context.Alias);
        }
    }

    public class TestModuleClassExecutor : ModuleBase<CommandContext>
    {
        public Task<SuccessResult> ExecuteAsync()
            => Task.FromResult(new SuccessResult());
    }
}