using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands
{
    /// <inheritdoc />
    public class ModuleCache : IModuleCache
    {
        private readonly ConcurrentDictionary<string, List<ICommand>> _commands;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleCache" />
        /// </summary>
        public ModuleCache(ICommandServiceOptions config)
        {
            _config = config;
            _commands = new ConcurrentDictionary<string, List<ICommand>>(StringComparer.FromComparison(_config.Comparison));
        }

        /// <inheritdoc />
        public void AddModule(IModule module)
        {
            module.NotNull(nameof(module));

            var lastNestedModule = GetLastNestedModule(module);

            AddModuleInternal(lastNestedModule, string.Empty);
        }

        private IModule GetLastNestedModule(IModule module)
        {
            var parent = module;

            while (module.Parent.HasContent())
                parent = parent.Parent;

            return parent;
        }

        private void AddModuleInternal(IModule module, string parentAlias)
        {
            if (module.Aliases.HasNoContent())
            {
                AddModuleWithPath(module, parentAlias);
            }
            else
            {
                foreach (var alias in module.Aliases)
                {
                    var path = $"{parentAlias}{_config.Separator}{alias}";

                    AddModuleWithPath(module, path);
                }
            }
        }

        private void AddModuleWithPath(IModule module, string path)
        {
            foreach (var subModule in module.Submodules)
                AddModuleInternal(subModule, path);

            foreach (var command in module.Commands)
                AddCommandInternal(command, path);
        }

        private void AddCommandInternal(ICommand command, string moduleAlias)
        {
            foreach (var alias in command.Aliases)
            {
                var path = $"{moduleAlias}{_config.Separator}{alias}";

                if (_commands.TryGetValue(path, out var commands))
                {
                    commands.Add(command);
                }
                else
                {
                    _commands.TryAdd(path, new List<ICommand> { command });
                }
            }
        }

        /// <inheritdoc />
        public void RemoveModule(IModule module)
        {
            var lastNestedModule = GetLastNestedModule(module);

            RemoveModuleInternal(lastNestedModule, string.Empty);
        }

        private void RemoveModuleInternal(IModule module, string parentAlias)
        {
            if (module.Aliases.HasNoContent())
            {
                RemoveModuleWithPath(module, parentAlias);
            }
            else
            {
                foreach (var alias in module.Aliases)
                {
                    var path = $"{parentAlias}{_config.Separator}{alias}";

                    RemoveModuleWithPath(module, path);
                }
            }
        }

        private void RemoveModuleWithPath(IModule module, string path)
        {
            foreach (var subModule in module.Submodules)
                RemoveModuleInternal(subModule, path);

            foreach (var command in module.Commands)
                RemoveCommandInternal(command, path);
        }

        private void RemoveCommandInternal(ICommand command, string moduleAlias)
        {
            foreach (var alias in command.Aliases)
            {
                var path = $"{moduleAlias}{_config.Separator}{alias}";

                _commands.TryRemove(path, out var _);
            }
        }

        /// <inheritdoc />
        public ValueTask<IReadOnlyCollection<ICommandMatch>> SearchCommandsAsync(string input)
        {
            input.NotNullOrEmpty(nameof(input));

            var paths = input.Split(_config.Separator);

            var fullPath = string.Empty;

            var matches = new List<CommandMatch>();

            for (var i = 0; i < paths.Count(); i++)
            {
                if (i == 0)
                {
                    fullPath += paths[i];
                }
                else
                {
                    fullPath += $"{_config.Separator}{paths[i]}";
                }

                if (_commands.TryGetValue(fullPath, out var commands))
                {
                    var remainingInput = string.Join(_config.Separator, paths.Skip(i + 1));

                    matches.AddRange(commands
                                    .Select(a => new CommandMatch(a, fullPath, input, remainingInput))
                                    .ToList());
                }
            }

            return new ValueTask<IReadOnlyCollection<ICommandMatch>>(matches);
        }
    }
}