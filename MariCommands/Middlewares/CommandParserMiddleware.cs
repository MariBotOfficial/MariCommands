using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Parsers;
using MariCommands.Results;
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
            else if (context.Command.HasContent())
            {
                await next(context);
            }
            else
            {
                await ParseCommandFromMatchesAsync(context, next);
            }
        }

        private async Task<IArgumentParserResult> ParseCommandFromMatchAsync(CommandContext context, ICommandMatch match)
        {
            var provider = context.CommandServices;

            IArgumentParser argumentParser;
            var argumentParserType = match.Command.GetArgumentParserType();

            if (argumentParserType.HasContent())
                argumentParser = ActivatorUtilities.GetServiceOrCreateInstance(provider, argumentParserType) as IArgumentParser;
            else
                argumentParser = provider.GetRequiredService<IArgumentParser>();


            return await argumentParser.ParseAsync(context, match);
        }


        private async Task ParseCommandFromMatchesAsync(CommandContext context, CommandDelegate next)
        {
            context.Features.Set<IArgumentParserFeature>(new ArgumentParserFeature());

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();
            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            var bestMatches = new List<ICommandMatch>();

            var fails = new Dictionary<ICommand, IResult>();

            foreach (var match in matchesFeature.CommandMatches)
            {
                var result = await ParseCommandFromMatchAsync(context, match);

                if (result.Success)
                {
                    bestMatches.Add(match);
                    argumentParserFeature.CommandArgs.Add(match, result.Args.Values.ToArray());
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

            matchesFeature.CommandMatches = bestMatches;

            await next(context);
        }
    }
}