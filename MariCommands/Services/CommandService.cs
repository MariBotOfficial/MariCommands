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

        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        /// <param name="provider">An <see cref="IServiceProvider" />.</param>
        public CommandService(IServiceProvider provider)
        {
            _provider = provider;
            _loggerFactory = _provider.GetOrDefault<ILoggerFactory, LoggerFactory>();
        }

        /// <inheritdoc />
        public Task AddModuleAsync<T>(T type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddModuleAsync<T>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddModuleAsync(Type type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddModulesAsync(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(string command, string[] args)
        {
            throw new NotImplementedException();
        }
    }
}