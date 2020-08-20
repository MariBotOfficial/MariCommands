using System;
using System.Threading;
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
        public async ValueTask CanExecuteAnyWorkerCommandAfterAddCommandStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .UseCommandStartup<TestCommandStartup>()
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<TestWorkerService>();
                    })
                    .Build();

            await host.StartAsync();
        }

        [Fact]
        public async ValueTask CantExecuteAnyWorkerCommandBeforeAddCommandStartup()
        {
            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                        .ConfigureServices(services =>
                        {
                            services.AddHostedService<TestWorkerService>();
                        })
                        .UseCommandStartup<TestCommandStartup>()
                        .Build();

                await host.StartAsync();
            });
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

        [Fact]
        public async ValueTask CanExecuteAnyWorkerCommandAfterAddCommandServiceStartup()
        {
            var host = Host.CreateDefaultBuilder()
                    .UseCommandServiceStartup<TestCommandStartup>()
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<TestWorkerService>();
                    })
                    .Build();

            await host.StartAsync();
        }

        [Fact]
        public async ValueTask CantExecuteAnyWorkerCommandBeforeAddCommandServiceStartup()
        {
            await Assert.ThrowsAnyAsync<InvalidOperationException>(async () =>
            {
                var host = Host.CreateDefaultBuilder()
                        .ConfigureServices(services =>
                        {
                            services.AddHostedService<TestWorkerService>();
                        })
                        .UseCommandServiceStartup<TestCommandStartup>()
                        .Build();

                await host.StartAsync();
            });
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

    public class TestWorkerService : BackgroundService
    {
        private readonly IContextExecutor _contextExecutor;

        public TestWorkerService(IContextExecutor contextExecutor)
        {
            _contextExecutor = contextExecutor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // If the app is not started yet it will throw exception.
            await _contextExecutor.ExecuteAsync(string.Empty, new CommandContext());
        }
    }
}