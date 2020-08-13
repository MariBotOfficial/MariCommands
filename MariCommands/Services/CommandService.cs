using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using MariCommands.Builder;
using MariCommands.Results;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandService : ICommandService
    {
        private readonly IContextExecutor _contextExecutor;

        private readonly IModuleConfigurer _moduleConfigurer;

        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        public CommandService(IContextExecutor contextExecutor, IModuleConfigurer moduleConfigurer)
        {
            _contextExecutor = contextExecutor;
            _moduleConfigurer = moduleConfigurer;
        }

        /// <inheritdoc />
        public void RemoveModule(IModule module)
            => _moduleConfigurer.RemoveModule(module);

        /// <inheritdoc />
        public IModule AddModule(IModule module)
            => _moduleConfigurer.AddModule(module);

        /// <inheritdoc />
        public IModule AddModule(Type type)
            => _moduleConfigurer.AddModule(type);

        /// <inheritdoc />
        public IReadOnlyCollection<IModule> AddModules(Assembly assembly)
            => _moduleConfigurer.AddModules(assembly);

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