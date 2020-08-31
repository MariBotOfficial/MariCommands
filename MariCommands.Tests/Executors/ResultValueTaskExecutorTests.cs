using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class ResultValueTaskExecutorTests
    {
        [Fact]
        public async Task CanExecuteResultAsync()
        {
            var type = typeof(ResultValueTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ResultValueTaskExecutorClassTest.TestMethodResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(IResult));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultValueTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultValueTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultValueTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }

        [Fact]
        public async Task CanExecuteInheritedResultAsync()
        {
            var type = typeof(ResultValueTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ResultValueTaskExecutorClassTest.TestMethodInheritedResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(SuccessObjectResult));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultValueTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultValueTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultValueTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class ResultValueTaskExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogValue = "Value";

        public ValueTask<IResult> TestMethodResult()
            => new ValueTask<IResult>(new SuccessObjectResult(LogValue));

        public ValueTask<SuccessObjectResult> TestMethodInheritedResult()
            => new ValueTask<SuccessObjectResult>(new SuccessObjectResult(LogValue));
    }
}