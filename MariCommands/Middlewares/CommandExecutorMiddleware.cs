using System;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Features;
using MariCommands.Filters;
using MariCommands.Results;
using MariCommands.Utils;
using MariGlobals.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MariCommands.Middlewares
{
    internal sealed class CommandExecutorMiddleware : ICommandMiddleware
    {
        public async Task InvokeAsync(CommandContext context, CommandDelegate next)
        {
            context.NotNull(nameof(context));

            if (context.Result.HasContent())
                return;

            var options = context.CommandServices.GetRequiredService<IOptions<MariCommandsOptions>>().Value;

            var (command, args) = GetCommandAndArgs(context);

            var module = GetModule(context, command) as IModuleBase;

            module.SetContext(context);

            var runMode = command.GetRunMode(options);

            IResult result;

            if (runMode == RunMode.Sequential)
            {
                result = await ExecuteSequentialAsync(context, command, module, args);
            }
            else
            {
                result = ExecuteConcurrent(context, command, module, args);
            }

            context.Result = result;
        }

        private async Task<IResult> ExecuteSequentialAsync(CommandContext context, ICommand command, IModuleBase module, object[] args)
        {
            await module.OnCommandExecutingAsync();

            var result = await command.Executor.ExecuteAsync(module, args);

            await module.OnCommandExecutedAsync();

            var filterProvider = context.CommandServices.GetRequiredService<IFilterProvider>();

            await filterProvider.InvokeFiltersAsync<CommandResultContext, ICommandResultFilter>(new CommandResultContext(context, result));

            return result;
        }

        private IResult ExecuteConcurrent(CommandContext context, ICommand command, IModuleBase module, object[] args)
        {
            var tsc = new TaskCompletionSource<IResult>();

            _ = Task.Run(() =>
            {
                return ExecuteSequentialAsync(context, command, module, args);
            }).ContinueWith(async task =>
            {
                IResult result = null;

                try
                {
                    result = await task;
                }
                catch (Exception ex)
                {
                    result = ExceptionResult.FromException(ex);
                }
                finally
                {
                    tsc.SetResult(result);
                }
            });

            return AsyncResult.FromTsc(tsc);
        }

        private object GetModule(CommandContext context, ICommand command)
        {
            var module = command.Module.Invoker.CreateInstance(context.CommandServices);

            MiddlewareUtils.RegisterForDispose(module, context);

            return module;
        }

        private (ICommand, object[]) GetCommandAndArgs(CommandContext context)
        {
            if (context.Command.HasContent() && context.Args != null)
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