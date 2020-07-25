using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class CommandInputCountMatcherMiddleware : ICommandMiddleware
    {
        private readonly ILogger _logger;
        private readonly ICommandServiceOptions _config;

        public CommandInputCountMatcherMiddleware(ILogger<CommandInputCountMatcherMiddleware> logger, ICommandServiceOptions config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Command.HasContent() || context.Result.HasContent())
            {
                await next(context);
                return;
            }

            if (!context.Items.TryGetValue(MiddlewareUtils.COMMAND_MATCHES, out var matchesObj))
                throw new InvalidOperationException($"Can't get command matches from items key: {MiddlewareUtils.COMMAND_MATCHES}.");

            var matches = matchesObj as IReadOnlyCollection<ICommandMatch>;

            var hasBestMatch = matches
                                .Any(a =>
                                    a.Command.Module.GetMatchHandling(_config) == MultiMatchHandling.Best);

            if (!hasBestMatch)
            {
                _logger.LogInformation($"This input has more than one match but none of then has {MultiMatchHandling.Best} as configuration.");
                context.Result = MultiMatchErrorResult.FromMatches(matches);
                return;
            }

            matches = matches
                        .OrderBy(a => a.Command.Priority)
                        .ToList();

            var bestMatches = new List<ICommandMatch>();

            var fails = new Dictionary<ICommand, IResult>();

            foreach (var match in matches)
            {
                var inputCount = match.RemainingInput.Split(_config.Separator).Count();

                var command = match.Command;

                if (command.Parameters.Count > inputCount)
                {
                    var hasAnyOptional = command.Parameters.Any(a => IsOptional(a));

                    if (!hasAnyOptional)
                    {
                        fails.Add(command, BadArgCountResult.FromCommand(command));
                        continue;
                    }

                    var optionalsCount = command.Parameters.Count(a => IsOptional(a));

                    var missingCount = command.Parameters.Count - inputCount;

                    if (optionalsCount < missingCount)
                    {
                        fails.Add(command, BadArgCountResult.FromCommand(command));
                        continue;
                    }
                }
                else if (command.Parameters.Count < inputCount)
                {
                    var ignoreExtraArgs = command.GetIgnoreExtraArgs(_config);

                    if (!ignoreExtraArgs)
                    {
                        fails.Add(command, BadArgCountResult.FromCommand(command));
                        continue;
                    }
                }

                bestMatches.Add(match);
            }

            if (bestMatches.HasNoContent())
            {
                _logger.LogInformation("All commands matched returned fail for input count.");
                context.Result = MatchesFailedResult.FromFaileds(fails);
            }

            context.Items[MiddlewareUtils.COMMAND_MATCHES] = bestMatches as IReadOnlyCollection<ICommandMatch>;
            await next(context);
        }

        private bool IsOptional(IParameter parameter)
        {
            return parameter.IsOptional || parameter.DefaultValue.HasContent() || parameter.IsParams;
        }
    }
}