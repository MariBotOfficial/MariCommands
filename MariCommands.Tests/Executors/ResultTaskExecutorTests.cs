using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class ResultTaskExecutorTests
    {
        [Fact]
        public async Task CanExecuteResultAsync()
        {
            var type = typeof(ResultTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ResultTaskExecutorClassTest.TestMethodResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(IResult));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }

        [Fact]
        public async Task CanExecuteInheritedResultAsync()
        {
            var type = typeof(ResultTaskExecutorClassTest);

            var method = type.GetMethod(nameof(ResultTaskExecutorClassTest.TestMethodInheritedResult));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.AsyncResultType).Returns(typeof(SuccessObjectResult));
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = ResultTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new ResultTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessObjectResult>(result);
            Assert.Equal(ResultTaskExecutorClassTest.LogValue, (result as SuccessObjectResult).Value);
        }
    }

    public class ResultTaskExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogValue = "Value";

        public Task<IResult> TestMethodResult()
            => Task.FromResult<IResult>(new SuccessObjectResult(LogValue));

        public Task<SuccessObjectResult> TestMethodInheritedResult()
            => Task.FromResult(new SuccessObjectResult(LogValue));
    }
}
