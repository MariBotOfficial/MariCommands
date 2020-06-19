using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariGlobals.Extensions;

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

                return await ExecuteAsync(match.Command, match.RemainingInput, commandContext)
                                .ConfigureAwait(false);
            }
            else
            {
                var hasBestMatch = matches
                                        .Any(a =>
                                            a.Command.Module.GetMatchHandling(_config) == MultiMatchHandling.Best);

                if (!hasBestMatch)
                    return MultiMatchErrorResult.FromMatches(matches);

                var bestMatches = new List<ICommandMatch>();

                foreach (var match in matches)
                {
                    var inputCount = match.RemainingInput.Split(_config.Separator).Count();

                    var command = match.Command;

                    if (command.Parameters.Count > inputCount)
                    {
                        var hasAnyOptional = command.Parameters.Any(a => IsOptional(a));

                        if (!hasAnyOptional)
                            continue;

                        var optionalsCount = command.Parameters.Count(a => IsOptional(a));

                        var missingCount = command.Parameters.Count - inputCount;

                        if (optionalsCount < missingCount)
                            continue;

                        bestMatches.Add(match);
                    }
                    else
                    {

                    }
                }

                return default;
            }
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
    }
}