using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.Services
{
    public class ContextExecutorTests
    {
        [Fact]
        public async Task ThrowsExceptionWhenNotInitialized()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ContextExecutor>();

            var provider = services.BuildServiceProvider(true);

            var contextExecutor = provider.GetRequiredService<ContextExecutor>();

            var context = new CommandContext();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await contextExecutor.ExecuteAsync("test", context);
            });
        }

        [Fact]
        public void ThrowsExceptionWhenInitializedMoreThanOnce()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ContextExecutor>();

            var provider = services.BuildServiceProvider(true);

            var contextExecutor = provider.GetRequiredService<ContextExecutor>();

            CommandDelegate commandDelegate = context =>
                Task.CompletedTask;

            contextExecutor.Initialize(commandDelegate);

            Assert.Throws<InvalidOperationException>(() =>
            {
                contextExecutor.Initialize(commandDelegate);
            });
        }

        [Fact]
        public async Task CanExecuteRawInputAsync()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ContextExecutor>();

            var provider = services.BuildServiceProvider(true);

            var contextExecutor = provider.GetRequiredService<ContextExecutor>();

            var executed = false;
            var input = "testInput";
            var context = new CommandContext();

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            CommandDelegate commandDelegate = context =>
            {
                executed = true;

                return Task.CompletedTask;
            };

            contextExecutor.Initialize(commandDelegate);

            await contextExecutor.ExecuteAsync(input, context);

            Assert.True(executed);
            Assert.Equal(input, context.RawArgs);
        }

        [Fact]
        public async Task CanExecuteCommandWithRawArguments()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ContextExecutor>();

            var provider = services.BuildServiceProvider(true);

            var contextExecutor = provider.GetRequiredService<ContextExecutor>();

            var executed = false;
            var command = new Mock<ICommand>().Object;
            var args = "testarg1 testarg2";
            var context = new CommandContext();

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            CommandDelegate commandDelegate = context =>
            {
                executed = true;

                return Task.CompletedTask;
            };

            contextExecutor.Initialize(commandDelegate);

            await contextExecutor.ExecuteAsync(command, args, context);

            Assert.True(executed);
            Assert.Equal(command, context.Command);
            Assert.Equal(args, context.RawArgs);
        }

        [Fact]
        public async Task CanExecuteCommandWithParsedArgs()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ContextExecutor>();

            var provider = services.BuildServiceProvider(true);

            var contextExecutor = provider.GetRequiredService<ContextExecutor>();

            var executed = false;
            var command = new Mock<ICommand>().Object;
            var args = new object[]
            {
                "testarg1",
                "testarg2"
            };
            var context = new CommandContext();

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            CommandDelegate commandDelegate = context =>
            {
                executed = true;

                return Task.CompletedTask;
            };

            contextExecutor.Initialize(commandDelegate);

            await contextExecutor.ExecuteAsync(command, args, context);

            Assert.True(executed);
            Assert.Equal(command, context.Command);
            Assert.Equal(args, context.Args);
        }
    }
}