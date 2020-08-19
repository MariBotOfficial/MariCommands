using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using MariCommands.Features;
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

            }
        }

        private void ClearCommandContext(CommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}