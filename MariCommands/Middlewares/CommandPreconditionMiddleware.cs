using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Results;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.Logging;

namespace MariCommands.Middlewares
{
    internal sealed class CommandPreconditionMiddleware : ICommandMiddleware
    {
        private readonly ILogger _logger;

        public CommandPreconditionMiddleware(ILogger<CommandPreconditionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Result.HasContent())
            {
                await next(context);
                return;
            }

            if (context.Command.HasContent())
            {
                var result = await ExecutePreconditionsForContextAsync(context);

                if (result.Success)
                    await next(context);
                else
                    context.Result = result;

                return;
            }

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            if (matchesFeature.HasNoContent() || matchesFeature.CommandMatches.HasNoContent())
                throw new InvalidOperationException($"Can't get command matches from feature: {nameof(ICommandMatchesFeature)}.");

            var matches = matchesFeature.CommandMatches;

            var fails = new Dictionary<ICommand, IResult>();
            var bestMatches = new List<ICommandMatch>();

            foreach (var match in matches)
            {
                var command = match.Command;

                var result = await ExecutePreconditionsAsync(command, context);

                if (result.Success)
                    bestMatches.Add(match);
                else
                    fails.Add(command, result);
            }

            if (bestMatches.HasNoContent())
            {
                _logger.LogInformation("All matched commands returned fail for input count.");
                context.Result = MiddlewareUtils.GetErrorResult(fails);

                return;
            }

            matchesFeature.CommandMatches = bestMatches;

            await next(context);
        }

        private Task<IResult> ExecutePreconditionsForContextAsync(CommandContext context)
             => ExecutePreconditionsAsync(context.Command, context);

        private async Task<IResult> ExecutePreconditionsAsync(ICommand command, CommandContext context)
        {
            var preconditions = command.GetAllPreconditions();

            var tasks = preconditions
                            .Select(a => ExecutePreconditionAsync(a, command, context));

            var results = await Task.WhenAll(tasks);

            var fail = results.Any(a => !a.Result.Success);

            if (!fail)
                return new SuccessResult();

            var fails = results
                            .Where(a => !a.Result.Success)
                            .ToList();

            return PreconditionsFailResult.FromFaileds(command, fails);
        }

        private async Task<(PreconditionAttribute, IPreconditionResult Result)> ExecutePreconditionAsync(
            PreconditionAttribute attribute,
            ICommand command,
            CommandContext context)
        {
            var result = await attribute.ExecuteAsync(command, context);

            return (attribute, result);
        }
    }
}