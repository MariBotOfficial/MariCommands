using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class VoidValueTaskExecutorTests
    {
        [Fact]
        public async Task CanExecuteTaskAsync()
        {
            var type = typeof(VoidValueTaskExecutorClassTest);

            var method = type.GetMethod(nameof(VoidValueTaskExecutorClassTest.TestMethod));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(true);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = VoidValueTaskExecutor.Create(moduleBuilder, commandBuilder);

            var module = new VoidValueTaskExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessResult>(result);
            Assert.NotEmpty(context.Items);
            Assert.True(context.Items.ContainsKey(VoidValueTaskExecutorClassTest.LogKey));
            Assert.Equal(VoidValueTaskExecutorClassTest.LogValue, context.Items[VoidValueTaskExecutorClassTest.LogKey]);
        }
    }

    public class VoidValueTaskExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogKey = "Log";

        public const string LogValue = "Value";

        public ValueTask TestMethod()
        {
            Context.Items[LogKey] = LogValue;

            return new ValueTask();
        }
    }
}