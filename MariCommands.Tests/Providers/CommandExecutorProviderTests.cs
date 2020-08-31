using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Providers;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Providers
{
    public class CommandExecutorProviderTests
    {
        [Fact]
        public void CanGetVoidExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.VoidCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<VoidExecutor>(executor);
        }

        [Fact]
        public void CanGetResultExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ResultCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ResultExecutor>(executor);
        }

        [Fact]
        public void CanGetObjectExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ObjectCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ObjectExecutor>(executor);
        }

        [Fact]
        public void CanGetVoidTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.VoidTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<VoidTaskExecutor>(executor);
        }

        [Fact]
        public void CanGetResultTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ResultTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(IResult));

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ResultTaskExecutor>(executor);
        }

        [Fact]
        public void CanGetObjectTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ObjectTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(object));

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ObjectTaskExecutor>(executor);
        }

        [Fact]
        public void CanGetVoidValueTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.VoidValueTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<VoidValueTaskExecutor>(executor);
        }

        [Fact]
        public void CanGetResultValueTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ResultValueTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(IResult));

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ResultValueTaskExecutor>(executor);
        }

        [Fact]
        public void CanGetObjectValueTaskExecutor()
        {
            var type = typeof(CommandExecutorProviderClassTest);
            var method = type.GetMethod(nameof(CommandExecutorProviderClassTest.ObjectValueTaskCommand));

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(object));

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var provider = new CommandExecutorProvider();

            var executor = provider.GetCommandExecutor(moduleBuilder, commandBuilder);

            Assert.NotNull(executor);
            Assert.IsType<ObjectValueTaskExecutor>(executor);
        }
    }

    public class CommandExecutorProviderClassTest
    {
        public void VoidCommand()
        { }

        public IResult ResultCommand()
            => new SuccessResult();

        public object ObjectCommand()
            => new object();

        public Task VoidTaskCommand()
            => Task.CompletedTask;

        public Task<IResult> ResultTaskCommand()
            => Task.FromResult<IResult>(new SuccessResult());

        public Task<object> ObjectTaskCommand()
            => Task.FromResult(new object());

        public ValueTask VoidValueTaskCommand()
            => default;

        public ValueTask<IResult> ResultValueTaskCommand()
            => new ValueTask<IResult>(new SuccessResult());

        public ValueTask<object> ObjectValueTaskCommand()
            => new ValueTask<object>(new object());
    }
}