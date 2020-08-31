using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class ResultExecutorTests
    {
        [Fact]
        public async Task CanExecuteResultAsync()
        {
            var type = typeof(ResultExecutorClassTest);

            var method = type.GetMethod(nameof(ResultExecutorClassTest.TestMethodResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }

        [Fact]
        public async Task CanExecuteInheritedResultAsync()
        {
            var type = typeof(ResultExecutorClassTest);

            var method = type.GetMethod(nameof(ResultExecutorClassTest.TestMethodInheritedResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class ResultExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogValue = "Value";

        public IResult TestMethodResult()
            => new SuccessObjectResult(LogValue);

        public SuccessObjectResult TestMethodInheritedResult()
            => new SuccessObjectResult(LogValue);
    }
}