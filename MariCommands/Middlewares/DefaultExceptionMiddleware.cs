using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Filters;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class DefaultExceptionMiddleware : ICommandMiddleware
    {
        private static readonly Action<ILogger, Exception> _unhandledException =
            LoggerMessage.Define(LogLevel.Error, new EventId(1, "UnhandledException"), "An unhandled exception has occurred while executing the request.");

        private static readonly Action<ILogger, Exception> _errorHandlerException =
            LoggerMessage.Define(LogLevel.Error, new EventId(3, "Exception"), "An exception was thrown attempting to execute the error handler.");

        private static readonly Action<ILogger, Exception> _resultSettedErrorHandler =
        LoggerMessage.Define(LogLevel.Warning, new EventId(2, "ResultSetted"), "The result has already setted, the error handler will not be executed.");

        private readonly ILogger _logger;

        public DefaultExceptionMiddleware(ILogger<DefaultExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            ExceptionDispatchInfo edi;

            try
            {
                await next(context);

                return;
            }
            catch (Exception ex)
            {
                edi = ExceptionDispatchInfo.Capture(ex);
            }

            await HandleExceptionAsync(context, edi);
        }

        private async Task HandleExceptionAsync(CommandContext context, ExceptionDispatchInfo edi)
        {
            _unhandledException(_logger, edi.SourceException);

            if (context.Result.HasContent())
            {
                _resultSettedErrorHandler(_logger, null);
                edi.Throw();
            }

            try
            {
                ClearCommandContext(context);

                var exceptionHandlerFeature = new ExceptionHandlerFeature(edi.SourceException);

                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

                var filterProvider = context.CommandServices.GetRequiredService<IFilterProvider>();

                await filterProvider.InvokeFiltersAsync<CommandExceptionContext, ICommandExceptionFilter>(new CommandExceptionContext(context, edi));

                return;
            }
            catch (Exception ex2)
            {
                _errorHandlerException(_logger, ex2);
            }
            finally
            {
                context.Result = ExceptionResult.FromException(edi.SourceException);
            }

            edi.Throw();
        }

        private void ClearCommandContext(CommandContext context)
        {
            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();
            var commandMatchesFeature = context.Features.Get<ICommandMatchesFeature>();

            argumentParserFeature?.CommandArgs?.Clear();

            if (commandMatchesFeature.HasContent())
            {
                commandMatchesFeature.CommandMatches = new List<ICommandMatch>(0);
            }
        }
    }
}