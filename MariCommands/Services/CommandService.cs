using System;
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
        /// <param name="provider">An <see cref="IServiceProvider" />.</param>
        public CommandService(IServiceProvider provider)
        {
            _provider = provider;
            _loggerFactory = _provider.GetOrDefault<ILoggerFactory, LoggerFactory>();
            // TODO: _moduleBuilder = _provider.GetOrDefault<IModuleBuilder, ModuleBuilder>();
            // TODO: _commandExecutor = _provider.GetOrDefault<ICommandExecutor, CommandExecutor>();
        }

        /// <inheritdoc />
        public Task AddModuleAsync<T>(T type)
            => _moduleBuilder.AddModuleAsync(type);

        /// <inheritdoc />
        public Task AddModuleAsync<T>()
            => _moduleBuilder.AddModuleAsync<T>();

        /// <inheritdoc />
        public Task AddModuleAsync(Type type)
            => _moduleBuilder.AddModuleAsync(type);

        /// <inheritdoc />
        public Task AddModulesAsync(Assembly assembly)
            => _moduleBuilder.AddModulesAsync(assembly);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command)
            => _commandExecutor.ExecuteAsync(command);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command, string[] args)
            => _commandExecutor.ExecuteAsync(command, args);
    }
}