using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariCommands.Results;
using Microsoft.Extensions.Logging;

namespace MariCommands.Utils
{
    internal static class MiddlewareUtils
    {
        public static bool VerifyMatchDisabled(CommandContext context, ICommandMatch match, ILogger logger = null)
        {
            if (!match.Command.IsEnabled)
            {
                logger.LogInformation("The matched command is disabled.");
                context.Result = CommandDisabledResult.FromCommand(match.Command);

                return true;
            }

            return true;
        }


        public static IResult GetErrorResult(Dictionary<ICommand, IResult> failedMatches)
        {
            if (failedMatches.Count == 1)
                return failedMatches.FirstOrDefault().Value;

            return MatchesFailedResult.FromFaileds(failedMatches);
        }

        public static void RegisterForDispose(object instance, CommandContext context)
        {
            switch (instance)
            {
                case IAsyncDisposable asyncDisposable:
                    context.RegisterForDisposeAsync(asyncDisposable);
                    break;

                case IDisposable disposable:
                    context.RegisterForDispose(disposable);
                    break;
            }
        }

        public static async ValueTask SwitchDisposeAsync(object instance)
        {
            switch (instance)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}