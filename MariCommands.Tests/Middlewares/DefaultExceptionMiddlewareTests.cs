using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Extensions;
using MariCommands.Features;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MariCommands.Tests.Middlewares
{
    public class DefaultExceptionMiddlewareTests
    {
        private async Task ExecuteMiddlewareAsync(CommandContext context, OnLog onLog = null)
        {
            var provider = new ServiceCollection()
                                .AddLogging(logging =>
                                {
                                    if (onLog.HasContent())
                                        logging.AddProvider(new TestLoggerProvider(onLog));
                                })
                                .BuildServiceProvider(true);

            context.ServiceScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            var app = new CommandApplicationBuilder(new Dictionary<string, object>(), provider);

            app.UseDefaultExceptionMiddleware();

            var commandDelegate = app.Build();

            await commandDelegate(context);
        }

        [Fact]
        public async Task RethrowsIfResultIsSetted()
        {
            var context = new CommandContext();

            context.Result = new SuccessResult();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context);
            });
        }

        [Fact]
        public async Task ClearCommandContextOnHandle()
        {
            var context = new CommandContext();

            var match1 = new Mock<ICommandMatch>().Object;
            var match2 = new Mock<ICommandMatch>().Object;
            var objArr1 = new object[0];
            var objArr2 = new object[0];

            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature
            {
                CommandArgs = new Dictionary<ICommandMatch, object[]>
                {
                    {match1, objArr1},
                    {match2, objArr2},
                },
            });

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature
            {
                CommandMatches = new List<ICommandMatch>
                {
                    match1,
                    match2,
                },
            });

            await ExecuteMiddlewareAsync(context);

            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();
            var commandMatchesFeature = context.Features.Get<ICommandMatchesFeature>();

            Assert.NotNull(argumentParserFeature);
            Assert.NotNull(commandMatchesFeature);
            Assert.Empty(argumentParserFeature.CommandArgs);
            Assert.Empty(commandMatchesFeature.CommandMatches);
        }

        [Fact]
        public async Task SetExceptionHandlerFeatureOnHandled()
        {
            var context = new CommandContext();

            await ExecuteMiddlewareAsync(context);

            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

            Assert.NotNull(exceptionHandlerFeature);
            Assert.NotNull(exceptionHandlerFeature.Error);
        }

        [Fact]
        public async Task SetExceptionResultOnHandled()
        {
            var context = new CommandContext();

            await ExecuteMiddlewareAsync(context);

            Assert.NotNull(context.Result);
            Assert.False(context.Result.Success);
            Assert.False(string.IsNullOrWhiteSpace(context.Result.Reason));
            Assert.NotNull(context.Result.Exception);
            Assert.IsType<ExceptionResult>(context.Result);
        }

        [Fact]
        public async Task RethrowsIfCantHandleException()
        {
            var context = new CommandContext();

            OnLog onLog = (logLevel, _, __, ___) =>
            {
                if (logLevel == LogLevel.Error)
                {
                    try
                    {
                        // Will throw exception when try access features.
                        context.DisposeAsync().GetAwaiter().GetResult();
                    }
                    catch { }
                }
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, onLog);
            });
        }

        [Fact]
        public async Task WriteUnhandledExceptionOnExceptionCaptured()
        {
            var unhandledException = false;

            var context = new CommandContext();

            OnLog onLog = (logLevel, eventId, exception, message) =>
            {
                var isUnhandledException =
                    logLevel == LogLevel.Error &&
                    eventId.Id == 1 &&
                    eventId.Name.Equals("UnhandledException") &&
                    message.Equals("An unhandled exception has occurred while executing the request.") &&
                    exception.HasContent();

                if (isUnhandledException)
                    unhandledException = true;
            };

            await ExecuteMiddlewareAsync(context, onLog);

            Assert.True(unhandledException);
        }

        [Fact]
        public async Task WriteResultSettedErrorHandlerOnContextWithResult()
        {
            var resultSetted = false;

            var context = new CommandContext();

            OnLog onLog = (logLevel, eventId, exception, message) =>
            {
                var isResultSetted =
                    logLevel == LogLevel.Warning &&
                    eventId.Id == 2 &&
                    eventId.Name.Equals("ResultSetted") &&
                    message.Equals("The result has already setted, the error handler will not be executed.") &&
                    exception.HasNoContent();

                if (isResultSetted)
                    resultSetted = true;
            };

            context.Result = new SuccessResult();

            try
            {
                await ExecuteMiddlewareAsync(context, onLog);
            }
            catch { }

            Assert.True(resultSetted);
        }

        [Fact]
        public async Task WriteErrorHandlerExceptionOnFailure()
        {
            var failed = false;

            var context = new CommandContext();

            OnLog onLog = (logLevel, eventId, exception, message) =>
            {
                if (logLevel == LogLevel.Error)
                {
                    try
                    {
                        // Will throw exception when try access features.
                        context.DisposeAsync().GetAwaiter().GetResult();
                    }
                    catch { }
                }

                var isFailed =
                    logLevel == LogLevel.Error &&
                    eventId.Id == 3 &&
                    eventId.Name.Equals("Exception") &&
                    message.Equals("An exception was thrown attempting to execute the error handler.") &&
                    exception.HasContent() &&
                    exception.OfType<ObjectDisposedException>();

                if (isFailed)
                    failed = true;
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await ExecuteMiddlewareAsync(context, onLog);
            });

            Assert.True(failed);
        }
    }

    public delegate void OnLog(LogLevel logLevel, EventId eventId, Exception exception, string message);

    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly OnLog _onLog;

        public TestLoggerProvider(OnLog onLog)
        {
            _onLog = onLog;
        }

        public ILogger CreateLogger(string categoryName)
            => new TestLogger(_onLog);

        public void Dispose()
        { }
    }

    public class TestLogger : ILogger
    {
        private readonly OnLog _onLog;

        public TestLogger(OnLog onLog)
        {
            _onLog = onLog;
        }

        public IDisposable BeginScope<TState>(TState state)
            => default;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            _onLog(logLevel, eventId, exception, message);
        }
    }
}