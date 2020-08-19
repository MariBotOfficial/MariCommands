using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class DefaultExceptionMiddleware : ICommandMiddleware
    {
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

            HandleException(context, edi);
        }

        private void HandleException(CommandContext context, ExceptionDispatchInfo edi)
        {
            //_logger.UnhandledException(edi.SourceException);

            try
            {
                ClearCommandContext(context);

                var exceptionHandlerFeature = new ExceptionHandlerFeature(edi.SourceException);

                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);

                return;
            }
            catch (Exception ex2)
            {
                //_logger.ErrorHandlerException(ex2);
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