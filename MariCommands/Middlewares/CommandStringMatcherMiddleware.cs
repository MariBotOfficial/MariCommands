using System.Linq;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class CommandStringMatcherMiddleware : ICommandMiddleware
    {
        private readonly IModuleCache _moduleCache;
        private readonly ILogger _logger;

        public CommandStringMatcherMiddleware(IModuleCache moduleCache, ILogger<CommandStringMatcherMiddleware> logger)
        {
            _moduleCache = moduleCache;
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Command.HasContent() || context.Result.HasContent())
            {
                await next(context);
                return;
            }

            context.RawArgs.NotNullOrWhiteSpace(nameof(context.RawArgs));
            var input = context.RawArgs;

            var matches = await _moduleCache.SearchCommandsAsync(input);

            _logger.LogDebug($"Total matches: {matches.Count}.");

            if (matches.HasNoContent())
            {
                _logger.LogInformation($"Don't find any commands that matches the input {input}.");
                context.Result = CommandNotFoundResult.FromInput(input);

                return;
            }

            if (matches.Count == 1)
            {
                var match = matches.FirstOrDefault();

                if (!MiddlewareUtils.VerifyMatchDisabled(context, match, _logger))
                    return;

                context.Command = match.Command;
                context.Items.Add(MiddlewareUtils.COMMAND_MATCH, match);
                await next(context);

                return;
            }

            context.Items.Add(MiddlewareUtils.COMMAND_MATCHES, matches);

            await next(context);

            return;
        }
    }
}