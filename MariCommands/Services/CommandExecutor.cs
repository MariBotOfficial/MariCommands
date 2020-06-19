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

            var matches = await _moduleCache.SearchCommandsAsync(input);

            if (matches.HasNoContent())
                return CommandNotFoundResult.FromInput(input);

            if (matches.Count == 1 && !matches.FirstOrDefault().Command.IsEnabled)
            {
                return CommandNotFoundResult.FromInput(input);
            }
            else if (matches.Count == 1)
            {
                var match = matches.FirstOrDefault();

                return await ExecuteAsync(match.Command, match.RemainingInput, commandContext);
            }
            else
            {
                var hasBestMatch = matches
                                        .Any(a =>
                                            a.Command.Module.GetMatchHandling(_config) == MultiMatchHandling.Best);

                if (!hasBestMatch)
                    return MultiMatchErrorResult.FromMatches(matches);

                foreach (var match in matches)
                {

                }

                return default;
            }
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