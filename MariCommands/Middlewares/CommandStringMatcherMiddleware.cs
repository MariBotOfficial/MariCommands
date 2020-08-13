using System.Linq;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Results;
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

            matches = matches
                        .Where(a => a.Command.IsEnabled)
                        .ToList();

            _logger.LogDebug($"Total matches: {matches.Count}.");

            if (matches.HasNoContent())
            {
                _logger.LogInformation($"Don't find any commands that matches the input {input}.");
                context.Result = CommandNotFoundResult.FromInput(input);

                return;
            }

            context.Features.Set<ICommandMatchesFeature>(new CommandMatchesFeature());
            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            matchesFeature.CommandMatches = matches;

            await next(context);

            return;
        }
    }
}