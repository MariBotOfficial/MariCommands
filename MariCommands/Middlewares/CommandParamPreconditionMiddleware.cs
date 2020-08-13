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
    internal sealed class CommandParamPreconditionMiddleware : ICommandMiddleware
    {
        private readonly ILogger _logger;

        public CommandParamPreconditionMiddleware(ILogger<CommandParamPreconditionMiddleware> logger)
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

            var isUniqueCommand =
                context.Command.HasContent() &&
                context.Args != null;

            if (isUniqueCommand)
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

            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            if (argumentParserFeature.HasNoContent() || argumentParserFeature.CommandArgs.HasNoContent())
                throw new InvalidOperationException($"Can't get command matches parsed param values from feature: {nameof(IArgumentParserFeature)}.");

            var matches = matchesFeature.CommandMatches;
            var parsedArgs = argumentParserFeature.CommandArgs;

            var fails = new Dictionary<ICommand, IResult>();
            var bestMatches = new List<ICommandMatch>();

            foreach (var match in matches)
            {
                var command = match.Command;
                var args = parsedArgs[match];

                var result = await ExecutePreconditionsAsync(command, args, context);

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
            => ExecutePreconditionsAsync(context.Command, context.Args, context);

        public async Task<IResult> ExecutePreconditionsAsync(ICommand command, IReadOnlyCollection<object> args, CommandContext context)
        {
            var count = command.Parameters.Count == args.Count
                ? command.Parameters.Count
                : args.Count;

            if (count == 0)
                return new SuccessResult();

            for (var i = 0; i < count; i++)
            {
                var param = command.Parameters.ElementAt(i);
                var parsedArg = args.ElementAt(i);

                var result = await ExecutePreconditionsAsync(param, parsedArg, context);

                if (!result.Success)
                    return result;
            }

            return new SuccessResult();
        }

        private async Task<IResult> ExecutePreconditionsAsync(IParameter param, object value, CommandContext context)
        {
            var preconditions = param.Preconditions;

            var tasks = preconditions
                            .Select(a => ExecutePreconditionAsync(a, value, param, context));

            var results = await Task.WhenAll(tasks);

            var fail = results.Any(a => !a.Result.Success);

            if (!fail)
                return new SuccessResult();

            var fails = results
                            .Where(a => !a.Result.Success)
                            .ToList();

            return ParamPreconditionsFailResult.FromFaileds(param, value, fails);
        }

        private async Task<(ParamPreconditionAttribute Attribute, IPreconditionResult Result)> ExecutePreconditionAsync(
            ParamPreconditionAttribute attribute,
            object value,
            IParameter parameter,
            CommandContext context)
        {
            var result = await attribute.ExecuteAsync(value, parameter, context);

            return (attribute, result);
        }
    }
}