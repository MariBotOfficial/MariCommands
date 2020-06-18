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
        private readonly IModuleFactory _moduleFactory;
        private readonly ICommandExecutor _commandExecutor;

        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        /// <param name="provider">A dependency container.</param>
        public CommandService(IServiceProvider provider)
        {
            _provider = provider ?? ServiceUtils.GetDefaultServiceProvider();
            _loggerFactory = _provider.GetOrDefault<ILoggerFactory, LoggerFactory>();
            _moduleFactory = _provider.GetOrDefault<IModuleFactory>(new ModuleFactory(_provider));
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
        public Task<IResult> ExecuteAsync(string input, CommandContext commandContext)
            => _commandExecutor.ExecuteAsync(input, commandContext);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext commandContext)
            => _commandExecutor.ExecuteAsync(command, args, commandContext);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext commandContext)
            => _commandExecutor.ExecuteAsync(command, args, commandContext);
    }
}