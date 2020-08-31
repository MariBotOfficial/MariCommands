using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class ObjectValueTaskExecutorTests
    {
        [Fact]
        public async Task CanExecuteResultAsync()
        {
            var type = typeof(ObjectValueTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ObjectValueTaskExecutorClassTest.TestMethodResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(object));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ObjectValueTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ObjectValueTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ObjectValueTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }

        [Fact]
        public async Task CanExecuteInheritedResultAsync()
        {
            var type = typeof(ObjectValueTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ObjectValueTaskExecutorClassTest.TestMethodInheritedResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(string));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ObjectValueTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ObjectValueTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ObjectValueTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class ObjectValueTaskExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogValue = "Value";

        public ValueTask<object> TestMethodResult()
            => new ValueTask<object>(LogValue);

        public ValueTask<string> TestMethodInheritedResult()
            => new ValueTask<string>(LogValue);
    }
}