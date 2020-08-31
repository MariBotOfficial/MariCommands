using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class VoidTaskExecutorTests
    {
        [Fact]
        public async Task CanExecuteTaskAsync()
        {
            var type = typeof(VoidTaskExecutorClassTest);

            var method = type.GetMethod(nameof(VoidTaskExecutorClassTest.TestMethod));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = VoidTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new VoidTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessResult>(result);
            Assert.NotEmpty(context.Items);
            Assert.True(context.Items.ContainsKey(VoidTaskExecutorClassTest.LogKey));
            Assert.Equal(VoidTaskExecutorClassTest.LogValue, context.Items[VoidTaskExecutorClassTest.LogKey]);
        }
    }

    public class VoidTaskExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogKey = "Log";

        public const string LogValue = "Value";

        public Task TestMethod()
        {
            Context.Items[LogKey] = LogValue;

            return Task.CompletedTask;
        }
    }
}