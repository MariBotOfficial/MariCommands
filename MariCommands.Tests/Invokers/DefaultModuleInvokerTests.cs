using MariCommands.Invokers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MariCommands.Tests.Invokers
{
    public class DefaultModuleInvokerTests
    {
        [Fact]
        public void CanCreateInvoker()
        {
            var moduleType = typeof(TestModuleClassInvoker);

            var invoker = DefaultModuleInvoker.Create(moduleType);

            Assert.NotNull(invoker);
        }

        [Fact]
        public void CanInstantiateModule()
        {
            var services = new ServiceCollection();

            using var provider = services.BuildServiceProvider(true);

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using var scope = provider.CreateScope();

            var moduleType = typeof(TestModuleClassInvoker);

            var invoker = DefaultModuleInvoker.Create(moduleType);
            var instance = invoker.CreateInstance(scope.ServiceProvider);

            Assert.NotNull(invoker);
            Assert.NotNull(instance);
            Assert.IsType<TestModuleClassInvoker>(instance);
        }
    }

    public class TestModuleClassInvoker : ModuleBase<CommandContext>
    {

    }
}