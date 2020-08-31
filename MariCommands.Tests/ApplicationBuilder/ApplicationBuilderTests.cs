using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Middlewares;
using MariCommands.Results;
using MariCommands.Utils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MariCommands.Tests.ApplicationBuilder
{
    public class ApplicationBuilderTests
    {
        [Fact]
        public void CanBuildApplication()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.UseDefaultMiddlewares();

            var cmdDelegate = app.Build();
        }

        [Fact]
        public async Task CanExecuteBuildedApplication()
        {
            var provider = new ServiceCollection()
                                .AddLogging()
                                .AddSingleton<ICommandServiceOptions, CommandServiceOptions>()
                                .AddSingleton<IModuleCache, ModuleCache>()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.UseDefaultMiddlewares();

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                RawArgs = "someRandomText",
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await cmdDelegate(context);

            Assert.NotNull(context.Result);
            Assert.IsType<CommandNotFoundResult>(context.Result);
        }

        [Fact]
        public async Task ThrowsExceptionForNoResultAndNoCommand()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
            {
                return cmdDelegate(context);
            });
        }

        [Fact]
        public async Task ThrowsExceptionForNoResult()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                Command = new Mock<ICommand>().Object,
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
            {
                return cmdDelegate(context);
            });
        }

        [Fact]
        public async Task ThrowsExceptionForNoCommand()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                Result = new Mock<IResult>().Object,
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
            {
                return cmdDelegate(context);
            });
        }

        [Fact]
        public async Task NotThrowExceptionForCommandAndResult()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                Result = new Mock<IResult>().Object,
                Command = new Mock<ICommand>().Object,
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await cmdDelegate(context);
        }

        [Fact]
        public async Task MiddlewareCanShortCircuit()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.Use((next) =>
            {
                return context =>
                {
                    return Task.CompletedTask;
                };
            });

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await cmdDelegate(context);

            Assert.Null(context.Result);
            Assert.Null(context.Command);
        }

        [Fact]
        public async Task MiddlewaresWillBeBuildedInCorrectOrder()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            var logKey = "LOG";

            var log1 = "Start middleware 1";
            var log2 = "Finish middleware 1";
            var log3 = "Start middleware 2";
            var log4 = "Finish middleware 2";
            var log5 = "Start middleware 3";
            var log6 = "Finish middleware 3";

            app.Use((next) =>
            {
                return async context =>
                {
                    var logs = context.Items[logKey] as List<string>;

                    logs.Add(log1);

                    await next(context);

                    logs.Add(log2);
                };
            });

            app.Use((next) =>
            {
                return async context =>
                {
                    var logs = context.Items[logKey] as List<string>;

                    logs.Add(log3);

                    await next(context);

                    logs.Add(log4);
                };
            });

            app.Use((next) =>
            {
                return async context =>
                {
                    var logs = context.Items[logKey] as List<string>;

                    logs.Add(log5);

                    await next(context);

                    logs.Add(log6);
                };
            });


            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                Result = new Mock<IResult>().Object,
                Command = new Mock<ICommand>().Object,
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            context.Items[logKey] = new List<string>();

            await cmdDelegate(context);

            var logs = context.Items[logKey] as List<string>;

            Assert.NotNull(context.Result);
            Assert.NotNull(context.Command);
            Assert.Equal(logs[0], log1);
            Assert.Equal(logs[1], log3);
            Assert.Equal(logs[2], log5);
            Assert.Equal(logs[3], log6);
            Assert.Equal(logs[4], log4);
            Assert.Equal(logs[5], log2);
        }

        [Fact]
        public async Task CanAddAnICommandMiddleware()
        {
            var provider = new ServiceCollection()
                                .BuildServiceProvider(true);

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.UseMiddleware<TestMiddleware>();

            var cmdDelegate = app.Build();

            var context = new CommandContext()
            {
                Result = new Mock<IResult>().Object,
                Command = new Mock<ICommand>().Object,
                ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>(),
            };

            await cmdDelegate(context);

            var item = context.Items[TestMiddleware.ItemKey] as string;

            Assert.False(string.IsNullOrWhiteSpace(item));
            Assert.Equal(TestMiddleware.ItemValue, item);
        }
    }

    public class TestMiddleware : ICommandMiddleware
    {
        public const string ItemKey = "itemKey";

        public const string ItemValue = "itemValue";

        public Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.Items[ItemKey] = ItemValue;

            return next(context);
        }
    }
}