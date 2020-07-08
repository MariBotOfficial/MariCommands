using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MariCommands
{
    /// <inheritdoc />
    public class CommandService : ICommandService
    {
        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        public CommandService()
        {
        }

        private CommandDelegate CommandDelegate { get; set; }

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
        public async Task<IResult> ExecuteAsync(string input, CommandContext commandContext)
        {
            commandContext.RawArgs = input;

            await CommandDelegate(commandContext);

            return commandContext.Result;
        }

        /// <inheritdoc />
        public async Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext commandContext)
        {
            commandContext.Command = command;
            commandContext.RawArgs = args;

            await CommandDelegate(commandContext);

            return commandContext.Result;
        }

        /// <inheritdoc />
        public async Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext commandContext)
        {
            commandContext.Command = command;
            commandContext.Args = args.ToList();

            await CommandDelegate(commandContext);

            return commandContext.Result;
        }

        /// <inheritdoc />
        public void Initialize(CommandDelegate commandDelegate)
        {
            if (CommandDelegate.HasContent())
                throw new InvalidOperationException("The current application has already started.");

            commandDelegate.NotNull(nameof(commandDelegate));

            CommandDelegate = commandDelegate;
        }

        private void CheckInitialized()
        {
            if (CommandDelegate.HasNoContent())
                throw new InvalidOperationException("The current application has not started yet.");
        }
    }
}