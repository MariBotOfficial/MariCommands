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
        private readonly IServiceProvider _provider;
        /// <summary>
        /// Creates an instance of <see cref="CommandService" />.
        /// </summary>
        public CommandService(IServiceProvider provider)
        {
            _provider = provider;
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
        public Task<IResult> ExecuteAsync(string input, CommandContext context)
        {
            context.RawArgs = input;

            return ExecuteAsync(context);
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, string args, CommandContext context)
        {
            context.Command = command;
            context.RawArgs = args;

            return ExecuteAsync(context);
        }

        /// <inheritdoc />
        public Task<IResult> ExecuteAsync(ICommand command, IEnumerable<object> args, CommandContext context)
        {
            context.Command = command;
            context.Args = args.ToList();

            return ExecuteAsync(context);
        }

        private async Task<IResult> ExecuteAsync(CommandContext context)
        {
            CheckInitialized();

            var scope = _provider.CreateScope();

            context.ServiceProvider = scope.ServiceProvider;
            context.RegisterForDispose(scope);

            await CommandDelegate(context);

            return context.Result;
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