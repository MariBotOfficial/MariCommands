using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Results;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands
{
    /// <inheritdoc />
    public class ContextExecutor : IContextExecutor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Creates a new instance of <see cref="ContextExecutor" />
        /// </summary>
        /// <param name="scopeFactory">the factory used to create services within a scope.</param>
        public ContextExecutor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private CommandDelegate CommandDelegate { get; set; }

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

            context.ServiceScopeFactory = _scopeFactory;

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