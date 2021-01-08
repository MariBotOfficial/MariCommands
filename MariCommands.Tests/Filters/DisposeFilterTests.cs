using System;
using System.Threading.Tasks;
using MariCommands.Filters;
using MariCommands.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace MariCommands.Tests.Filters
{
    public class DisposeFilterTests
    {
        [Fact]
        public async Task Should_Dispose_When_MariCommandsOptions_AutoDisposeContext_Is_True()
        {
            // Act
            var scopeFactory = new ServiceCollection()
                                    .AddSingleton<IOptions<MariCommandsOptions>>(new MariCommandsOptions
                                    {
                                        AutoDisposeContext = true,
                                    })
                                    .BuildServiceProvider()
                                    .GetRequiredService<IServiceScopeFactory>();

            var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory
            };

            var filter = new DisposeFilter();

            var resultContext = new CommandResultContext(context, new SuccessResult());

            static Task resultDelegate(CommandResultContext ctx) => Task.CompletedTask;

            // Arrange
            await filter.InvokeAsync(resultContext, resultDelegate);

            // Arrange
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = context.Features;
            });
        }

        [Fact]
        public async Task Shouldnt_Dispose_When_MariCommandsOptions_AutoDisposeContext_Is_False()
        {
            // Act
            var scopeFactory = new ServiceCollection()
                                    .AddSingleton<IOptions<MariCommandsOptions>>(new MariCommandsOptions
                                    {
                                        AutoDisposeContext = false,
                                    })
                                    .BuildServiceProvider()
                                    .GetRequiredService<IServiceScopeFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory
            };

            var filter = new DisposeFilter();

            var resultContext = new CommandResultContext(context, new SuccessResult());

            static Task resultDelegate(CommandResultContext ctx) => Task.CompletedTask;

            // Arrange
            await filter.InvokeAsync(resultContext, resultDelegate);

            // Arrange
            _ = context.Features;
        }

        [Fact]
        public async Task Shouldnt_Dispose_When_MariCommandsOptions_Is_Null()
        {
            // Act
            var scopeFactory = new ServiceCollection()
                                    .BuildServiceProvider()
                                    .GetRequiredService<IServiceScopeFactory>();

            await using var context = new CommandContext
            {
                ServiceScopeFactory = scopeFactory
            };

            var filter = new DisposeFilter();

            var resultContext = new CommandResultContext(context, new SuccessResult());

            static Task resultDelegate(CommandResultContext ctx) => Task.CompletedTask;

            // Arrange
            await filter.InvokeAsync(resultContext, resultDelegate);

            // Arrange
            _ = context.Features;
        }
    }
}