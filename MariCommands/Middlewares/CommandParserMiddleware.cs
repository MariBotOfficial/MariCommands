using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class CommandParserMiddleware : ICommandMiddleware
    {
        private readonly ILogger _logger;

        public CommandParserMiddleware(ILogger<CommandParserMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Result.HasContent())
            {
                await next(context);
            }
            else if (context.Items.ContainsKey(MiddlewareUtils.COMMAND_MATCH))
            {
                var result = await ParseCommandFromMatchAsync(context);

                await ValidateResultAsync(result, context, next);
            }
            else if (context.Command.HasContent())
            {
                await next(context);
            }
            else
            {
                await ParseCommandFromMatchesAsync(context, next);
            }
        }

        private async Task ValidateResultAsync(IResult result, CommandContext context, CommandDelegate next)
        {
            if (result.Success)
            {
                await next(context);
                return;
            }

            context.Result = result;
        }

        private Task<IResult> ParseCommandFromMatchAsync(CommandContext context)
        {
            var commandMatch = context.Items[MiddlewareUtils.COMMAND_MATCH] as ICommandMatch;

            return ParseCommandFromMatchAsync(context, commandMatch);
        }

        private async Task<IResult> ParseCommandFromMatchAsync(CommandContext context, ICommandMatch match)
        {
            var provider = context.CommandServices;

            IArgumentParser argumentParser;
            var argumentParserType = match.Command.GetArgumentParserType();

            if (argumentParserType.HasContent())
                argumentParser = ActivatorUtilities.GetServiceOrCreateInstance(provider, argumentParserType) as IArgumentParser;
            else
                argumentParser = provider.GetRequiredService<IArgumentParser>();

            return await argumentParser.ParseAsync(context);
        }


        private async Task ParseCommandFromMatchesAsync(CommandContext context, CommandDelegate next)
        {
            var commandMatches = context.Items[MiddlewareUtils.COMMAND_MATCHES] as IReadOnlyCollection<ICommandMatch>;

            var bestMatches = new List<ICommandMatch>();

            var fails = new Dictionary<ICommand, IResult>();

            foreach (var match in commandMatches)
            {
                var result = await ParseCommandFromMatchAsync(context, match);

                if (result.Success)
                {
                    bestMatches.Add(match);
                }
                else
                {
                    fails.Add(match.Command, result);
                }
            }


            if (bestMatches.HasNoContent())
            {
                _logger.LogInformation("All matched commands returned fail for command parsing.");
                context.Result = MatchesFailedResult.FromFaileds(fails);

                return;
            }
            else if (bestMatches.Count == 1)
            {
                var match = bestMatches.FirstOrDefault();

                if (!MiddlewareUtils.VerifyMatchDisabled(context, match, _logger))
                    return;

                context.Items[MiddlewareUtils.COMMAND_MATCH] = match;
                context.Command = match.Command;
            }
            else
            {
                context.Items[MiddlewareUtils.COMMAND_MATCHES] = bestMatches as IReadOnlyCollection<ICommandMatch>;
            }

            await next(context);
        }
    }
}