using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class ObjectExecutorTests
    {
        [Fact]
        public async Task CanExecuteObjectAsync()
        {
            var type = typeof(ObjectExecutorClassTest);

            var method = type.GetMethod(nameof(ObjectExecutorClassTest.TestMethodObject));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ObjectExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ObjectExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }

        [Fact]
        public async Task CanExecuteInheritedObjectAsync()
        {
            var type = typeof(ObjectExecutorClassTest);

            var method = type.GetMethod(nameof(ObjectExecutorClassTest.TestMethodInheritedObject));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ObjectExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ObjectExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class ObjectExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogValue = "Value";

        public object TestMethodObject()
            => LogValue;

        public string TestMethodInheritedObject()
            => LogValue;
    }
}