using System;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Hosting;
using MariCommands.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace MariCommands.Tests.HostBuilder
{
    public class HostBuilderTests
    {
        public HostBuilderTests()
        {
            MariCommandsHostBuilderExtensions.Clear();
        }

        [Fact]
        public void TwoStartupsThrowsException()
        {
            var builder = Host.CreateDefaultBuilder();

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                builder
                    .UseCommandStartup<TestCommandStartup>()
                    .UseCommandStartup<TestCommandStartup>()
                    .Build();
            });
        }

        [Fact]
        public void OneStartupNotThrowsException()
        {
            Host.CreateDefaultBuilder()
                    .UseCommandStartup<TestCommandStartup>()
                    .Build();
        }

        [Fact]
        public async ValueTask CanExecuteAnyCommandAfterAddCommandStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .UseCommandStartup<TestCommandStartup>()
                    .Build();

            await host.StartAsync();

            var contextExecutor = host.Services.GetRequiredService<IContextExecutor>();

            await contextExecutor.ExecuteAsync(string.Empty, new CommandContext());
        }

        [Fact]
        public void TwoCommandServiceStartupsThrowsException()
        {
            var builder = Host.CreateDefaultBuilder();

            Assert.ThrowsAny<InvalidOperationException>(() =>
            {
                builder
                    .UseCommandServiceStartup<TestCommandStartup>()
                    .UseCommandServiceStartup<TestCommandStartup>()
                    .Build();
            });
        }

        [Fact]
        public void OneCommandServiceStartupNotThrowsException()
        {
            Host.CreateDefaultBuilder()
                    .UseCommandServiceStartup<TestCommandStartup>()
                    .Build();
        }

        [Fact]
        public async ValueTask CanExecuteAnyCommandAfterAddCommandServiceStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .UseCommandServiceStartup<TestCommandStartup>()
                    .Build();

            await host.StartAsync();

            var contextExecutor = host.Services.GetRequiredService<IContextExecutor>();

            await contextExecutor.ExecuteAsync(string.Empty, new CommandContext());
        }
    }

    public class TestCommandStartup : ICommandServiceStartup
    {

        private class TestMiddleware : ICommandMiddleware
        {
            public Task InvokeAsync(CommandContext context, CommandDelegate next)
                => Task.CompletedTask;
        }

        public void ConfigureApp(ICommandApplicationBuilder app)
        {
            app.UseMiddleware<TestMiddleware>();
        }

        public void ConfigureModules(IModuleConfigurer moduleConfigurer)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}