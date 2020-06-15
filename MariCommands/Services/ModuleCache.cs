using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MariGlobals.Extensions;

namespace MariCommands.Services
{
    /// <inheritdoc />
    public class ModuleCache : IModuleCache
    {
        private readonly IServiceProvider _provider;
        private readonly ConcurrentDictionary<string, ICommand> _commands;
        private readonly ICommandServiceOptions _config;

        /// <summary>
        /// Creates a new instance of <see cref="ModuleCache" />
        /// </summary>
        public ModuleCache(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _config = _provider.GetOrDefault<ICommandServiceOptions, CommandServiceOptions>();
            _commands = new ConcurrentDictionary<string, ICommand>(StringComparer.FromComparison(_config.Comparison));
        }

        /// <inheritdoc />
        public bool AddModule(IModule module)
        {
            if (module.HasNoContent())
                return false;

            return true;
        }

        /// <inheritdoc />
        public bool RemoveModule(IModule module)
        {
            if (module.HasNoContent())
                return false;

            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask<ICommandMatch> SearchCommandsAsync(string input)
        {
            throw new System.NotImplementedException();
        }
    }
}