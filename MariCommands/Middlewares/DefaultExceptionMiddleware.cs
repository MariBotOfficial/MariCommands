using System;
using System.Threading.Tasks;
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
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}