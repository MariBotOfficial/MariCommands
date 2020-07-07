using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceProvider _provider;
        private readonly IModuleCache _moduleCache;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Creates a new instance of <see cref="CommandExecutor" />
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public CommandExecutor(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _moduleCache = _provider.GetOrDefault<IModuleCache>(new ModuleCache(_provider));
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
        }

        /// <inheritdoc />
        public async Task<IResult> ExecuteAsync(string input, CommandContext commandContext)
        {
            input.NotNullOrWhiteSpace(nameof(input));
            commandContext.NotNull(nameof(commandContext));

            var matches = await _moduleCache.SearchCommandsAsync(input)
                                                        .ConfigureAwait(false);

            if (matches.HasNoContent())
                return CommandNotFoundResult.FromInput(input);

            if (matches.Count == 1)
            {
                var match = matches.FirstOrDefault();

                if (!match.Command.IsEnabled)
                    return CommandDisabledResult.FromCommand(match.Command);

                return await ExecuteAsync(match.Command, match.RemainingInput, commandContext)
                                .ConfigureAwait(false);
            }
            else
            {
                return await HandleMatchesAsync(input, commandContext, matches)
                                .ConfigureAwait(false);
            }
        }

        private async Task<IResult> HandleMatchesAsync(string input, CommandContext commandContext, IReadOnlyCollection<ICommandMatch> matches)
        {
            var hasBestMatch = matches
                                .Any(a =>
                                    a.Command.Module.GetMatchHandling(_config) == MultiMatchHandling.Best);

            if (!hasBestMatch)
                return MultiMatchErrorResult.FromMatches(matches);

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
                return MatchesFailedResult.FromFaileds(fails);
            }

            return await HandleMatchesFromPreconditions(input, commandContext, bestMatches)
                            .ConfigureAwait(false);
        }

        private async Task<IResult> HandleMatchesFromPreconditions(string input, CommandContext commandContext, IReadOnlyCollection<ICommandMatch> matches)
        {
            await Task.Delay(0);
            foreach (var match in matches)
            {
                // Set command match.
            }

            return null;
        }

        private async Task<IResult> HandleMatchesFromParser(string input, CommandContext commandContext, IReadOnlyCollection<ICommandMatch> matches)
        {
            foreach (var match in matches)
            {
                var parser = GetParser(match);

                await parser.ParseAsync(commandContext);
            }


            return null;
        }

        private bool IsOptional(IParameter parameter)
        {
            return parameter.IsOptional || parameter.DefaultValue.HasContent() || parameter.IsParams;
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext commandContext)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext commandContext)
        {
            throw new System.NotImplementedException();
        }

        private IArgumentParser GetParser(ICommandMatch match)
        {
            var type = match.Command.GetArgumentParserType();

            IArgumentParser parser = null;

            if (type.HasContent())
                parser = ActivatorUtilities.GetServiceOrCreateInstance(_provider, type) as IArgumentParser;

            if (parser.HasNoContent())
                parser = _provider.GetOrDefault<IArgumentParser>(new ArgumentParser(_provider));

            return parser;
        }
    }
}