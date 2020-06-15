using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandService : ICommandService
    {
        private readonly IServiceProvider _provider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IModuleBuilder _moduleBuilder;
        private readonly ICommandExecutor _commandExecutor;

        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public CommandService(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _loggerFactory = _provider.GetOrDefault<ILoggerFactory, LoggerFactory>();
            // TODO: _moduleBuilder = _provider.GetOrDefault<IModuleFactory, ModuleFactory>();
            // TODO: _commandExecutor = _provider.GetOrDefault<ICommandExecutor, CommandExecutor>();
        }

        /// <inheritdoc />
        public IModule AddModule<T>(T type) where T : IModule
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModule AddModule<T>() where T : IModule
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IModule AddModule(Type type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IModule> AddModules(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command)
            => _commandExecutor.ExecuteAsync(command);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command, string[] args)
            => _commandExecutor.ExecuteAsync(command, args);
    }
}