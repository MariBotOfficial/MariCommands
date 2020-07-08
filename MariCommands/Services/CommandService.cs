using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandService : ICommandService
    {
        private readonly IContextExecutor _contextExecutor;

        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        public CommandService(IContextExecutor contextExecutor)
        {
            _contextExecutor = contextExecutor;
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
        public Task<IResult> ExecuteAsync(string input, CommandContext context)
            => _contextExecutor.ExecuteAsync(input, context);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext context)
            => _contextExecutor.ExecuteAsync(command, args, context);

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext context)
            => _contextExecutor.ExecuteAsync(command, args, context);

        /// <inheritdoc />
        public void Initialize(CommandDelegate commandDelegate)
            => _contextExecutor.Initialize(commandDelegate);
    }
}