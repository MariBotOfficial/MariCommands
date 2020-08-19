using System;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Results;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MariCommands.Middlewares
{
    internal sealed class CommandExecutorMiddleware : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Result.HasContent())
            {
                await next(context);
                return;
            }

            var options = context.CommandServices.GetRequiredService<ICommandServiceOptions>();

            var (command, args) = GetCommandAndArgs(context);

            var module = GetModule(context, command) as IModuleBase;

            module.SetContext(context);

            var runMode = command.GetRunMode(options);

            IResult result;

            if (runMode == RunMode.Sequential)
            {
                result = await ExecuteSequentialAsync(command, module, args);
            }
            else
            {
                result = await ExecuteConcurrentAsync(command, module, args);
            }


            context.Result = result;
        }

        private async Task<IResult> ExecuteSequentialAsync(ICommand command, IModuleBase module, object[] args)
        {
            await module.OnCommandExecutingAsync();

            var result = await command.Executor.ExecuteAsync(module, args);

            await module.OnCommandExecutedAsync();

            return result;
        }

        private Task<IResult> ExecuteConcurrentAsync(ICommand command, IModuleBase module, object[] args)
        {
            var task = Task.Run(() =>
            {
                return ExecuteSequentialAsync(command, module, args);
            });

            return Task.FromException<IResult>(new NotImplementedException());
        }

        private object GetModule(CommandContext context, ICommand command)
        {
            var module = context.CommandServices.GetService(command.Module.Type);

            if (module.HasContent())
                return module;

            module = ActivatorUtilities.CreateInstance(context.CommandServices, command.Module.Type);

            if (module is IAsyncDisposable asyncDisposable)
                context.RegisterForDisposeAsync(asyncDisposable);
            else if (module is IDisposable disposable)
                context.RegisterForDispose(disposable);

            return module;
        }

        private (ICommand, object[]) GetCommandAndArgs(CommandContext context)
        {
            if (context.Command.HasContent())
                return (context.Command, context.Args.ToArray());

            var matchesFeature = context.Features.Get<ICommandMatchesFeature>();

            if (matchesFeature.HasNoContent() || matchesFeature.CommandMatches.HasNoContent())
                throw new InvalidOperationException($"Can't get command matches from feature: {nameof(ICommandMatchesFeature)}.");

            var argumentParserFeature = context.Features.Get<IArgumentParserFeature>();

            if (argumentParserFeature.HasNoContent() || argumentParserFeature.CommandArgs.HasNoContent())
                throw new InvalidOperationException($"Can't get command matches parsed param values from feature: {nameof(IArgumentParserFeature)}.");

            var matches = matchesFeature.CommandMatches;

            // We don't have any way to choose between these matches, just catch the first
            // with highest priority.

            var match = matches
                    .OrderByDescending(a => a.Command.Priority)
                    .FirstOrDefault();

            var args = argumentParserFeature.CommandArgs[match];

            context.Command = match.Command;
            context.Alias = match.Alias;
            context.Args = args;

            return (match.Command, args);
        }
    }
}