using System.Threading.Tasks;
using MariCommands.Executors;
using MariCommands.Results;
using Moq;
using Xunit;

namespace MariCommands.Tests.Executors
{
    public class VoidExecutorTests
    {
        [Fact]
        public async Task CanExecuteVoidAsync()
        {
            var type = typeof(VoidExecutorClassTest);

            var method = type.GetMethod(nameof(VoidExecutorClassTest.TestMethod));

            await using var context = new CommandContext();

            var moduleBuilderMock = new Mock<IModuleBuilder>();

            moduleBuilderMock.SetupGet(a => a.Type).Returns(type);

            var commandBuilderMock = new Mock<ICommandBuilder>();

            commandBuilderMock.SetupGet(a => a.IsAsync).Returns(false);
            commandBuilderMock.SetupGet(a => a.MethodInfo).Returns(method);

            var moduleBuilder = moduleBuilderMock.Object;
            var commandBuilder = commandBuilderMock.Object;

            var executor = VoidExecutor.Create(moduleBuilder, commandBuilder);

            var module = new VoidExecutorClassTest();

            (module as IModuleBase).SetContext(context);

            var result = await executor.ExecuteAsync(module, new object[0]);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.IsType<SuccessResult>(result);
            Assert.NotEmpty(context.Items);
            Assert.True(context.Items.ContainsKey(VoidExecutorClassTest.LogKey));
            Assert.Equal(VoidExecutorClassTest.LogValue, context.Items[VoidExecutorClassTest.LogKey]);
        }
    }

    public class VoidExecutorClassTest : ModuleBase<CommandContext>
    {
        public const string LogKey = "Log";

        public const string LogValue = "Value";

        public void TestMethod()
        {
            Context.Items[LogKey] = LogValue;
        }
    }
}